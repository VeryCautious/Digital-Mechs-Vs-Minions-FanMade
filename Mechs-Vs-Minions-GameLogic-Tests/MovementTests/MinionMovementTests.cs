using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_GameLogic;
using System.Collections.Immutable;
using System.Drawing;

namespace Mechs_Vs_Minions_GameLogic_Tests.MovementTests;

public class MinionMovementTests
{
    public MinionMovementTests()
    {
        AssertionOptions.AssertEquivalencyUsing(o => o.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void Minion_MoveNorth1_MinionMoved()
    {
        var minion = MinionState.CreateNew(new Point(0, 0), Orientation.South);
        var gameState = GameStateWith(new[] { minion });
        var expectedMinionState = minion with { Position = new Point(0, 1) };

        var newMinionState = Movement.MoveMinion(gameState, minion, Orientation.North);

        newMinionState.Should().BeEquivalentTo(expectedMinionState);
    }

    [Fact]
    public void Minion_MoveNorth3_MinionMoved3()
    {
        var minion = MinionState.CreateNew(new Point(0, 0), Orientation.South);
        var gameState = GameStateWith(new[] { minion });
        var expectedMinionState = minion with { Position = new Point(0, 3) };

        var newMinionState = Movement.MoveMinion(gameState, minion, Orientation.North, 3);

        newMinionState.Should().BeEquivalentTo(expectedMinionState);
    }

    [Fact]
    public void MinionAtBorder_MoveOverBorder_MinionStays()
    {
        var minion = MinionState.CreateNew(new Point(0, 0), Orientation.South);
        var gameState = GameStateWith(new[] { minion });
        var expectedMinionState = minion with { Position = new Point(0, 0) };

        var newMinionState = Movement.MoveMinion(gameState, minion, Orientation.South);

        newMinionState.Should().BeEquivalentTo(expectedMinionState);
    }

    [Fact]
    public void MinionOneAwayFromBorder_MoveOverBorder_MinionStaysAtBorder()
    {
        var minion = MinionState.CreateNew(new Point(0, 1), Orientation.South);
        var gameState = GameStateWith(new[] { minion });
        var expectedMinionState = minion with { Position = new Point(0, 0) };

        var newMinionState = Movement.MoveMinion(gameState, minion, Orientation.South, 2);

        newMinionState.Should().BeEquivalentTo(expectedMinionState);
    }

    [Fact]
    public void MinionOneAwayMech_MoveOverMech_MinionStaysAtMech()
    {
        var minion = MinionState.CreateNew(new Point(0, 0), Orientation.South);
        var mech = CreateMechAt(new Point(0, 2));
        var gameState = GameStateWith(new[] { mech }, new[] { minion });
        var expectedMinionState = minion with { Position = new Point(0, 1) };

        var newMinionState = Movement.MoveMinion(gameState, minion, Orientation.North, 2);

        newMinionState.Should().BeEquivalentTo(expectedMinionState);
    }

    private static MechState CreateMechAt(Point position)
        => new(Guid.NewGuid(), position, Orientation.North, CommandLine.Empty);

    private static GameState GameStateWith(IEnumerable<MechState> mechs, IEnumerable<MinionState> minions) =>
        GameState.Initial with
        {
            MechStates = mechs.ToImmutableList(),
            MinionStates = minions.ToImmutableList()
        };

    private static GameState GameStateWith(IEnumerable<MinionState> minions)
        => GameStateWith(Enumerable.Empty<MechState>(), minions);

}
