using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_GameLogic;
using System.Collections.Immutable;
using System.Drawing;

namespace Mechs_Vs_Minions_GameLogic_Tests.MovementTests;

public class MechMovementTests
{
    public MechMovementTests()
    {
        AssertionOptions.AssertEquivalencyUsing(o => o.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void MechFacingNorth_MoveForward1_MechMovesNorth1()
    {
        var mech = CreateMechAt(new Point(0, 0), Orientation.North);
        var gameState = GameStateWith(new[] { mech });
        var expectedMechState = mech with { Position = new Point(0, 1) };

        var (newMechState, stomedMinions, _) = Movement.MoveMech(gameState, mech, Direction.Forward, 1);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        stomedMinions.Should().BeEmpty();
    }

    [Fact]
    public void MechFacingEast_MoveForward1_MechMovesEast1()
    {
        var mech = CreateMechAt(new Point(0, 0), Orientation.East);
        var gameState = GameStateWith(new[] { mech });
        var expectedMechState = mech with { Position = new Point(1, 0) };

        var (newMechState, stomedMinions, _) = Movement.MoveMech(gameState, mech, Direction.Forward, 1);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        stomedMinions.Should().BeEmpty();
    }

    [Fact]
    public void MechFacingEast_MoveLeft1_MechMovesNorth1()
    {
        var mech = CreateMechAt(new Point(0, 0), Orientation.East);
        var gameState = GameStateWith(new[] { mech });
        var expectedMechState = mech with { Position = new Point(0, 1) };

        var (newMechState, stomedMinions, _) = Movement.MoveMech(gameState, mech, Direction.Left, 1);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        stomedMinions.Should().BeEmpty();
    }

    [Fact]
    public void MechFacingBorder_MoveOverBorder_MechStaysBeforeBorder()
    {
        var mech = CreateMechAt(new Point(0, 1), Orientation.East);
        var gameState = GameStateWith(new[] { mech });
        var expectedMechState = mech with { Position = new Point(0, 0) };

        var (newMechState, stomedMinions, _) = Movement.MoveMech(gameState, mech, Direction.Right, 2);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        stomedMinions.Should().BeEmpty();
    }

    [Fact]
    public void Mech_MoveOverSeveralMinions_MechMovedAndMinionsMarkedForDeath()
    {
        var mech = CreateMechAt(new Point(0, 0), Orientation.East);
        var minion1 = MinionState.CreateNew(new Point(1, 0), Orientation.South);
        var minion2 = MinionState.CreateNew(new Point(3, 0), Orientation.South);
        var minion3 = MinionState.CreateNew(new Point(4, 0), Orientation.South);
        var gameState = GameStateWith(new[] { mech }, new[] { minion1, minion2, minion3 });
        var expectedMechState = mech with { Position = new Point(4, 0) };

        var (newMechState, stomedMinions, _) = Movement.MoveMech(gameState, mech, Direction.Forward, 4);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        stomedMinions.Should().BeEquivalentTo(new[] { minion1, minion2, minion3 });
    }

    [Fact]
    public void Mech_MoveNotOverMinion_MechMovedAndMinionStaysAlive()
    {
        var mech = CreateMechAt(new Point(0, 0), Orientation.East);
        var minion = MinionState.CreateNew(new Point(0, 1), Orientation.South);
        var gameState = GameStateWith(new[] { mech }, new[] { minion });
        var expectedMechState = mech with { Position = new Point(4, 0) };

        var (newMechState, stomedMinions, _) = Movement.MoveMech(gameState, mech, Direction.Forward, 4);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        stomedMinions.Should().BeEmpty();
    }

    [Fact]
    public void MechBehindOther_MoveForwardTwo_MechMovedAndOtherMechPushed()
    {
        var mech = CreateMechAt(new Point(1, 1), Orientation.North);
        var mech2 = CreateMechAt(new Point(1, 2), Orientation.East);
        var gameState = GameStateWith(new[] { mech, mech2 });
        var expectedMechState = mech with { Position = new Point(1, 3) };
        var expectedMech2State = mech2 with { Position = new Point(1, 4) };

        var (newMechState, _, movedOtherMechs) = Movement.MoveMech(gameState, mech, Direction.Forward, 2);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        movedOtherMechs.Values.Should().BeEquivalentTo(new[]{expectedMech2State});
    }

    [Fact]
    public void MechBehind2Other_MoveForward_MechMovedAndOtherMechsPushed()
    {
        var mech = CreateMechAt(new Point(1, 1), Orientation.North);
        var mech2 = CreateMechAt(new Point(1, 3), Orientation.East);
        var mech3 = CreateMechAt(new Point(1, 6), Orientation.South);
        var gameState = GameStateWith(new[] { mech, mech2, mech3 });
        var expectedMechState = mech with { Position = new Point(1, 5) };
        var expectedMech2State = mech2 with { Position = new Point(1, 6) };
        var expectedMech3State = mech3 with { Position = new Point(1, 7) };

        var (newMechState, _, movedOtherMechs) = Movement.MoveMech(gameState, mech, Direction.Forward, 10);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        movedOtherMechs.Values.Should().BeEquivalentTo(new[]{expectedMech2State, expectedMech3State});
    }

    [Fact]
    public void MechBehindOtherAndMinions_MoveForward_MechMovedAndOtherMechsPushedAndMinionsKilled()
    {
        var mech = CreateMechAt(new Point(1, 1), Orientation.North);
        var mech2 = CreateMechAt(new Point(3, 1), Orientation.West);
        var minion = MinionState.CreateNew(new Point(2, 1), Orientation.South);
        var minion2 = MinionState.CreateNew(new Point(5, 1), Orientation.South);
        var gameState = GameStateWith(new[] { mech, mech2 }, new []{ minion, minion2 });
        var expectedMechState = mech with { Position = new Point(4, 1) };
        var expectedMech2State = mech2 with { Position = new Point(5, 1) };

        var (newMechState, killedMinions, movedOtherMechs) = Movement.MoveMech(gameState, mech, Direction.Right, 3);

        newMechState.Should().BeEquivalentTo(expectedMechState);
        killedMinions.Should().BeEquivalentTo(new []{ minion, minion2 });
        movedOtherMechs.Values.Should().BeEquivalentTo(new[]{expectedMech2State});
    }

    private static MechState CreateMechAt(Point position, Orientation orientation)
        => new(Guid.NewGuid(), position, orientation, CommandLine.Empty);

    private static GameState GameStateWith(IEnumerable<MechState> mechs, IEnumerable<MinionState> minions) =>
        GameState.Initial with
        {
            MechStates = mechs.ToImmutableList(),
            MinionStates = minions.ToImmutableList()
        };

    private static GameState GameStateWith(IEnumerable<MechState> mechs)
        => GameStateWith(mechs, Enumerable.Empty<MinionState>());
}
