using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Utilities;

namespace Mechs_Vs_Minions_GameLogic;

public class ActionPrompter : IActionPrompter<GameState>
{
    public async Task<StateAction> GetActionFrom(GameState gameState, IController controller)
    {
        var cts = new CancellationTokenSource();
        var checkForMetaGameInteractions = HandleMetaGameRequests(controller, cts.Token);

        var getNextStateActionTask = TryGetNextStateAction(gameState, controller);

        var finishedTask = await Task.WhenAny(checkForMetaGameInteractions, getNextStateActionTask);

        if (!getNextStateActionTask.IsCompletedSuccessfully)
            finishedTask = checkForMetaGameInteractions;

        cts.Cancel();

        return await finishedTask;
    }

    private Task<StateAction> TryGetNextStateAction(GameState gameState, IController controller)
    {
        return gameState.GamePhase switch
        {
            DrawCardPhase drawCardPhase => GetDrawCardActionFrom(gameState, drawCardPhase, controller),
            MechPlayActionsPhase playCardPhase => GetPlayCardAction(gameState, playCardPhase, controller),
            MinionMovePhase => GetMinionMoveAction(),
            QueueDamageToMechsPhase => GetQueueDamageToMechsAction(),
            ApplyDamageToMechsPhase => GetApplyDamageToMechsAction(gameState),
            PickDrawableCardsPhase => GetDrawableCardsAction(gameState),
            EndOfGamePhase => GetEndOfGameAction(controller),
            _ => throw new NotImplementedException()
        };
    }

    private static async Task<StateAction> HandleMetaGameRequests(IController controller, CancellationToken cancellationToken)
    {
        return GetActionFrom(await controller.DequeueMetaGameInteraction(cancellationToken));
    }

    private static StateAction GetActionFrom(MetaGameRequest metaGameRequest)
    {
        return metaGameRequest switch
        {
            ResetGameRequest => new ResetGameAction(),
            _ => throw new NotImplementedException(metaGameRequest.GetType().Name)
        };
    }

    private static Task<StateAction> GetDrawableCardsAction(GameState gameState)
    {
        return Task.FromResult<StateAction>(new PickDrawableCardsAction(gameState.MechStates.Count));
    }

    private static Task<StateAction> GetQueueDamageToMechsAction()
    {
        return Task.FromResult<StateAction>(new QueueDamageToMechsAction());
    }

    private static Task<StateAction> GetApplyDamageToMechsAction(GameState gameState)
    {
        var (mech, damageList) = gameState.ToBeAppliedDamageCards.First();
        var mechIndex = gameState.MechStates.IndexOf(gameState.MechStates.Single(otherMech => mech.Id.Equals(otherMech.Id)));
        var nextDamage = damageList[0];
        var rnd = new Random();

        //TODO show what kind of damage is applied and let the user confirm

        StateAction action = nextDamage switch
        {
            GlitchCard glitch => new ApplyGlitchDamageToMechAction(mechIndex, glitch),
            StuckControlsCard stuckControls => new ApplyStuckControlsDamageToMechAction(mechIndex, rnd.Next(CommandLine.SlotCount), stuckControls),
            _ => throw new NotImplementedException()
        };

        Console.WriteLine($"{nextDamage.GetType().Name} was applied");

        return Task.FromResult(action);
    }

    private static Task<StateAction> GetMinionMoveAction()
    {
        var r = new Random();
        var directions = new[]
        {
            Orientation.North,
            Orientation.South,
            Orientation.East,
            Orientation.West
        };
        return Task.FromResult<StateAction>(new MoveMinionsAction(directions[r.Next(0,3)]));
    }

    private Task<StateAction> GetPlayCardAction(GameState gameState, MechPlayActionsPhase playCardPhase, IController controller)
    {
        var mechCards = gameState.MechStates[playCardPhase.MechIndex].CommandLine.Cards;

        if (!mechCards.ContainsKey(playCardPhase.SlotToPlay))
        {
            return Task.FromResult<StateAction>(new UseEmptySlotAction(playCardPhase.MechIndex));
        }

        var cardToPlay = mechCards[playCardPhase.SlotToPlay].Peek();
        var level = Math.Min(3, mechCards[playCardPhase.SlotToPlay].Count());
        
        return cardToPlay switch
        {
            OmniStompCard => UseOmniStomp(gameState, controller, playCardPhase.MechIndex, level),
            BlazeCard => UseBlazeCard(gameState, controller, playCardPhase.MechIndex, level),
            ScytheCard => UseScytheCard(controller, playCardPhase.MechIndex, level),
            RipsawCard => UseRipsawCard(controller, playCardPhase.MechIndex, level),
            SpeedCard => UseSpeedCard(gameState, controller, playCardPhase.MechIndex, level),
            AimBotCard => UseAimBotCard(gameState, controller, playCardPhase.MechIndex, level),
            StuckControlsCard stuckControls => UseStuckControls(controller, playCardPhase.MechIndex, stuckControls),
            _ => throw new NotImplementedException()
        };
    }

