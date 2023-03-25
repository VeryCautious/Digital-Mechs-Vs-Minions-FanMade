using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Utilities;

namespace Mechs_Vs_Minions_GameLogic;

public class GameStateTransformer : IGameStateTransformer<GameState>
{
    public (Transition transition, GameState newGameState) Apply(GameState gameState, StateAction action)
    {
        return action switch
        {
            UseOmniStompAction useOmniStompAction => ApplySpecific(gameState, useOmniStompAction),
            UseAimBotAction aimBotAction => ApplySpecific(gameState ,aimBotAction),
            DrawCardAction drawCardAction => ApplySpecific(gameState, drawCardAction),
            MoveMinionsAction moveMinionsAction => ApplySpecific(gameState, moveMinionsAction),
            UseBlazeAction useBlazeAction => ApplySpecific(gameState, useBlazeAction),
            UseEmptySlotAction noAction => ApplySpecific(gameState, noAction),
            UseScytheAction useScytheAction => ApplySpecific(gameState, useScytheAction),
            UseRipsawAction useRipsawAction => ApplySpecific(gameState, useRipsawAction),
            UseSpeedAction useSpeedAction => ApplySpecific(gameState, useSpeedAction),
            UseStuckControlsAction stuckControlsAction => ApplySpecific(gameState, stuckControlsAction),
            PickDrawableCardsAction pickDrawableCardsAction => ApplySpecific(gameState, pickDrawableCardsAction),
            QueueDamageToMechsAction damageMechsAction => ApplySpecific(gameState, damageMechsAction),
            ApplyGlitchDamageToMechAction applyGlitchDamageToMechAction => ApplySpecific(gameState, applyGlitchDamageToMechAction),
            ApplyStuckControlsDamageToMechAction applyStuckControlsDamageToMechAction => ApplySpecific(gameState, applyStuckControlsDamageToMechAction),
            EndOfGameAction endOfGameAction => ApplySpecific(gameState, endOfGameAction),
            ResetGameAction => (Transition.None, GameState.InitialRandom),
            _ => throw new NotImplementedException()
        };
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, DrawCardAction action)
    {
        if (gameState.GamePhase is not DrawCardPhase drawPhase || drawPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var mechState = gameState.MechStates[action.MechIndex];
        var newMechState = mechState with { CommandLine = mechState.CommandLine.PlaceCard(action.TargetCommandSlot, action.PlayableCard) };

        var nextGamePhase = GamePhaseTransitions.NextGamePhaseFrom(gameState, drawPhase, action.PlayableCard);
        var newGameState = gameState with
        {
            MechStates = gameState.MechStates.SetItem(action.MechIndex, newMechState),
            GamePhase = nextGamePhase
        };

        var transition = (nextGamePhase is MechPlayActionsPhase) ? new StartOfPlayActionPhaseTransition() : Transition.None;

        return (transition, newGameState);
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, MoveMinionsAction action)
    {
        if (gameState.GamePhase is not MinionMovePhase moveMinionPhase)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var changingState = gameState;
        foreach (var minion in OrderMinionsByFarthestInOrientation(gameState.MinionStates, action.Orientation)) {
            var indexOfMinion = changingState.MinionStates.IndexOf(minion, 0, changingState.MinionStates.Count, EqualityComparer<MinionState>.Default);
            changingState = changingState with { 
                MinionStates = changingState.MinionStates.SetItem(indexOfMinion, Movement.MoveMinion(changingState, minion, action.Orientation)) 
            };
        }

        var newGameState = changingState with
        {
            GamePhase = GamePhaseTransitions.NextGamePhaseFrom(moveMinionPhase)
        };

        Debug.Assert(gameState.MinionStates.Count == newGameState.MinionStates.Count);
        
        var oldStates = gameState.MinionStates.ToDictionary(minion => minion.Id);
        var newStates = newGameState.MinionStates.ToDictionary(minion => minion.Id);

        var minionMoveTransitions = gameState.MinionStates.Select(
            minion => new MoveTransition(minion.Id, oldStates[minion.Id].Position, newStates[minion.Id].Position)
        ).ToImmutableList();

        return (new MinionMoveTransition(minionMoveTransitions), newGameState);
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseEmptySlotAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        return (Transition.None, gameState.WithNextPhaseFrom(actionPhase));
    }

    internal static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, QueueDamageToMechsAction damageToMechsAction)
    {
        if (gameState.GamePhase is not QueueDamageToMechsPhase actionPhase)
        {
            throw new ArgumentException($"{damageToMechsAction.GetType().Name} can currently not be applied!");
        }
        
        int GetAmountOfAdjacentMinions(MechState mechState) => 
            mechState.Position.
            GetDirectlyAdjacent().
            Count(adjField => Movement.IsMinionAtLocation(gameState, adjField));

        var newGameState = gameState.MechStates.
            ToDictionary(mech => mech, GetAmountOfAdjacentMinions).
            Where(mechKvPair => mechKvPair.Value > 0).
            Aggregate(gameState, QueueDamageFor);
        
        return (Transition.None, newGameState.WithNextPhaseFrom(actionPhase));
    }

