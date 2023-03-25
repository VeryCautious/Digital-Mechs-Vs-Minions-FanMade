using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.Utilities;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Animations;
using Mechs_Vs_Minions_Graphics.Graphics.GameWindow;
using Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.UserInteractions;
using Mechs_Vs_Minions_Graphics.Utilities;
using Microsoft.VisualBasic.CompilerServices;
using OpenTK.Mathematics;
using Utilities;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

internal class WorldGraphicsState : IUniformProvider
{
    private readonly IAudioStore _audioStore;
    private readonly List<IAnimation> _animations = new();

    private readonly GraphicsState<LightModel, MinionInstance, WorldGraphicsState> _lightGraphicsState;
    private Vector3 _boardCoordinates = new(-4, 0, -4);
    private GameBoard? _loadedBoard;

    private readonly LightUniformSet _lightUniformSet;
    private readonly Func<GameState,Action> _reloadGameAssetsActionProvider;

    private readonly GraphicsState<CoordinateSystemModel, PositionalInstance, WorldGraphicsState> _coordinateSystemGraphicsState;
    private readonly GraphicsState<MechModel, MechInstance, WorldGraphicsState> _mechGraphicsState;
    private readonly Translator<Guid, Handle<MechInstance>> _mechHandleTranslator;
    private readonly GraphicsState<MinionModel, MinionInstance, WorldGraphicsState> _minionGraphicsState;
    private readonly Translator<Guid, Handle<MinionInstance>> _minionHandleTranslator;
    private readonly GraphicsState<ParticleModel, ParticleInstance, IUniformProvider> _particleGraphicsState;
    private readonly GraphicsState<RipsawModel, RipSawInstance, IUniformProvider> _ripSawGraphicsState;
    private readonly GraphicsState<SkyBoxModel, PositionalInstance, IUniformProvider> _skyBoxGraphicsState;

    private readonly GraphicsState<RockModel, PositionalInstance, IUniformProvider> _rockGraphicsState;
    private readonly GraphicsState<GameBoardModel, PositionalInstance, IUniformProvider> _gameBoardGraphicsState;
    private readonly GraphicsState<GameBoardRockModel, PositionalInstance, IUniformProvider> _gameBoardRockGraphicsState;

    private readonly GraphicsState<SquareOutlineModel, BoardTileInstance, IUniformProvider> _tileOutlineState;
    private readonly Translator<Point, Handle<BoardTileInstance>> _boardTileTranslator;
    public HashSet<Point> OutlinedTiles;
    private readonly GraphicsState<ClickDummySquareModel, ClickableInstance, IUniformProvider> _clickDummyState;
    private readonly Clickables _clickables;

    private readonly GraphicsState<PolylineModel, PositionalInstance, IUniformProvider> _polylineGraphicsState;

    private Handle<MinionInstance>? _lightHandle;

    private readonly IGraphicsModelProvider _graphicsModelState;

    public Vector3 LightPosition { get; private set; } = new(0.0f, -4.0f, 0.0f);

    private readonly List<Circle> _stonePositions;
    public IImmutableList<Circle> StonePositions => _stonePositions.ToImmutableList();
    
    private GameState? _nextGameState;
    private Handle<MechInstance>? _selectedHandle;

    private readonly GameCameraManager _cameraManager;

