using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.UserInteractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal class DrawCardPhaseOverlayManager : GamePhaseOverlayManager
{
    private readonly List<InstanceHandle> _cardIds = new();
    private readonly List<InstanceHandle> _slotIds = new();
    private InstanceHandle? _commandlineId;
    private readonly Dictionary<(int, int), InstanceHandle> _commandlineCardIds = new();

    private bool _cardSelected;

    public DrawCardPhaseOverlayManager(
       float screenHwRatio,
       IGraphicsModelProvider graphicsModelProvider,
       ITexturesStore texturesStore,
       IUserInteractionLookup userInteractionLookup
    ) : base(
        screenHwRatio,
        graphicsModelProvider,
        texturesStore,
        userInteractionLookup
    ) { 
        _cardSelected = false;
    }

    public override void Init(GameState? gameState)
    {
        if (gameState?.GamePhase.GetType() != typeof(DrawCardPhase)) return;
        var drawCardPhase = (gameState.GamePhase as DrawCardPhase)!;

        InitDrawableCards(drawCardPhase);
        InitCommandLine();
        InitCommandlineCards(drawCardPhase, gameState);
        
        Update();
    }

    #region Init
    private void InitDrawableCards(DrawCardPhase drawCardPhase)
    {
        for (var i = 0; i < drawCardPhase.DrawableCards.Count; i++)
        {
            var card = drawCardPhase.DrawableCards[i];
            var cardType = CardModel.GetCardModelType(card);

            var id = OverlaySceneGraph.AddToRoot(
                CardModel.GetRectData(card, TexturesStore) with
                {
                    XTranslate = -1.0f + (i + 1) * 2.0f / (drawCardPhase.DrawableCards.Count + 1),
                    YTranslate = 0.9f,
                    MaxWidthPercentage = 0.15f,
                    MaxHeightPercentage = 0.3f,
                },
                cardType
            );

            UserInteractionLookup.Add(id, new DrawableCardSelected(i));

            _cardIds.Add(id);
        }
    }

    private void InitCommandLine()
    {
        var cmdLineModel = GraphicsModelProvider.GetRenderModel<CommandLineModel, OverlayInstance>();
        _commandlineId = OverlaySceneGraph.AddToRoot<CommandLineModel>(
            cmdLineModel.GetRectData with
            {
                XTranslate = 0.0f,
                YTranslate = -1.0f,
                MaxWidthPercentage = 1f,
                MaxHeightPercentage = 0.3f,
            }
        );

        for (var slot = 0; slot < CommandLine.SlotCount; slot++)
        {
            var slotHandle = OverlaySceneGraph.Add<TransparentOverlayModel>(
                cmdLineModel.GetCardPosition(new BlazeCard(), slot + 1, 0),
                _commandlineId!
            );
            OverlaySceneGraph.MoveZIndex(slotHandle, float.MinValue);
            UserInteractionLookup.Add(slotHandle, new CommandLineSlotSelected(slot));
            _slotIds.Add(slotHandle);
        }
    }

    private void InitCommandlineCards(DrawCardPhase drawCardPhase, GameState gameState)
    {
        var cmdLine = gameState.MechStates[drawCardPhase.MechIndex].CommandLine;
        var cmdLineModel = GraphicsModelProvider.GetRenderModel<CommandLineModel, OverlayInstance>();
        foreach (var (slot, stack) in cmdLine.Cards)
        {
            var cards = stack.Reverse().ToArray();
            for (var level = 1; level <= Math.Min(3, cards.Length); level++)
            {
                var card = cards[level - 1];
                var cardModel = CardModel.GetCardModelType(card);
                
                var id = OverlaySceneGraph.Add(
                    cmdLineModel.GetCardPosition(card, slot + 1, level),
                    parentHandle: _commandlineId!,
                    cardModel);

                _commandlineCardIds.Add((slot + 1, level), id);
                OverlaySceneGraph.MoveZIndex(id, -0.01f * level);
            }
        }
    }
    #endregion

    public override void Notify(UserInteraction interaction)
    {
        if (interaction is not DrawableCardSelected cardSelectedInteraction) return;

        if (_cardSelected) return;
        _cardSelected = true;

        var cardModelId = _cardIds[cardSelectedInteraction.CardIndex];
        OverlaySceneGraph.UpdateRectData(
            cardModelId,
            rectData => rectData with { 
                YTranslate = 0.85f, 
                MaxHeightPercentage = rectData.MaxHeightPercentage + 0.02f,
                MaxWidthPercentage = rectData.MaxWidthPercentage + 0.02f,
            }
        );
    }

    protected override void Update()
    {
        foreach (var instance in _cardIds)
        {
            OverlaySceneGraph.UpdateInstance(instance);
        }

        if (_commandlineId is not null)
        {
            OverlaySceneGraph.UpdateInstance(_commandlineId);
        }

        foreach (var cmdLineCardId in _commandlineCardIds.Values)
        {
            OverlaySceneGraph.UpdateInstance(cmdLineCardId);
        }
    }

    public override void Destroy()
    {
        foreach (var handle in _cardIds.Union(_slotIds))
        {
            OverlaySceneGraph.Delete(handle);
            UserInteractionLookup.Remove(handle);
        }
        _cardIds.Clear();
        _slotIds.Clear();

        if (_commandlineId is not null)
        {
            OverlaySceneGraph.Delete(_commandlineId);
            _commandlineId = null;
        }
        foreach (var cardId in _commandlineCardIds.Values)
        {
            OverlaySceneGraph.Delete(cardId);
        }
        _commandlineCardIds.Clear();
    }
}
