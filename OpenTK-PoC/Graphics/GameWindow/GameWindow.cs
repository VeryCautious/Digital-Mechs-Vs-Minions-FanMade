using System.Diagnostics;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.Overlays;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.UserInteractions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

namespace Mechs_Vs_Minions_Graphics.Graphics.GameWindow;

internal sealed partial class GameWindow : OpenTK.Windowing.Desktop.GameWindow, IController
{
    private static NativeWindowSettings WindowSettings
    {
        get
        {
            var image = TexturesStore.LoadImageFromFile("Models/mvmlogo.png", false);
            return new NativeWindowSettings
            {
                Size = new Vector2i(1200, 630),
                API = ContextAPI.OpenGL,
                APIVersion = new Version("4.0"),
                AutoLoadBindings = true,
                Icon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image(image.Width, image.Height, image.Data))
            };
        }
    }

    private readonly ShadowMapBuffer _shadowMapBuffer;
    private readonly ClickDummyTextureBuffer _clickDummyTextureBuffer;

    private readonly GameCameraManager _cameraManager;

    private readonly IGameStateProvider _gameStateProvider;
    
    private readonly GraphicsModelStore _graphicsModelStore;
    private readonly CameraUniformSet _cameraUniformSet;
    private readonly ITexturesStore _texturesStore;

    private readonly OverlayGraphicsState _overlayGraphicsState;
    private readonly OverlayUniformSet _overlayUniformSet;

    private readonly Stopwatch _stopwatch = new();

    private readonly WorldGraphicsState _worldGraphicsState;
    private DateTime _currentGameStateSetTime;
    private GameState? _gameState;

    private float _screenHwRatio;

    private readonly KeyBindings _keyBindings;

    private readonly Queue<Task> _loadingTasks = new();
    private int _toBeLoadedItemCount;
    private string _titlePrefix = "";

    private readonly MenuOverlay _menu;

    private Action ReloadGameAssetsActionProvider(GameState gameState) => 
        () => ReLoadGameStateAssets(_graphicsModelStore, _texturesStore, _cameraUniformSet, gameState);

    private int LoadingPercentage => 
        _toBeLoadedItemCount == 0 ? 100 : (_toBeLoadedItemCount - _loadingTasks.Count) * 100 / _toBeLoadedItemCount;
    private bool DoneLoading => !_loadingTasks.Any();
    private bool _pleaseSaveShadowMap;

    public GameWindow(
        IGameStateProvider gameStateProvider,
        ITexturesStore texturesStore,
        IUserInteractionLookup userInteractionLookup,
        IObjStore objStore,
        LightUniformSet lightUniformSet,
        CameraUniformSet cameraUniformSet,
        OverlayUniformSet overlayUniformSet,
        GraphicsModelStore graphicsModelStore,
        ParticleUniformSet particleUniformSet,
        IAudioStore audioStore
    ) : base(new GameWindowSettings { RenderFrequency = 60, UpdateFrequency = 60 }, WindowSettings)
    {
        _cameraManager = new GameCameraManager(cameraUniformSet);
        
        _graphicsModelStore = graphicsModelStore;
        _cameraUniformSet = cameraUniformSet;
        _texturesStore = texturesStore;

        _pleaseSaveShadowMap = false;
        _shadowMapBuffer = new ShadowMapBuffer(lightUniformSet, _cameraUniformSet, _cameraManager);

        _graphicsModelStore.AddRenderModel<LoadingScreenModel,OverlayInstance>(new LoadingScreenModel(_texturesStore, overlayUniformSet));

        _loadingTasks.Enqueue(new Task(() => _shadowMapBuffer.Load())); //Must be first
        _loadingTasks.Enqueue(new Task(() => LoadAudio(audioStore)));
        EnqueueLoadGameModels(_graphicsModelStore, _texturesStore, objStore, _cameraUniformSet, particleUniformSet);
        EnqueueLoadOverlayModels(_graphicsModelStore, _texturesStore, overlayUniformSet);
        _loadingTasks.Enqueue(new Task(() => _menu!.Init(null)));

        _toBeLoadedItemCount = _loadingTasks.Count;

        _gameStateProvider = gameStateProvider;
        _overlayUniformSet = overlayUniformSet;

        _worldGraphicsState = new WorldGraphicsState(_graphicsModelStore, audioStore, lightUniformSet, ReloadGameAssetsActionProvider, _cameraManager);
        _overlayGraphicsState = new OverlayGraphicsState(_graphicsModelStore, _texturesStore, userInteractionLookup);
        
        _screenHwRatio = (int)Math.Floor((float)Size.X / Size.Y);
        _menu = new MenuOverlay(_screenHwRatio, _graphicsModelStore, _texturesStore, userInteractionLookup);
        _keyBindings = new KeyBindings();
        AddLightKeyBindings();
        AddCameraKeyBindings();
        AddOverlayKeyBindings();
        _keyBindings.Add(Keys.Enter, () => _pleaseSaveShadowMap = true);

        _clickDummyTextureBuffer = new ClickDummyTextureBuffer(Size.X, Size.Y);
        _clickDummyTextureBuffer.Load();
    }

    protected override void OnLoad()
    {
        GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnRenderFrame(FrameEventArgs obj)
    {
        if (!DoneLoading || _gameState is null)
        {
            _overlayGraphicsState.Update(null, _screenHwRatio);
            DrawLoadingScreen();
            DoLoadingStep();
            return;
        }

        if (PullNextGameState(out var newGameState, out var transition))
        {
            _cameraManager.UpdateCameraPoses(_worldGraphicsState, _gameState);
            _gameState = newGameState;
            _currentGameStateSetTime = DateTime.Now;
            _worldGraphicsState.Display(transition, _gameState!);
            _overlayGraphicsState.Update(_gameState!, _screenHwRatio);
        }

        var elapsedTimeStamp = new TimeSpan(_stopwatch.ElapsedTicks);
        _worldGraphicsState.Update(elapsedTimeStamp);

        ShadowRenderPass();
        StandardRenderPass(elapsedTimeStamp);
        ClickablesRenderPass();
    }

    private void ShadowRenderPass()
    {
        _shadowMapBuffer.Bind(_worldGraphicsState.LightPosition);
        DrawBoardInstances();

        if (_pleaseSaveShadowMap)
        {
            _shadowMapBuffer.SaveToFile();
            _pleaseSaveShadowMap = false;
        }

        _shadowMapBuffer.Unbind();
    }

    private void ClickablesRenderPass()
    {
        _clickDummyTextureBuffer.Bind();

        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _worldGraphicsState.DrawClickableDummies();

        _clickDummyTextureBuffer.Unbind();
    }

    private void StandardRenderPass(TimeSpan elapsedTimeStamp)
    {
        GL.Viewport(0, 0, Size.X, Size.Y);

        GL.Enable(EnableCap.StencilTest);
        GL.Enable(EnableCap.DepthTest);

        GL.ClearColor(Color4.Black);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        GL.StencilMask(0x00);

        _worldGraphicsState.Update(elapsedTimeStamp);
        _stopwatch.Restart();

        DrawBoardInstances();
       
        GL.Clear(ClearBufferMask.DepthBufferBit); //Overlay should be on top of world
        DrawOverlay();
        GL.Clear(ClearBufferMask.DepthBufferBit); //Menu should always be on top
        _menu.Draw();
       
        SwapBuffers();
    }

    private void DoLoadingStep()
    {
        if (!DoneLoading)
        {
            ExecuteNextLoadingTask(out var wasLastTask);
            if (!wasLastTask) return;
        }

        if (_gameState is null)
        {
            if (_gameStateProvider.PeekGameState(out var firstGameState))
            {
                EnqueueLoadGameStateAssets(_graphicsModelStore, _texturesStore, _cameraUniformSet, firstGameState);
                _gameState = firstGameState;
            }
            else
            {
                Title = "Waiting for game state ...";
            }
            return;
        }

        Title = "Mechs vs. Minions";
        _worldGraphicsState.OnAfterLoad(_gameState);
    }

    private void DrawLoadingScreen()
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        DrawOverlay();
        SwapBuffers();
    }

    private void ExecuteNextLoadingTask(out bool wasLastTask){ 
        var loadingTask = _loadingTasks.Dequeue();
        loadingTask.RunSynchronously(); //OpenGL context only exists in main thread.

        Title = $"{_titlePrefix} {LoadingPercentage}%";

        wasLastTask = !_loadingTasks.Any();
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) => _keyBindings.OnKeyDown(e.Key);

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        UpdatePerspective();
        UpdateOverlay(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        _clickDummyTextureBuffer.Resize(Size);
    }

    private void UpdatePerspective()
    {
        _cameraManager.UpdatePerspective((float)ClientSize.X / ClientSize.Y);
    }

    private void DrawBoardInstances()
    {
        _worldGraphicsState.Draw();
    }

    private void DrawOverlay()
    {
        _overlayGraphicsState.Draw();
    }

    private void UpdateOverlay(ResizeEventArgs e)
    {
        _screenHwRatio = e.Height / (float)e.Width;
        _overlayUniformSet.ProjectionUniform.Matrix = Matrix4.CreateScale(1f, 1f / _screenHwRatio, 1f);
        _overlayGraphicsState.Resize(_screenHwRatio);
    }

    private bool PullNextGameState(out GameState newGameState, out Transition transition)
    {
        newGameState = null!;
        transition = null!;

        if ((_gameState != null && (DateTime.Now - _currentGameStateSetTime).Milliseconds <= 100) ||
            _worldGraphicsState.AnimationsPlaying) 
            return false;

        if (!_gameStateProvider.GetNext(out var oTransition, out var oNewGameState)) 
            return false;

        newGameState = oNewGameState;
        transition = oTransition;
        return true;
    }

    public void Display(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        Run();
    }
}