    public WorldGraphicsState(IGraphicsModelProvider graphicsModelState, IAudioStore audioStore, LightUniformSet lightUniformSet, Func<GameState,Action> reloadGameAssetsActionProvider, GameCameraManager cameraManager)
    {
        _audioStore = audioStore;
        _lightUniformSet = lightUniformSet;
        _reloadGameAssetsActionProvider = reloadGameAssetsActionProvider;
        _graphicsModelState = graphicsModelState;

        _mechHandleTranslator = new Translator<Guid, Handle<MechInstance>>();
        _minionHandleTranslator = new Translator<Guid, Handle<MinionInstance>>();
        _boardTileTranslator = new Translator<Point, Handle<BoardTileInstance>>();

        _mechGraphicsState = new GraphicsState<MechModel, MechInstance, WorldGraphicsState>(this);
        _minionGraphicsState = new GraphicsState<MinionModel, MinionInstance, WorldGraphicsState>(this);
        _skyBoxGraphicsState = new GraphicsState<SkyBoxModel, PositionalInstance, IUniformProvider>(this);
        _lightGraphicsState = new GraphicsState<LightModel, MinionInstance, WorldGraphicsState>(this);
        _particleGraphicsState = new GraphicsState<ParticleModel, ParticleInstance, IUniformProvider>(this);
        _ripSawGraphicsState = new GraphicsState<RipsawModel, RipSawInstance, IUniformProvider>(this);
        _coordinateSystemGraphicsState = new GraphicsState<CoordinateSystemModel, PositionalInstance, WorldGraphicsState>(this);
        _rockGraphicsState = new GraphicsState<RockModel, PositionalInstance, IUniformProvider>(this);
        _gameBoardGraphicsState = new GraphicsState<GameBoardModel, PositionalInstance, IUniformProvider>(this);
        _gameBoardRockGraphicsState = new GraphicsState<GameBoardRockModel, PositionalInstance, IUniformProvider>(this);

        _stonePositions = new List<Circle>();

        _tileOutlineState = new GraphicsState<SquareOutlineModel, BoardTileInstance, IUniformProvider>(this);
        _clickDummyState = new GraphicsState<ClickDummySquareModel, ClickableInstance, IUniformProvider>(this);
        _clickables = new Clickables();
        OutlinedTiles = new();

        _polylineGraphicsState = new GraphicsState<PolylineModel, PositionalInstance, IUniformProvider>(this);

        _cameraManager = cameraManager;
    }

    public void OnAfterLoad(GameState firstGameState)
    {
        _minionGraphicsState.AddModel(_graphicsModelState.GetRenderModel<BlueMinionModel, MinionInstance>());
        _minionGraphicsState.AddModel(_graphicsModelState.GetRenderModel<RedMinionModel, MinionInstance>());
        _mechGraphicsState.AddModel(_graphicsModelState.GetRenderModel<TristanaModel, MechInstance>());
        _mechGraphicsState.AddModel(_graphicsModelState.GetRenderModel<HeimerdingerModel, MechInstance>());
        _mechGraphicsState.AddModel(_graphicsModelState.GetRenderModel<CorkiModel, MechInstance>());
        _mechGraphicsState.AddModel(_graphicsModelState.GetRenderModel<ZiggsModel, MechInstance>());
        _skyBoxGraphicsState.AddModel(_graphicsModelState.GetRenderModel<SkyBoxModel, PositionalInstance>());
        _lightGraphicsState.AddModel(_graphicsModelState.GetRenderModel<LightModel, MinionInstance>());
        _particleGraphicsState.AddModel(_graphicsModelState.GetRenderModel<ParticleModel, ParticleInstance>());
        _ripSawGraphicsState.AddModel(_graphicsModelState.GetRenderModel<RipsawModel, RipSawInstance>());
        _coordinateSystemGraphicsState.AddModel(_graphicsModelState.GetRenderModel<CoordinateSystemModel, PositionalInstance>());
        _clickDummyState.AddModel(_graphicsModelState.GetRenderModel<ClickDummySquareModel, ClickableInstance>());
        _tileOutlineState.AddModel(_graphicsModelState.GetRenderModel<SquareOutlineModel, BoardTileInstance>());
        _gameBoardRockGraphicsState.AddModel(_graphicsModelState.GetRenderModel<GameBoardRockModel, PositionalInstance>());

        _skyBoxGraphicsState.InstanceStore<SkyBoxModel>().CreateInstance(() => new PositionalInstance(Matrix4.Identity));

        _lightHandle = _lightGraphicsState.InstanceStore<LightModel>().CreateInstance(MinionInstance.DefaultFactory);
        SetLightPosition(LightPosition);

        _coordinateSystemGraphicsState.InstanceStore<CoordinateSystemModel>().CreateInstance(
            () => new PositionalInstance(Matrix4.Identity)
        );

        var boardCenter = firstGameState.Board.GetCenter();
        var radius = 1 + firstGameState.Board.BoardTiles.Keys
            .Select(point => new PointF(point.X + 0.5f, point.Y + 0.5f))
            .Select(pointCenter => pointCenter.DistanceTo(boardCenter) + 1)
            .Max();
        _gameBoardRockGraphicsState.InstanceStore<GameBoardRockModel>().CreateInstance(
            () => new PositionalInstance(Matrix4.CreateTranslation(0, 0.05f, 0) * Matrix4.CreateScale(radius))
        );

        _polylineGraphicsState.AddModel(_graphicsModelState.GetRenderModel<PolylineModel, PositionalInstance>());

    }

