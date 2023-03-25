using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.UserInteractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal class VictoryOverlayManager : GamePhaseOverlayManager
{
    private InstanceHandle? _victoryId;

    public VictoryOverlayManager(
        float screenHwRatio,
        IGraphicsModelProvider graphicsModelProvider,
        ITexturesStore texturesStore,
        IUserInteractionLookup userInteractionLookup
    ) : base(
        screenHwRatio,
        graphicsModelProvider,
        texturesStore,
        userInteractionLookup
    )
    { }

    public override void Init(GameState? gameState)
    {
        if (gameState?.GamePhase is not EndOfGamePhase) throw new InvalidOperationException();

        var victoryModel = GraphicsModelProvider.GetRenderModel<VictoryOverlayModel, OverlayInstance>();
        _victoryId = OverlaySceneGraph.AddToRoot<VictoryOverlayModel>(
            victoryModel.GetRectData with
            {
                XTranslate = 0,
                YTranslate = 0,
                MaxWidthPercentage = 0.33f,
                MaxHeightPercentage = 0.33f,
            }
        );
        Update();
    }

    protected override void Update()
    {
        if (_victoryId != null) OverlaySceneGraph.UpdateInstance(_victoryId);
    }

    public override void Destroy()
    {
        if (_victoryId is not null)
        {
            OverlaySceneGraph.Delete(_victoryId);
            _victoryId = null;
        }
    }
}