    private static async Task<StateAction> UseStuckControls(IController controller, int mechIndex, StuckControlsCard stuckControlsCard)
    {
        await controller.Prompt(new Confirm("Confirm stuck controls"));

        return new UseStuckControlsAction(mechIndex, stuckControlsCard.Rotation);
    }

    private static async Task<StateAction> UseScytheCard(IController controller, int mechIndex, int level)
    {
        var rotations = new List<Rotation>
        {
            Rotation.Plus90Degrees,
            Rotation.Minus90Degrees,
            Rotation.Plus180Degrees,
            Rotation.NoRotation
        };

        var rotationResult = await controller.Prompt(new SelectRotationFrom(rotations.Take(level+1).ToImmutableList()));

        return new UseScytheAction(mechIndex, rotationResult.Rotation, level);
    }

    private static async Task<StateAction> UseRipsawCard(IController controller, int mechIndex, int level)
    {
        await controller.Prompt(new Confirm("Ripsaw PlayableCard"));
        return new UseRipsawAction(mechIndex, level);
    }

    private static async Task<StateAction> UseBlazeCard(GameState gameState, IController controller, int mechIndex, int level)
    {
        var mechState = gameState.MechStates[mechIndex];
        var endPosition = Movement.MoveMech(gameState, mechState, Direction.Forward, level).MovedMech.Position;
        await controller.Prompt(new SelectFieldFrom(new List<Point> { endPosition }.ToImmutableList()));
        return new UseBlazeAction(mechIndex, level);
    }

    private static async Task<StateAction> UseSpeedCard(GameState gameState, IController controller, int mechIndex, int level)
    {
        var mechState = gameState.MechStates[mechIndex];
        var possibleMovements = Enumerable.Range(level, level+1)
            .Select(steps => (
                Movement.MoveMech(gameState, mechState, Direction.Forward, steps).MovedMech.Position,
                NeededSteps: steps
            ))
            .GroupBy(posStepPair => posStepPair.Position)
            .Select(e => e.First())
            .ToDictionary(
                keySelector: posStepPair => posStepPair.Position,
                elementSelector: posStepPair => posStepPair.NeededSteps
            );

        var fieldSelectionResult = await controller.Prompt(new SelectFieldFrom(possibleMovements.Keys.ToImmutableList()));

        return new UseSpeedAction(mechIndex, possibleMovements[fieldSelectionResult.SelectedField]);
    }

    private static async Task<StateAction> UseOmniStomp(GameState gameState, IController controller, int mechIndex, int level)
    {
        var mechState = gameState.MechStates[mechIndex];

        var leftPosition = Movement.MoveMech(gameState, mechState, Direction.Left, level).MovedMech.Position;
        var forwardPosition = Movement.MoveMech(gameState, mechState, Direction.Forward, level).MovedMech.Position;
        var rightPosition = Movement.MoveMech(gameState, mechState, Direction.Right, level).MovedMech.Position;

        var directions = new Dictionary<Point, Direction>();
        directions[leftPosition] = Direction.Left;
        directions[forwardPosition] = Direction.Forward;
        directions[rightPosition] = Direction.Right;

        var list = new List<Point>()
        {
            leftPosition, forwardPosition, rightPosition
        }.ToImmutableList();

        var fieldSelectionResult = await controller.Prompt(new SelectFieldFrom(list));
        
        return new UseOmniStompAction(mechIndex, directions[fieldSelectionResult.SelectedField], level);
    }

    private static async Task<StateAction> UseAimBotCard(GameState gameState, IController controller, int mechIndex, int level)
    {
        var mechState = gameState.MechStates[mechIndex];

        var targetableFields = Enumerable.Range(0, level).
            Aggregate(
                Enumerable.Empty<Point>().Append(mechState.Position),
                (targetFields,_) => targetFields.SelectMany(field => field.PointsAround())).
            Distinct().
            ToImmutableList();

        var fieldSelectionResult = await controller.Prompt(new SelectFieldFrom(targetableFields));
        
        return new UseAimBotAction(mechIndex, fieldSelectionResult.SelectedField);
    }

    private static async Task<StateAction> GetDrawCardActionFrom(GameState gameState, DrawCardPhase drawCardPhase, IController controller)
    {
        var cardResult = await controller.Prompt(new SelectCardFrom(drawCardPhase.DrawableCards));

        var commandLine = gameState.MechStates[drawCardPhase.MechIndex].CommandLine;
        var availableIndices = Enumerable
            .Range(0, CommandLine.SlotCount)
            .Where(index => 
                commandLine.TopCardAt(index) is null ||
                (commandLine.TopCardAt(index) is PlayableCard playableCard && playableCard.Color == cardResult.PlayableCard.Color));

        var slotResult = await controller.Prompt(new SelectCommandSlotFrom(availableIndices.ToImmutableList(), commandLine));

        return new DrawCardAction(drawCardPhase.MechIndex, cardResult.PlayableCard, slotResult.SlotIndex);
    }

    private static async Task<StateAction> GetEndOfGameAction(IController controller)
    {
        await controller.Prompt(new Confirm("Victory"));
        return new EndOfGameAction();
    }
}