    public bool AnimationsPlaying => _animations.Any();
    public IImmutableList<Uniform> GetUniforms() => _lightUniformSet.GetUniforms();

    public void Draw()
    {
        _skyBoxGraphicsState.DrawAllInstances();
        // _coordinateSystemGraphicsState.DrawAllInstances(); // only for debug
        _mechGraphicsState.DrawAllInstances();
        //_lightGraphicsState.DrawAllInstances(); would occlude shadow-map
        _particleGraphicsState.DrawAllInstances();
        _minionGraphicsState.DrawAllInstances();
        _rockGraphicsState.DrawAllInstances();
        _gameBoardGraphicsState.DrawAllInstances();
        _gameBoardRockGraphicsState.DrawAllInstances();
        _tileOutlineState.DrawAllInstances();
        // _polylineGraphicsState.DrawAllInstances(); // only for debug
        _ripSawGraphicsState.DrawAllInstances(); //Must be last because it has alpha
    }

    public void DrawClickableDummies()
    {
        _clickDummyState.DrawAllInstances();
    }

    public void Display(Transition transition, GameState gameState)
    {
        _animations.AddRange(AnimationFrom(transition, gameState));
        _nextGameState = gameState;
    }

    public void UpdateLightPosition(Func<Vector3, Vector3> modifier)
    {
        LightPosition = modifier.Invoke(LightPosition);
        _lightUniformSet.LightPositionUniform.Vector = LightPosition;
        _lightGraphicsState.InstanceStore<LightModel>().Update(_lightHandle ?? throw new IncompleteInitialization(),
            _ => new MinionInstance(Matrix4.CreateTranslation(LightPosition)));
    }

    public void Update(TimeSpan timeSinceLastFrame)
    {
        _animations.ForEach(animation => animation.Update(timeSinceLastFrame));
        var finishedAnimations = _animations.Where(animation => animation.HasFinished()).ToArray();
        foreach (var finishedAnimation in finishedAnimations)
        {
            _animations.Remove(finishedAnimation);
            finishedAnimation.Dispose();
        }

        if (!_animations.Any()) Update(_nextGameState);
    }

