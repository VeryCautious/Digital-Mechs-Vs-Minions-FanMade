using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Overlays;
using Mechs_Vs_Minions_Graphics.UserInteractions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

internal class OverlayGraphicsState
{
    private GamePhaseOverlayManager? _gamePhaseOverlayManager;
    private readonly IGraphicsModelProvider _modelProvider;
    private readonly ITexturesStore _texturesStore;
    private readonly IUserInteractionLookup _userInteractionLookup;

    public OverlayGraphicsState(IGraphicsModelProvider modelProvider, ITexturesStore texturesStore, IUserInteractionLookup userInteractionLookup)
    {
        _modelProvider = modelProvider;
        _texturesStore = texturesStore;
        _userInteractionLookup = userInteractionLookup;
    }

    public void Update(GameState? gameState, float screenHwRatio)
    {
        SwitchOutOverlayManager(gameState, screenHwRatio);
    }

    public void Resize(float screenHwRatio)
    {
        _gamePhaseOverlayManager?.OnResize(screenHwRatio);
    }

    private void SwitchOutOverlayManager(GameState? gameState, float screenHwRatio)
    {
        _gamePhaseOverlayManager?.Destroy();

        if (gameState is null)
        {
            _gamePhaseOverlayManager = new LoadingOverlayManager(screenHwRatio, _modelProvider, _texturesStore, _userInteractionLookup);
            _gamePhaseOverlayManager?.Init(null);
            return;
        }

        _gamePhaseOverlayManager = gameState.GamePhase switch
        {
            DrawCardPhase => new DrawCardPhaseOverlayManager(screenHwRatio, _modelProvider, _texturesStore, _userInteractionLookup),
            MechPlayActionsPhase gamePhase => GetMechPlayActionsPhaseManager(gameState, gamePhase, screenHwRatio),
            EndOfGamePhase => new VictoryOverlayManager(screenHwRatio, _modelProvider, _texturesStore, _userInteractionLookup),
            _ => null
        };

        _gamePhaseOverlayManager?.Init(gameState);
    }

    private GamePhaseOverlayManager GetMechPlayActionsPhaseManager(GameState gameState, MechPlayActionsPhase gamePhase, float screenHwRatio)
    {
        var card = gameState.MechStates[gamePhase.MechIndex].CommandLine.TopCardAt(gamePhase.SlotToPlay);
        return card switch
        {
            OmniStompCard => new OmniStompOverlayManager(screenHwRatio, _modelProvider, _texturesStore, _userInteractionLookup),
            ScytheCard => new ScytheOverlayManager(screenHwRatio, _modelProvider, _texturesStore, _userInteractionLookup),
            _ => new ConfirmCardOverlayManager(screenHwRatio, _modelProvider, _texturesStore, _userInteractionLookup),
        };
    }

    public void Draw()
    {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _gamePhaseOverlayManager?.Draw();

        GL.Disable(EnableCap.Blend);
    }

    public void ToggleVisibility()
    {
        _gamePhaseOverlayManager?.ToggleVisibility();
    }

    public bool IsVisible => _gamePhaseOverlayManager?.IsVisible ?? false;
    public void SetVisibility(bool visible) => _gamePhaseOverlayManager?.SetVisibility(visible);

    public UserInteraction? GetUserInteractionFromClick(Vector2 normalizedScreenClickPosition)
    {
        return _gamePhaseOverlayManager?.GetUserInteractionFromClick(normalizedScreenClickPosition);
    }

    public void Notify(UserInteraction interaction)
    {
        _gamePhaseOverlayManager?.Notify(interaction);
    }
}