    private static GameState QueueDamageFor(GameState gameState, KeyValuePair<MechState,int> mechKeyValuePair) 
        => QueueDamageFor(gameState, mechKeyValuePair.Key, mechKeyValuePair.Value);

    private static GameState QueueDamageFor(GameState gameState, MechState mech, int damageAmount)
    {
        var nextDamageCards = gameState.DamageCards.Take(damageAmount).ToImmutableList();

        var newGameState = gameState with
        {
            ToBeAppliedDamageCards = gameState.ToBeAppliedDamageCards.Add(mech, nextDamageCards),
            DamageCards = gameState.DamageCards.Skip(damageAmount).ToImmutableList()
        };

        return newGameState;
    }

    internal static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, ApplyGlitchDamageToMechAction applyGlitchDamageToMechAction)
    {
        if (gameState.GamePhase is not ApplyDamageToMechsPhase actionPhase)
        {
            throw new ArgumentException($"{applyGlitchDamageToMechAction.GetType().Name} can currently not be applied!");
        }

        var oldMechState = gameState.MechStates[applyGlitchDamageToMechAction.MechIndex];
        var newMechState = oldMechState with
        {
            CommandLine = oldMechState.CommandLine.SwapSlots(applyGlitchDamageToMechAction.GlitchCard.Index1, applyGlitchDamageToMechAction.GlitchCard.Index2),
        };

        var newGameState = gameState with
        {
            MechStates = gameState.MechStates.SetItem(applyGlitchDamageToMechAction.MechIndex, newMechState),
            ToBeAppliedDamageCards = PopDamageCardFrom(gameState.ToBeAppliedDamageCards, applyGlitchDamageToMechAction.GlitchCard),
        };

        return (new ApplyGlitchDamageToMechTransition(oldMechState.Id), newGameState.WithNextPhaseFrom(actionPhase));
    }

    internal static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, ApplyStuckControlsDamageToMechAction applyStuckControlsAction)
    {
        if (gameState.GamePhase is not ApplyDamageToMechsPhase actionPhase)
        {
            throw new ArgumentException($"{applyStuckControlsAction.GetType().Name} can currently not be applied!");
        }

        var oldMechState = gameState.MechStates[applyStuckControlsAction.MechIndex];
        var newMechState = oldMechState with
        {
            CommandLine = oldMechState.CommandLine.PlaceCard(applyStuckControlsAction.TargetCommandSlot, applyStuckControlsAction.StuckControlsCard),
        };

        var newGameState = gameState with
        {
            MechStates = gameState.MechStates.SetItem(applyStuckControlsAction.MechIndex, newMechState),
            ToBeAppliedDamageCards = PopDamageCardFrom(gameState.ToBeAppliedDamageCards, applyStuckControlsAction.StuckControlsCard),
        };

        return (new ApplyStuckControlsToMechTransition(oldMechState.Id), newGameState.WithNextPhaseFrom(actionPhase));
    }

    private static IImmutableDictionary<MechState, IImmutableList<DamageCard>> PopDamageCardFrom(IImmutableDictionary<MechState, IImmutableList<DamageCard>> damageList, DamageCard expectedDamageCard)
    {
        var (mechState, mechsDamage) = damageList.First();

        if (!mechsDamage[0].Equals(expectedDamageCard))
        {
            throw new ArgumentException("The poppedCard should equal the expected card", nameof(expectedDamageCard));
        }

        if (mechsDamage.Count == 1)
        {
            return damageList.Skip(1).ToImmutableDictionary();
        }

        return damageList.SetItem(mechState, damageList[mechState].RemoveAt(0));
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, PickDrawableCardsAction action)
    {
        if (gameState.GamePhase is not PickDrawableCardsPhase actionPhase)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var newGameState = gameState with
        {
            CardDeck = gameState.CardDeck.Skip(action.Amount).ToImmutableList(),
            GamePhase = GamePhaseTransitions.NextGamePhaseFrom(gameState.CardDeck.Take(action.Amount).ToImmutableList(), actionPhase)
        };

        return (Transition.None, newGameState);
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseOmniStompAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        if (action.Direction == Direction.Backward) {
            throw new ArgumentException($"{action.GetType().Name} can not move backwards!");
        }

        var oldMechState = gameState.MechStates[action.MechIndex];
        var movementResult = Movement.MoveMech(gameState, oldMechState, action.Direction, action.Steps);

        var newGameState = Movement.ApplyMovement(gameState, movementResult);

        var transition = new UseOmniStompTransition(
            movementResult.MovedMech.Id,
            oldMechState.Position,
            movementResult.MovedMech.Position,
            PushesFrom(movementResult, gameState),
            movementResult.StompedMinions
        );
        return (transition, newGameState.WithNextPhaseFrom(actionPhase));
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseAimBotAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var mechState = gameState.MechStates[action.MechIndex];

        var killedMinion = Movement.MinionAtLocation(gameState, action.TargetField);
        var killedMinions =
            killedMinion is null ? ImmutableList<MinionState>.Empty : ImmutableList.Create(killedMinion);

        var newGameState = KillMinions(
            gameState,
            killedMinions
        );

        var transition = new UseAimBotTransition(
            mechState.Id,
            mechState.Position,
            action.TargetField,
            killedMinions
        );
        return (transition, newGameState.WithNextPhaseFrom(actionPhase));
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseSpeedAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var oldMechState = gameState.MechStates[action.MechIndex];
        var movementResult = Movement.MoveMech(gameState, oldMechState, Direction.Forward, action.Steps);

        var newGameState = Movement.ApplyMovement(gameState, movementResult);
        
        var transition = new UseSpeedTransition(
            movementResult.MovedMech.Id,
            oldMechState.Position,
            movementResult.MovedMech.Position,
            PushesFrom(movementResult, gameState),
            movementResult.StompedMinions
        );
        return (transition, newGameState.WithNextPhaseFrom(actionPhase));
    }

    internal static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseBlazeAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var oldMechState = gameState.MechStates[action.MechIndex];
        var movementResult = Movement.MoveMech(gameState, oldMechState, Direction.Forward, action.Steps);

        var burningPositions = new[]
        {
            movementResult.MovedMech.Position.GetNewPositionFrom(movementResult.MovedMech.Orientation, Direction.Left, 1),
            movementResult.MovedMech.Position.GetNewPositionFrom(movementResult.MovedMech.Orientation, Direction.Right, 1)
        };

        var burnedMinions = gameState.MinionStates.
            Except(movementResult.StompedMinions).
            Where(minion => burningPositions.Contains(minion.Position)).ToImmutableList();

        var newGameState = Movement.ApplyMovement(KillMinions(gameState, burnedMinions), movementResult);

        var transition = new UseBlazeTransition(
            movementResult.MovedMech.Id,
            oldMechState.Position,
            movementResult.MovedMech.Position,
            PushesFrom(movementResult, gameState),
            movementResult.StompedMinions,
            burnedMinions
            );
        return (transition, newGameState.WithNextPhaseFrom(actionPhase));
    }

    internal static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseScytheAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var oldMechState = gameState.MechStates[action.MechIndex];
        var newMechState = Movement.TurnMech(oldMechState, action.Rotation);

        var targetFields = newMechState.Position.PointsAround();
        var slicedMinions = gameState.MinionStates.
            Where(minion => targetFields.Contains(minion.Position)).
            Take(action.MaxMinionKills).
            ToImmutableList();

        var newGameState = KillMinions(gameState, slicedMinions) with
        {
            MechStates = gameState.MechStates.SetItem(action.MechIndex, newMechState)
        };

        var transition = new UseScytheTransition(
            oldMechState.Id,
            oldMechState.Orientation, 
            newMechState.Orientation,
            slicedMinions
        );
        return (transition, newGameState.WithNextPhaseFrom(actionPhase));
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseStuckControlsAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var oldMechState = gameState.MechStates[action.MechIndex];
        var newMechState = Movement.TurnMech(oldMechState, action.Rotation);

       
        var newGameState = gameState with
        {
            MechStates = gameState.MechStates.SetItem(action.MechIndex, newMechState)
        };

        var transition = new UseStuckControlsTransition(oldMechState.Id, oldMechState.Orientation, newMechState.Orientation);
        return (transition, newGameState.WithNextPhaseFrom(actionPhase));
    }

    internal static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, UseRipsawAction action)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase actionPhase || actionPhase.MechIndex != action.MechIndex)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }

        var mechState = gameState.MechStates[action.MechIndex];
        var ripsawPosition = mechState.Position.GetNewPositionFrom(mechState.Orientation, Direction.Forward, 1);
        var lastValidRipsawPosition = ripsawPosition;
        var killedMinions = new List<MinionState>();
        while (
            gameState.Board.IsPassable(ripsawPosition) && 
            killedMinions.Count < action.MaxMinionKills && 
            !Movement.IsMechAtLocation(gameState, ripsawPosition)
        ) {
            var minionState = Movement.MinionAtLocation(gameState, ripsawPosition);
            lastValidRipsawPosition = ripsawPosition;
            if (minionState is not null)
            {
                killedMinions.Add(minionState);
            }
            ripsawPosition = ripsawPosition.GetNewPositionFrom(mechState.Orientation, Direction.Forward, 1);
        }

        var newGameState = KillMinions(gameState, killedMinions.ToImmutableArray());

        var transition = new UseRipSawTransition(mechState.Id, mechState.Position, lastValidRipsawPosition, killedMinions.ToImmutableList());
        return (transition, newGameState.WithNextPhaseFrom(actionPhase));
    }

    private static (Transition transition, GameState newGameState) ApplySpecific(GameState gameState, EndOfGameAction action)
    {
        if (gameState.GamePhase is not EndOfGamePhase endOfGamePhase)
        {
            throw new ArgumentException($"{action.GetType().Name} can currently not be applied!");
        }
        var newGameState = gameState with
        {
            GamePhase = GamePhaseTransitions.NextGamePhaseFrom(endOfGamePhase)
        };
        return (Transition.None, newGameState);
    }

    internal static GameState KillMinions(GameState gameState, IImmutableList<MinionState> minionsMarkedForDeath)
    {
        var newMinionStates = gameState.MinionStates.Except(minionsMarkedForDeath).ToImmutableList();
        return gameState with
        {
            MinionStates = newMinionStates,
            KilledMinions = gameState.KilledMinions + gameState.MinionStates.Count - newMinionStates.Count
        };
    }

    private static IEnumerable<MinionState> OrderMinionsByFarthestInOrientation(IEnumerable<MinionState> minions, Orientation orientation) 
        => orientation switch
        {
            Orientation.North => minions.OrderByDescending(minion => minion.Position.Y),
            Orientation.South => minions.OrderBy(minion => minion.Position.Y),
            Orientation.West => minions.OrderBy(minion => minion.Position.X),
            Orientation.East => minions.OrderByDescending(minion => minion.Position.X),
            _ => throw new NotImplementedException()
        };

    private static IImmutableList<MoveTransition> PushesFrom(Movement.MechMovementResult mechMovementResult, GameState originalGameState)
    {
        return mechMovementResult.MovedOtherMechs.Values.
            Select(movedMech => 
                new MoveTransition(
                    movedMech.Id,
                    originalGameState.MechStates.Single(mech => mech.Id.Equals(movedMech.Id)).Position, 
                    movedMech.Position)
            ).
            ToImmutableList();
    }

}
