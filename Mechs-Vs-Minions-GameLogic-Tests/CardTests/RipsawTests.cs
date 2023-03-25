using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_GameLogic;
using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions;

namespace Mechs_Vs_Minions_GameLogic_Tests.CardTests;

public class RipsawTests
{

    public RipsawTests()
    {
        AssertionOptions.AssertEquivalencyUsing(o => o.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void MechFacingNorthAndNoMinions_UseRipsaw_NoMinionsKilled()
    {
        var mech = CreateMechFacing(Orientation.North);
        var gameState = GameStateWith(new[] { mech });
        var expectedGameState = gameState with
        {
            GamePhase = new EndOfGamePhase(),
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseRipsawAction(0, 1));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public void MechFacingNorthAndOneMinion_UseRipsaw_KillsMinion()
    {
        var mech = CreateMechFacing(Orientation.North);
        var minion = CreateMinionAt(new Point(0, 1));
        var gameState = GameStateWith(new[] { mech }, new[] {minion});
        var expectedGameState = gameState with
        {
            MinionStates = ImmutableList.Create<MinionState>(),
            GamePhase = new EndOfGamePhase(),
            KilledMinions = 1,
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseRipsawAction(0, 1));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public void MechFacingNorthAndTwoMinions_UseRipsaw_KillsOneMinion()
    {
        var mech = CreateMechFacing(Orientation.North);
        var minion1 = CreateMinionAt(new Point(0, 1));
        var minion2 = CreateMinionAt(new Point(0, 2));
        var gameState = GameStateWith(new[] { mech }, new[] { minion1, minion2 });
        var expectedGameState = gameState with
        {
            MinionStates = ImmutableList.Create(minion2),
            GamePhase = new MechPlayActionsPhase(0, 1),
            KilledMinions = 1,
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseRipsawAction(0, 1));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    private static MechState CreateMechFacing(Orientation orientation)
        => new(Guid.NewGuid(), new Point(0, 0), orientation, CommandLine.Empty);

    private static MinionState CreateMinionAt(Point position)
        => new(Guid.NewGuid(), position, Orientation.South);

    private static GameState GameStateWith(IEnumerable<MechState> mechs, IEnumerable<MinionState> minions) =>
        GameState.Initial with
        {
            GamePhase = new MechPlayActionsPhase(0, 0),
            MechStates = mechs.ToImmutableList(),
            MinionStates = minions.ToImmutableList()
        };

    private static GameState GameStateWith(IEnumerable<MechState> mechs)
        => GameStateWith(mechs, Enumerable.Empty<MinionState>());
}
