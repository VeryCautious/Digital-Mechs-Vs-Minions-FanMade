using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_GameLogic;
using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions;

namespace Mechs_Vs_Minions_GameLogic_Tests.CardTests;

public class BlazeTests
{
    public BlazeTests()
    {
        AssertionOptions.AssertEquivalencyUsing(o => o.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void MechAndNoMinions_UseBlazeWith2Steps_MechMoved2Steps()
    {
        var mech = CreateMechAt(new Point(0, 0));
        var gameState = GameStateWith(new[] { mech });
        var expectedGameState = gameState with
        {
            MechStates = ImmutableList.Create(mech with {Position = new(0,2)}),
            GamePhase = new EndOfGamePhase()
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseBlazeAction(0, 2));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public void MechInFrontOfMinion_UseBlazeWith1Steps_MechMoved1StepsStompedMinion()
    {
        var mech = CreateMechAt(new Point(0, 0));
        var gameState = GameStateWith(new[] { mech }, new [] { MinionState.CreateNew(new Point(0, 1), Orientation.South) });
        var expectedGameState = gameState with
        {
            MinionStates = ImmutableList.Create<MinionState>(),
            MechStates = ImmutableList.Create(mech with {Position = new Point(0,1)}),
            GamePhase = new EndOfGamePhase(),
            KilledMinions = 1,
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseBlazeAction(0, 1));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public void MechInTargetRangeOfMinion_UseBlazeWith1Steps_MechMoved1StepsBurnedMinion()
    {
        var mech = CreateMechAt(new Point(0, 0));
        var gameState = GameStateWith(new[] { mech }, new [] { MinionState.CreateNew(new Point(1, 2), Orientation.South) });
        var expectedGameState = gameState with
        {
            MinionStates = ImmutableList.Create<MinionState>(),
            MechStates = ImmutableList.Create(mech with {Position = new Point(0,2)}),
            GamePhase = new EndOfGamePhase(),
            KilledMinions = 1,
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new UseBlazeAction(0, 2));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    private static MechState CreateMechAt(Point position)
        => new(Guid.NewGuid(), position, Orientation.North, CommandLine.Empty);

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