    private IEnumerable<IAnimation> AnimationFrom(Transition transition, GameState gameState)
    {
        return transition switch
        {
            NoTransition => Enumerable.Empty<IAnimation>(),
            MinionMoveTransition minionMoveTransition => minionMoveTransition.GetAnimations(
                gameState,
                ToWorldSpaceFromBoard,
                id => _minionGraphicsState.InstanceStore(MinionTypeFromNumber(id)),
                minionMoveTransition.MoveTransitions.Select(move => _minionHandleTranslator.Get(move.Id)).ToImmutableList(),
                _audioStore),
            UseOmniStompTransition moveMechTransition => moveMechTransition.GetAnimation(gameState, ToWorldSpaceFromBoard, 
                id => _mechGraphicsState.InstanceStore(GetMechTypeById(id, gameState)),
                _mechHandleTranslator,
                id => _minionGraphicsState.InstanceStore(MinionTypeFromNumber(id)),
                _minionHandleTranslator,
                _audioStore,
                _cameraManager
            ),
            UseBlazeTransition useBlazeTransition => useBlazeTransition.GetAnimation(
                gameState,
                ToWorldSpaceFromBoard,
                id => _mechGraphicsState.InstanceStore(GetMechTypeById(id, gameState)),
                _mechHandleTranslator,
                id => _minionGraphicsState.InstanceStore(MinionTypeFromNumber(id)),
                _minionHandleTranslator,
                _particleGraphicsState.InstanceStore<ParticleModel>(),
                _audioStore,
                _cameraManager
            ),
            UseScytheTransition useScytheTransition => useScytheTransition.GetAnimation(
                gameState,
                ToWorldSpaceFromBoard,
                _mechGraphicsState.InstanceStore(GetMechTypeById(useScytheTransition.MechId, gameState)),
                _mechHandleTranslator.Get(useScytheTransition.MechId),
                _particleGraphicsState.InstanceStore<ParticleModel>(),
                id => _minionGraphicsState.InstanceStore(MinionTypeFromNumber(id)),
                _minionHandleTranslator,
                _audioStore,
                _cameraManager
            ),
            UseRipSawTransition useRipSawTransition => useRipSawTransition.GetAnimation(
                gameState,
                id => _minionGraphicsState.InstanceStore(MinionTypeFromNumber(id)),
                _minionHandleTranslator,
                ToWorldSpaceFromBoard,
                _ripSawGraphicsState.InstanceStore<RipsawModel>(),
                _audioStore,
                _cameraManager
            ),
            UseSpeedTransition useSpeedTransition => useSpeedTransition.GetAnimation(
                gameState,
                ToWorldSpaceFromBoard, 
                id => _mechGraphicsState.InstanceStore(GetMechTypeById(id, gameState)),
                _mechHandleTranslator,
                id => _minionGraphicsState.InstanceStore(MinionTypeFromNumber(id)),
                _minionHandleTranslator,
                _audioStore,
                _cameraManager
            ),
            UseStuckControlsTransition useStuckControlsTransition => useStuckControlsTransition.GetAnimation(
                gameState, 
                ToWorldSpaceFromBoard,
                _mechGraphicsState.InstanceStore(GetMechTypeById(useStuckControlsTransition.MechId, gameState)), 
                _mechHandleTranslator.Get(useStuckControlsTransition.MechId),
                _cameraManager
            ),
            ApplyGlitchDamageToMechTransition applyGlitchDamageToMechTransition => applyGlitchDamageToMechTransition.GetAnimation(
                gameState, 
                ToWorldSpaceFromBoard,
                _particleGraphicsState.InstanceStore<ParticleModel>(),
                _audioStore
            ),
            ApplyStuckControlsToMechTransition applyStuckControlsToMechTransition => applyStuckControlsToMechTransition.GetAnimation(
                gameState, ToWorldSpaceFromBoard,
                _particleGraphicsState.InstanceStore<ParticleModel>(),
                _audioStore
            ),
            UseAimBotTransition applyStuckControlsToMechTransition => applyStuckControlsToMechTransition.GetAnimation(
                gameState,
                ToWorldSpaceFromBoard,
                _particleGraphicsState.InstanceStore<ParticleModel>(),
                id => _minionGraphicsState.InstanceStore(MinionTypeFromNumber(id)),
                _minionHandleTranslator,
                _audioStore,
                _cameraManager
            ),
            StartOfPlayActionPhaseTransition startOfPlayActionPhaseTransition => startOfPlayActionPhaseTransition.GetAnimations(_cameraManager),
            _ => throw new ArgumentException("Transition type is not handled", nameof(transition))
        };
    }

    private static Type GetMechTypeById(Guid mechId, GameState gameState)
    {
        var mechState = gameState.MechStates.Single(state => state.Id == mechId);
        return MechTypeFromPlayerNumber(gameState.MechStates.IndexOf(mechState));
    }

    private void Update(GameState? gameState)
    {
        if (gameState == null) return;

        if(gameState.Board != _loadedBoard)
            UpdateGameBoard(gameState);
        DestroyUnusedHandles(gameState);
        UpdateMechs(gameState);
        UpdateMinions(gameState);
        UpdateBoardTiles(gameState);
    }

