using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.UserInteractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal class ConfirmCardOverlayManager : GamePhaseOverlayManager
{
    private InstanceHandle? _cardId;

    public ConfirmCardOverlayManager(
        float screenHwRatio, 
        IGraphicsModelProvider graphicsModelProvider, 
        ITexturesStore texturesStore, 
        IUserInteractionLookup userInteractionLookup
    ) : base(
        screenHwRatio,
        graphicsModelProvider,
        texturesStore,
        userInteractionLookup
    ) { }

    public override void Init(GameState? gameState)
    {
        if (gameState?.GamePhase.GetType() != typeof(MechPlayActionsPhase)) return;
        var mechPlayActionsPhase = (gameState.GamePhase as MechPlayActionsPhase)!;

        var card = gameState.MechStates[mechPlayActionsPhase.MechIndex].CommandLine.TopCardAt(mechPlayActionsPhase.SlotToPlay);
        if (card == null) return;
        var cardType = CardModel.GetCardModelType(card);

        _cardId = OverlaySceneGraph.AddToRoot(
            CardModel.GetRectData(card, TexturesStore) with
            {
                XTranslate = 1.0f,
                YTranslate = 1.0f,
                MaxWidthPercentage = 1f,
                MaxHeightPercentage = 0.33f,
            },
            cardType
        );

        UserInteractionLookup.Add(_cardId, new ConfirmAction());

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
