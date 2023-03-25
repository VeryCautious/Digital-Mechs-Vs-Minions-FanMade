using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Graphics.UserInteractions;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

namespace Mechs_Vs_Minions_Graphics.Graphics.GameWindow;

internal partial class GameWindow
{
    private CancellationToken _cancellationToken;
    private Vector2i _lastHoveredPixel;

    private void AddLightKeyBindings()
    {
        _keyBindings.Add(Keys.S, () => _worldGraphicsState.UpdateLightPosition(light => light - 0.1f * Vector3.UnitZ));
        _keyBindings.Add(Keys.W, () => _worldGraphicsState.UpdateLightPosition(light => light + 0.1f * Vector3.UnitZ));
        _keyBindings.Add(Keys.A, () => _worldGraphicsState.UpdateLightPosition(light => light - 0.1f * Vector3.UnitX));
        _keyBindings.Add(Keys.D, () => _worldGraphicsState.UpdateLightPosition(light => light + 0.1f * Vector3.UnitX));
        _keyBindings.Add(Keys.E, () => _worldGraphicsState.UpdateLightPosition(light => light - 0.1f * Vector3.UnitY));
        _keyBindings.Add(Keys.Z, () => _worldGraphicsState.UpdateLightPosition(light => light + 0.1f * Vector3.UnitY));
    }

    private void AddCameraKeyBindings()
    {
        _keyBindings.Add(Keys.Left, () => _cameraManager.MoveTarget(-Vector3.UnitX));
        _keyBindings.Add(Keys.Right, () => _cameraManager.MoveTarget(Vector3.UnitX));
        _keyBindings.Add(Keys.Down, () => _cameraManager.MoveTarget(-Vector3.UnitZ));
        _keyBindings.Add(Keys.Up, () => _cameraManager.MoveTarget(Vector3.UnitZ));
        _keyBindings.Add(Keys.LeftControl, () => _cameraManager.MoveTarget(-Vector3.UnitY));
        _keyBindings.Add(Keys.LeftShift, () => _cameraManager.MoveTarget(Vector3.UnitY));

        _keyBindings.Add(Keys.KeyPad6, () => _cameraManager.MoveCounterClockwise());
        _keyBindings.Add(Keys.KeyPad4, () => _cameraManager.MoveClockwise());
        _keyBindings.Add(Keys.KeyPad8, () => _cameraManager.MoveUpOnSphere());
        _keyBindings.Add(Keys.KeyPad2, () => _cameraManager.MoveDownOnSphere());
        _keyBindings.Add(Keys.KeyPad9, () => _cameraManager.MoveIn());
        _keyBindings.Add(Keys.KeyPad1, () => _cameraManager.MoveOut());

        _keyBindings.Add(Keys.C, () => _cameraManager.ToggleCameraPose(_worldGraphicsState, _gameState));
    }
    private void AddOverlayKeyBindings()
    {
        _keyBindings.Add(Keys.Space, _overlayGraphicsState.ToggleVisibility);
        _keyBindings.Add(Keys.Escape, () =>
        {
            _menu.ToggleVisibility();
            if (_menu.IsVisible)
            {
                _overlayGraphicsState.SetVisibility(false);
            }
        });
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        _lastHoveredPixel = new Vector2i((int) e.X, (int) e.Y);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        InterpretMousePress(_lastHoveredPixel).Wait(_cancellationToken);
    }

    private UserInteraction? FindUserInteractionInWorldSpace(Vector2 pixelCoordinates)
    {
        var clickedColor = GetClickableDummyColorAt(pixelCoordinates);
        return _worldGraphicsState.GetUserInteractionFromClick(clickedColor);
    }
    private async Task InterpretMousePress(Vector2i pixelCoordinates)
    {
        var x = ((float) pixelCoordinates.X) / Size.X * 2.0f - 1.0f;
        var y = -( (float) pixelCoordinates.Y / Size.Y * 2.0f - 1.0f);
        var normalizedScreenClickPosition = new Vector2(x, y);
        
        if (_overlayGraphicsState.IsVisible)
        {
            var interaction = _overlayGraphicsState.GetUserInteractionFromClick(normalizedScreenClickPosition);
            if (interaction is not null)
            {
                await _userInteractions.Enqueue(interaction, _cancellationToken);
                _overlayGraphicsState.Notify(interaction);
            }
        }

        if (_menu.IsVisible)
        {
            var interaction2 = _menu.GetUserInteractionFromClick(normalizedScreenClickPosition);
            if (interaction2 is not null)
            {
                await _userInteractions.Enqueue(interaction2, _cancellationToken);
            }
        }

        var interaction3 = FindUserInteractionInWorldSpace(pixelCoordinates);
        if (interaction3 is not null)
        {
            await _userInteractions.Enqueue(interaction3, _cancellationToken);
        }
    }

