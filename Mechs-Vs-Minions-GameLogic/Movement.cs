using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.Utilities;
using System.Collections.Immutable;
using System.Drawing;

namespace Mechs_Vs_Minions_GameLogic;

internal static class Movement
{
    internal sealed record MechMovementResult(MechState MovedMech, IImmutableList<MinionState> StompedMinions,
        IImmutableDictionary<Guid,MechState> MovedOtherMechs)
    {
        public static MechMovementResult EmptyFor(MechState movedMech) => 
            new (movedMech, ImmutableList.Create<MinionState>(), ImmutableDictionary.Create<Guid, MechState>());
    };

    internal static MechMovementResult MoveMech(GameState gameState, MechState mech, Direction direction, int steps)
        => MoveMech(gameState, MechMovementResult.EmptyFor(mech), direction, steps);

    private static MechMovementResult MoveMech(GameState gameState, MechMovementResult carry, Direction direction, int steps)
        => MoveMech(gameState, carry, carry.MovedMech.Orientation, direction, steps);

    private static MechMovementResult MoveMech(GameState gameState, MechMovementResult carry,  Orientation baseOrientation, Direction direction, int steps)
    {
        const int singleStep = 1;
        var newPosition = carry.MovedMech.Position.GetNewPositionFrom(baseOrientation, direction, singleStep);

        var collidedMech = MechAtLocation(gameState, newPosition);
        MechMovementResult? pushMovement = null;

        if (collidedMech is not null)
        {
            pushMovement = MoveMech(gameState, MechMovementResult.EmptyFor(collidedMech), baseOrientation, direction, singleStep);

            if (NotMovedAfter(collidedMech, pushMovement))
            {
                return carry;
            }
        }

        if (!gameState.Board.IsPassable(newPosition))
        {
            return carry;
        }

        var newlyStompedMinions = gameState.MinionStates.Where(minion => minion.Position == newPosition);
        var newCarry = carry with
        {
            MovedMech = carry.MovedMech with { Position = newPosition },
            StompedMinions = carry.StompedMinions.AddRange(newlyStompedMinions)
        };

        if (pushMovement is not null)
        {
            newCarry = UpdateCarryFrom(newCarry, pushMovement);
        }

        var newGameState = ApplyMovement(gameState, newCarry); 

        return steps > 1 ? MoveMech(newGameState, newCarry, direction, steps - 1) : newCarry;
    }

    private static MechMovementResult UpdateCarryFrom(MechMovementResult newCarry, MechMovementResult pushMovement)
    {
        return newCarry with
        {
            StompedMinions = newCarry.StompedMinions.
                AddRange(pushMovement.StompedMinions),
            MovedOtherMechs = newCarry.MovedOtherMechs.
                SetItems(pushMovement.MovedOtherMechs).
                SetItem(pushMovement.MovedMech.Id, pushMovement.MovedMech),
        };
    }

    private static bool NotMovedAfter(MechState collidedMech, MechMovementResult transitiveMovement)
    {
        return transitiveMovement.MovedMech.Position == collidedMech.Position;
    }

    internal static MinionState MoveMinion(GameState gameState, MinionState minionState, Orientation orientation, int steps = 1)
    {
        var newPosition = minionState.Position.GetNewPositionFrom(orientation, Direction.Forward, 1);

        if (IsMechAtLocation(gameState, newPosition))
        {
            return minionState;
        }

        if (IsMinionAtLocation(gameState, newPosition))
        {
            return minionState;
        }

        if (!gameState.Board.IsPassable(newPosition))
        {
            return minionState;
        }

        var updatedMinionState = minionState with { Position = newPosition };

        if (steps > 1)
        {
            return MoveMinion(gameState, updatedMinionState, orientation, steps - 1);
        }

        return updatedMinionState;
    }

    internal static MechState TurnMech(MechState mechState, Rotation rotation)
        => mechState with
        {
            Orientation = mechState.Orientation.Turn(rotation)
        };

    public static bool IsMechAtLocation(GameState gameState, Point location)
        => gameState.MechStates.Any(mech => mech.Position == location);

    public static bool IsMinionAtLocation(GameState gameState, Point location)
        => gameState.MinionStates.Any(minion => minion.Position == location);

    public static MinionState? MinionAtLocation(GameState gameState, Point location)
        => gameState.MinionStates.SingleOrDefault(minion => minion.Position == location);

    public static MechState? MechAtLocation(GameState gameState, Point location)
        => gameState.MechStates.SingleOrDefault(mech => mech.Position == location);

    public static GameState ApplyMovement(GameState gameState, MechMovementResult mechMovementResult)
    {
        return GameStateTransformer.KillMinions(gameState, mechMovementResult.StompedMinions) with
        {
            MechStates = Update(gameState.MechStates, mechMovementResult.MovedOtherMechs.Values.Append(mechMovementResult.MovedMech).ToImmutableList()),
        };
    }

    private static IImmutableList<MechState> Update(IImmutableList<MechState> oldMechStates,
        IImmutableList<MechState> changes)
    {
        return changes.Aggregate(oldMechStates, (current, changedState) => current.Replace(changedState, changedState, new SameId()));
    }

    private class SameId : EqualityComparer<MechState>
    {
        public override bool Equals(MechState? x, MechState? y)
            => x is not null && x.Id.Equals(y?.Id);

        public override int GetHashCode(MechState obj)
            => obj.Id.GetHashCode();
    }

}
