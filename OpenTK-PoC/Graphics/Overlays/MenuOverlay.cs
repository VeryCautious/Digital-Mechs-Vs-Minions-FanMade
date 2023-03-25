using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.UserInteractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal class MenuOverlay : GamePhaseOverlayManager
{

    private InstanceHandle? _resetGameElementId;

    public MenuOverlay(float screenHwRatio, IGraphicsModelProvider graphicsModelProvider, ITexturesStore texturesStore, IUserInteractionLookup userInteractionLookup) 
        : base(screenHwRatio, graphicsModelProvider, texturesStore, userInteractionLookup)
    {
    }

    public override void Init(GameState? gameState)
    {
        SetVisibility(false);
        _resetGameElementId = OverlaySceneGraph.AddToRoot(
            new OverlaySceneGraphRectangle(.15f, .15f, 0f, 0f, 1f),
            typeof(ResetGameMenuModel)
        );
        OverlaySceneGraph.MoveZIndex(_resetGameElementId, 0.1f);
        UserInteractionLookup.Add(_resetGameElementId, new MetaGameInteraction(new ResetGameRequest()));
    }

    public override void Destroy()
    {
        if (_resetGameElementId is null) return;
        OverlaySceneGraph.Delete(_resetGameElementId);
    }

    protected override void Update()
    {
        if (_resetGameElementId is null) return;
        OverlaySceneGraph.UpdateInstance(_resetGameElementId);
    }
}