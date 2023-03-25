using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.UserInteractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal class ScytheOverlayManager : GamePhaseOverlayManager
{
    private InstanceHandle? _arrowContainer;
    private InstanceHandle? _cardId;
    private readonly List<InstanceHandle> _arrowIds = new();

    public ScytheOverlayManager(
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

        var slot = gameState.MechStates[mechPlayActionsPhase.MechIndex].CommandLine.SlotAt(mechPlayActionsPhase.SlotToPlay)!;
        var card = (Card?)slot.Peek();
        
        if (card is null or not ScytheCard) throw new InvalidOperationException();

        var level = slot.Count();

        _cardId = OverlaySceneGraph.AddToRoot<ScytheCardModel>(
            CardModel.GetRectData(card, TexturesStore) with
            {
                XTranslate = 0.99f,
                YTranslate = 0.99f,
                MaxWidthPercentage = 1f,
                MaxHeightPercentage = 0.33f,
            }
        );

        _arrowContainer = OverlaySceneGraph.AddToRoot<TransparentOverlayModel>(new OverlaySceneGraphRectangle(
            MaxWidthPercentage: 0.5f,
            MaxHeightPercentage: 0.5f,
            XTranslate: 0.0f,
            YTranslate: 0.0f,
            HwRatio: 1.0f
        ));

        var leftId = CreateAndAddLeftArrow(_arrowContainer);
        _arrowIds.Add(leftId);
        UserInteractionLookup.Add(leftId, new RotationSelected(Rotation.Minus90Degrees));

        var rightId = CreateAndAddRightArrow(_arrowContainer);
        _arrowIds.Add(rightId);
        UserInteractionLookup.Add(rightId, new RotationSelected(Rotation.Plus90Degrees));

        if (level >= 2)
        {
            var downId = CreateAndAddDownArrow(_arrowContainer);
            _arrowIds.Add(downId);
            UserInteractionLookup.Add(downId, new RotationSelected(Rotation.Plus180Degrees));
        }

        if (level >= 3)
        {
            var topId = CreateAndAddTopArrow(_arrowContainer);
            _arrowIds.Add(topId);
            UserInteractionLookup.Add(topId, new RotationSelected(Rotation.NoRotation));
        }

        Update();
    }

    private InstanceHandle CreateAndAddLeftArrow(InstanceHandle parentHandle)
    {
        return OverlaySceneGraph.Add<LeftArrowModel>(
            GraphicsModelProvider.GetRenderModel<LeftArrowModel, OverlayInstance>().GetRectData with
            {
                MaxWidthPercentage = 0.3f,
                MaxHeightPercentage = 0.3f,
                XTranslate = -1,
                YTranslate = 0,
            },
            parentHandle
        );
    }

    private InstanceHandle CreateAndAddRightArrow(InstanceHandle parentHandle)
    {
        return OverlaySceneGraph.Add<RightArrowModel>(
            GraphicsModelProvider.GetRenderModel<RightArrowModel, OverlayInstance>().GetRectData with
            {
                MaxWidthPercentage = 0.3f,
                MaxHeightPercentage = 0.3f,
                XTranslate = 1,
                YTranslate = 0,
            },
            parentHandle
        );
    }

    private InstanceHandle CreateAndAddDownArrow(InstanceHandle parentHandle)
    {
        return OverlaySceneGraph.Add<DownArrowModel>(
            GraphicsModelProvider.GetRenderModel<DownArrowModel, OverlayInstance>().GetRectData with
            {
                MaxWidthPercentage = 0.3f,
                MaxHeightPercentage = 0.3f,
                XTranslate = 0,
                YTranslate = -1,
            },
            parentHandle
        );
    }

    private InstanceHandle CreateAndAddTopArrow(InstanceHandle parentHandle)
    {
        return OverlaySceneGraph.Add<TopArrowModel>(
            GraphicsModelProvider.GetRenderModel<TopArrowModel, OverlayInstance>().GetRectData with
            {
                MaxWidthPercentage = 0.3f,
                MaxHeightPercentage = 0.3f,
                XTranslate = 0,
                YTranslate = 1,
            },
            parentHandle
        );
    }

    protected override void Update()
    {
        if (_cardId != null) OverlaySceneGraph.UpdateInstance(_cardId);
        if (_arrowContainer != null) OverlaySceneGraph.UpdateInstance(_arrowContainer);
        foreach (var id in _arrowIds) OverlaySceneGraph.UpdateInstance(id);
    }

    public override void Destroy()
    {
        if (_cardId is not null)
        {
            OverlaySceneGraph.Delete(_cardId);
            _cardId = null;
        }
        if (_arrowContainer is not null)
        {
            OverlaySceneGraph.Delete(_arrowContainer);
            _arrowContainer = null;
        }
        foreach (var id in _arrowIds)
        {
            OverlaySceneGraph.Delete(id);
        }
        _arrowIds.Clear();
    }
}