    private void UpdateGameBoard(GameState gameState)
    {
        _rockGraphicsState.DestroyAllVersionsOf<RockModel>();

        _reloadGameAssetsActionProvider(gameState)();

        _rockGraphicsState.ClearInstances();
        _gameBoardGraphicsState.ClearInstances();

        if (StonePositions.Count > 0)
        {
            _graphicsModelState.GetProceduralRenderModels<RockModel, PositionalInstance>()
                .Select((m, i) => new { Model = m, Index = i}).ToList()
                .ForEach(entry => _rockGraphicsState.AddModel(entry.Model, entry.Index));
        }
        _gameBoardGraphicsState.DestroyAllVersionsOf<GameBoardModel>();
        _gameBoardGraphicsState.AddModel(_graphicsModelState.GetRenderModel<GameBoardModel, PositionalInstance>());

        var boardCenter = gameState.Board.GetCenter();
        _boardCoordinates = new Vector3(-boardCenter.X, 0, -boardCenter.Y);

        for (var i = 0; i < _stonePositions.Count; i++)
        {
            var c = _stonePositions[i];
            _rockGraphicsState.InstanceStore<RockModel>(i).CreateInstance(
                () => new PositionalInstance(
                    Matrix4.CreateScale(c.Radius) * Matrix4.CreateTranslation(BoardToWorldSpaceTransformation.Transform(new Vector3(c.Position.X, -0.15f, c.Position.Y)))
                )
            );
        }

        _gameBoardGraphicsState.InstanceStore<GameBoardModel>().CreateInstance(
            () => new PositionalInstance(BoardToWorldTransformMatrix(new Point(0, 0)))
        );

        var instanceStore = _clickDummyState.InstanceStore<ClickDummySquareModel>();
        foreach (var tilePos in gameState.Board.BoardTiles.Keys)
        {
            var worldSpacePos = BoardToWorldTransformMatrix(tilePos);
            var tileHandle = instanceStore.CreateInstance(
                () => new ClickableInstance(worldSpacePos, default)
            );
            _clickables.Add(tileHandle.Id, new FieldSelected(tilePos));
            _clickables.TryGetColorOf(tileHandle.Id, out var color);
            instanceStore.Update(tileHandle, new ClickableInstance(worldSpacePos, color));
        }

        _loadedBoard = gameState.Board;
    }

    public UserInteraction? GetUserInteractionFromClick(Color4 clickedColor) => _clickables.GetUserInteractionFor(clickedColor);

    private Handle<MechInstance>? GetSelectionId(GameState gameState)
    {
        return gameState.GamePhase switch
        {
            DrawCardPhase drawCardPhase
                => _mechHandleTranslator.Get(gameState.MechStates[drawCardPhase.MechIndex]),
            MechPlayActionsPhase mechPlayActionsPhase
                => _mechHandleTranslator.Get(gameState.MechStates[mechPlayActionsPhase.MechIndex]),
            _ => null
        };
    }

    private void SetLightPosition(Vector3 position)
    {
        LightPosition = position;
        _lightUniformSet.LightPositionUniform.Vector = LightPosition;
        _lightGraphicsState.InstanceStore<LightModel>().Update(
            _lightHandle ?? throw new IncompleteInitialization(),
            instance => instance with { ModelTransform = Matrix4.CreateTranslation(LightPosition) }
        );
    }

    private void DestroyUnusedHandles(GameState gameState)
    {
        var unusedMechIds = _mechHandleTranslator.Keys.Except(gameState.MechStates.Select(mech => mech.Id))
            .ToArray();
        var unusedMinionIds = _minionHandleTranslator.Keys.Except(gameState.MinionStates.Select(mech => mech.Id))
            .ToArray();

        _mechGraphicsState.Destroy(unusedMechIds.Select(id => _mechHandleTranslator.Get(id))
            .ToImmutableList());
        _minionGraphicsState.Destroy(unusedMinionIds.Select(id => _minionHandleTranslator.Get(id))
            .ToImmutableList());

        foreach (var id in unusedMechIds) _mechHandleTranslator.Remove(id);
        foreach (var id in unusedMinionIds) _minionHandleTranslator.Remove(id);
    }

    private void UpdateMechs(GameState gameState)
    {
        var mechHandles = new List<Handle<MechInstance>>();
        for (var i = 0; i < gameState.MechStates.Count; i++)
        {
            var mechState = gameState.MechStates[i];
            var mechType = MechTypeFromPlayerNumber(i);
            mechHandles.Add(_mechHandleTranslator.GetOrCreate(
                mechState,
                () => _mechGraphicsState.InstanceStore(mechType).CreateInstance(MechInstance.DefaultFactory)
            ));
        }

        _selectedHandle = GetSelectionId(gameState);

        for (var i = 0; i < gameState.MechStates.Count; i++)
        {
            var handle = mechHandles[i];
            var mechState = gameState.MechStates[i];
            var mechAngle = mechState.Orientation.ToFloatAngle();
            _mechGraphicsState.InstanceStoreOf(handle).Update(
                handle,
                instance => instance with
                {
                    ModelTransform = Matrix4.CreateRotationY(mechAngle) * ToWorldSpaceTransform(mechState.Position),
                    IsOutlined = _selectedHandle == handle
                }
            );
        }
    }

