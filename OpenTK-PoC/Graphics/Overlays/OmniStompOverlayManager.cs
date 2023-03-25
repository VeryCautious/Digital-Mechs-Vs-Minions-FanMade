using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.UserInteractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal class OmniStompOverlayManager : GamePhaseOverlayManager
{
    private InstanceHandle? _cardId;

    public OmniStompOverlayManager(
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
        if (gameState?.GamePhase is not MechPlayActionsPhase mechPlayActionsPhase) throw new InvalidOperationException();

        var card = gameState.MechStates[mechPlayActionsPhase.MechIndex].CommandLine.TopCardAt(mechPlayActionsPhase.SlotToPlay);
        
        if (card is null or not OmniStompCard) throw new InvalidOperationException();

        _cardId = OverlaySceneGraph.AddToRoot<OmnistompCardModel>(
            CardModel.GetRectData(card, TexturesStore) with
            {
                XTranslate = 0.99f,
                YTranslate = 0.99f,
                MaxWidthPercentage = 1f,
                MaxHeightPercentage = 0.33f,
            }
        );

        Update();
    }

    protected override void Update()
    {
        if (_cardId != null) OverlaySceneGraph.UpdateInstance(_cardId);
    }

    public override void Destroy()
    {
        if (_cardId is not null)
        {
            OverlaySceneGraph.Delete(_cardId);
            _cardId = null;
        }
    }
}
