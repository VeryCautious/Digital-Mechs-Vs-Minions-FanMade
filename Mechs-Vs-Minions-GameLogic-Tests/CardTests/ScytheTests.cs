using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_GameLogic;
using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions;

namespace Mechs_Vs_Minions_GameLogic_Tests.CardTests;

public class ScytheTests
{

    public ScytheTests()
    {
        AssertionOptions.AssertEquivalencyUsing(o => o.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void MechFacingNorthAndNoMinions_UseScytheWithPlus90D_MechTurnsEast()
    {
        var mech = CreateMechFacing(Orientation.North);
        var gameState = GameStateWith(new[] { mech });
        var expectedGameState = gameState with
        {
            MechStates = ImmutableList.Create(mech with {Orientation = Orientation.East}),
            GamePhase = new EndOfGamePhase()
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseScytheAction(0, Rotation.Plus90Degrees, 2));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public void MechInFrontOfMinion_UseScytheWith1Steps_MechTurnedSlicedMinion()
    {
        var mech = CreateMechFacing(Orientation.West);
        var gameState = GameStateWith(new[] { mech }, new [] { MinionState.CreateNew(new Point(0, 1), Orientation.South) });
        var expectedGameState = gameState with
        {
            MinionStates = ImmutableList.Create<MinionState>(),
            MechStates = ImmutableList.Create(mech with {Orientation = Orientation.East}),
            GamePhase = new EndOfGamePhase(),
            KilledMinions = 1,
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseScytheAction(0, Rotation.Plus180Degrees, 2));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public void MechInFrontOf3Minion_UseScytheWith2MaxKills_Sliced2Minion()
    {
        var mech = CreateMechFacing(Orientation.West);
        var minions = new[]
        {
            MinionState.CreateNew(new Point(0, 1), Orientation.South),
            MinionState.CreateNew(new Point(1, 0), Orientation.South),
            MinionState.CreateNew(new Point(1, 1), Orientation.South)
        };
        var gameState = GameStateWith(new[] { mech }, minions);

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseScytheAction(0, Rotation.Plus180Degrees, 2));

        newGameState.MinionStates.Count.Should().Be(1);
    }


    private static MechState CreateMechFacing(Orientation orientation)
        => new(Guid.NewGuid(), new Point(0,0), orientation, CommandLine.Empty);

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