    private Color4 GetClickableDummyColorAt(Vector2 pixelCoordinates) =>
        _clickDummyTextureBuffer.GetColorAt(new Vector2i((int) pixelCoordinates.X, (int) pixelCoordinates.Y));

    public async Task<FieldSelectionResult> Prompt(SelectFieldFrom userPrompt)
    {
        _userInteractions.Clear();
        _worldGraphicsState.OutlinedTiles = userPrompt.Fields.ToHashSet();

        int selectedIndex;
        do
        {
            var userInput = await WaitFor<FieldSelected>();
            selectedIndex = userPrompt.Fields.IndexOf(userInput.Point, 0, userPrompt.Fields.Count, EqualityComparer<Point>.Default);
        } while (selectedIndex == -1);

        _worldGraphicsState.OutlinedTiles = new HashSet<Point>();
        
        Console.WriteLine(selectedIndex);
        return new FieldSelectionResult(userPrompt.Fields[selectedIndex]);
    }

    public async Task<CardSelectionResult> Prompt(SelectCardFrom userPrompt)
    {   
        _userInteractions.Clear();
        for (var cardIndex = 0; cardIndex < userPrompt.Cards.Count; cardIndex++)
        {
            _keyBindings.Add(KeyFor(cardIndex+1), CreatePushAction(new DrawableCardSelected(cardIndex)));
        }

        DrawableCardSelected drawableCardClicked;
        do
        {
            drawableCardClicked = await WaitFor<DrawableCardSelected>();
        } while (!Enumerable.Range(0, userPrompt.Cards.Count).Contains(drawableCardClicked.CardIndex));

        _keyBindings.Delete(Enumerable.Range(0, userPrompt.Cards.Count).Select(KeyFor));

        return new CardSelectionResult(userPrompt.Cards[drawableCardClicked.CardIndex]);
    }

    public async Task<SelectCommandSlotResult> Prompt(SelectCommandSlotFrom userPrompt)
    {
        _userInteractions.Clear();
        foreach (var slotIndex in userPrompt.SlotIndices)
        {
            _keyBindings.Add(KeyFor(slotIndex+1), CreatePushAction(new CommandLineSlotSelected(slotIndex)));
        }

        CommandLineSlotSelected commandLineSlotSelected;
        do
        {
            commandLineSlotSelected = await WaitFor<CommandLineSlotSelected>();
        } while (!userPrompt.SlotIndices.Contains(commandLineSlotSelected.SlotIndex));

        _keyBindings.Delete(userPrompt.SlotIndices.Select(KeyFor));
 
        return new SelectCommandSlotResult(commandLineSlotSelected.SlotIndex);
    }

    private static Keys KeyFor(int number)
        => number switch
        {
            0 => Keys.D0,
            1 => Keys.D1,
            2 => Keys.D2,
            3 => Keys.D3,
            4 => Keys.D4,
            5 => Keys.D5,
            6 => Keys.D6,
            7 => Keys.D7,
            8 => Keys.D8,
            9 => Keys.D9,
            _ => throw new ArgumentOutOfRangeException(nameof(number), "This number can not be represented by a key")
        };

    private Action CreatePushAction(UserInteraction userInteraction)
    {
        return () => _userInteractions.Enqueue(userInteraction, _cancellationToken).Wait(_cancellationToken);
    }

    public async Task<SelectRotationResult> Prompt(SelectRotationFrom userPrompt)
    {
        _userInteractions.Clear();
        var userInput = await WaitFor<RotationSelected>();
        var index = userInput.Rotation switch
        {
            Rotation.Plus90Degrees => 0,
            Rotation.Minus90Degrees => 1,
            Rotation.Plus180Degrees => 2,
            Rotation.NoRotation => 3,
            _ => 0
        };
        return new SelectRotationResult(userPrompt.Rotations[index]);
    }

    public async Task<ConfirmResult> Prompt(Confirm userPrompt)
    {
        _userInteractions.Clear();
        _keyBindings.Add(Keys.Enter, CreatePushAction(new ConfirmAction()));

        await WaitFor<ConfirmAction>();

        _keyBindings.Delete(Keys.Enter);

        return new ConfirmResult();
    }
    
    public async Task<MetaGameRequest> DequeueMetaGameInteraction(CancellationToken cancellation) => await _unhandledMetaGameActions.Dequeue(cancellation);
    private readonly AsyncQueue<UserInteraction> _userInteractions = new();
    private readonly AsyncQueue<MetaGameRequest> _unhandledMetaGameActions = new();

    private async Task<T> WaitFor<T>() where T : UserInteraction
    {
        while (true)
        {
            var interaction = await _userInteractions.Dequeue(_cancellationToken);
            if (interaction is T tInteraction)
            {
                return tInteraction;
            }

            if (interaction is MetaGameInteraction metaGameInteraction)
            {
                await _unhandledMetaGameActions.Enqueue(metaGameInteraction.Request, _cancellationToken);
                throw new TaskCanceledException();
            }
        }
    }
}
