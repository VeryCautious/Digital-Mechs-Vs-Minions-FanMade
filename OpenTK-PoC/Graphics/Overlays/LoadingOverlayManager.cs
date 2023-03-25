using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.UserInteractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal class LoadingOverlayManager : GamePhaseOverlayManager
{
    private InstanceHandle? _loadingId;

    public LoadingOverlayManager(
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

        var victoryModel = GraphicsModelProvider.GetRenderModel<LoadingScreenModel, OverlayInstance>();
        _loadingId = OverlaySceneGraph.AddToRoot<LoadingScreenModel>(
            victoryModel.GetRectData with
            {
                XTranslate = 0,
                YTranslate = 0,
                MaxWidthPercentage = 1f,
                MaxHeightPercentage = 1f,
            }
        );
        Update();
    }

    protected override void Update()
    {
        if (_loadingId != null) OverlaySceneGraph.UpdateInstance(_loadingId);
    }

    public override void Destroy()
    {
        if (_loadingId is not null)
        {
            OverlaySceneGraph.Delete(_loadingId);
            _loadingId = null;
        }
    }
}