    private static Type MechTypeFromPlayerNumber(int i)
    {
        return i switch
        {
            0 => typeof(TristanaModel),
            1 => typeof(HeimerdingerModel),
            2 => typeof(CorkiModel),
            3 => typeof(ZiggsModel),
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };
    }

    private void UpdateMinions(GameState gameState)
    {
        foreach (var minionState in gameState.MinionStates)
        {
            var minionType = MinionTypeFromNumber(minionState.Id);
            var handle = _minionHandleTranslator.GetOrCreate(
                minionState,
                () => _minionGraphicsState.InstanceStore(minionType).CreateInstance(MinionInstance.DefaultFactory)
            );
            var minionAngle = minionState.Orientation.ToFloatAngle();
            _minionGraphicsState.InstanceStoreOf(handle).Update(
                handle,
                instance => instance with
                {
                    ModelTransform = Matrix4.CreateRotationY(minionAngle) * ToWorldSpaceTransform(minionState.Position)
                }
            );
        }
    }

    private void UpdateBoardTiles(GameState gameState)
    {
        foreach (var tilePos in gameState.Board.BoardTiles.Keys)
        {
            var handle = _boardTileTranslator.GetOrCreate(
                tilePos,
                () => _tileOutlineState.InstanceStore<SquareOutlineModel>().CreateInstance(
                    () => new BoardTileInstance(BoardToWorldTransformMatrix(tilePos) * Matrix4.CreateTranslation(0, -1e-2f, 0), false)
                )
            );
            _tileOutlineState.InstanceStore<SquareOutlineModel>().Update(
                handle,
                instance => instance with
                {
                    IsOutlined = OutlinedTiles.Contains(tilePos)
                }
            );
        }
    }

    public (Vector3 pos, float azimuthAngle) GetMechWorldPose(MechState mechState)
    {
        var pos = new Vector3(mechState.Position.X + 0.5f, 0.0f, mechState.Position.Y + 0.5f) + _boardCoordinates;
        var azimuthAngle = -mechState.Orientation.ToFloatAngle();
        return (pos, azimuthAngle);
    }

    private static Type MinionTypeFromNumber(Guid id)
    {
        var random = new Random(id.GetHashCode());
        var modelNumber = random.Next(0, 2);
        return modelNumber == 0 ? typeof(BlueMinionModel) : typeof(RedMinionModel);
    }


    private Matrix4 ToWorldSpaceTransform(Point boardPosition)
    {
        var location = new Vector3(boardPosition.X + 0.5f, 0.0f, boardPosition.Y + 0.5f);
        return Matrix4.CreateTranslation(location) * BoardToWorldSpaceTransformation;
    }

    private Matrix4 BoardToWorldSpaceTransformation => Matrix4.CreateTranslation(_boardCoordinates);

    private Vector3 ToWorldSpaceFromBoard(Point boardPosition)
    {
        return BoardToWorldSpaceTransformation.Transform(new Vector3(boardPosition.X + .5f, 0.0f, boardPosition.Y + .5f));
    }

    private static Matrix4 SwapYAndZMatrix => new(
        1.0f, 0, 0, 0,
        0, 0, 1.0f, 0,
        0, 1.0f, 0, 0,
        0, 0, 0, 1.0f
    );

    private Matrix4 BoardToWorldTransformMatrix(Point boardPosition)
        => SwapYAndZMatrix * Matrix4.CreateTranslation(new Vector3(boardPosition.X, -0, boardPosition.Y)) * Matrix4.CreateTranslation(_boardCoordinates);

    public void AddStones(IEnumerable<Circle> newStoneCircles) => _stonePositions.AddRange(newStoneCircles);
    public void ClearStones() => _stonePositions.Clear();
}