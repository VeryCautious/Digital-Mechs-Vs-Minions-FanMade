using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_GameLogic;
using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;

namespace Mechs_Vs_Minions_GameLogic_Tests.CardTests;

public class DamageTests
{
    public DamageTests()
    {
        AssertionOptions.AssertEquivalencyUsing(o => o.
            IncludingAllRuntimeProperties().
            AllowingInfiniteRecursion()
        );
    }

    [Fact]
    public void MechsDoNotStandNextToMinions_ApplyQueueDamageToMechs_QueueIsEmpty()
    {
        var mech1 = CreateMechAt(new Point(0, 0));
        var mech2 = CreateMechAt(new Point(0, 1));
        var damageCards = new DamageCard[] { new GlitchCard(1, 2) };
        var gameState = QueueDamageStateWith(
            new[] { mech1, mech2 },
            new []
            {
                MinionState.CreateNew(new Point(2, 1), Orientation.South),
                MinionState.CreateNew(new Point(2, 2), Orientation.South),
            },
            damageCards
        );

        var expectedGameState = gameState with
        {
            ToBeAppliedDamageCards = ImmutableDictionary<MechState, IImmutableList<DamageCard>>.Empty,
            GamePhase = new PickDrawableCardsPhase()
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new QueueDamageToMechsAction());

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }


    [Fact]
    public void MechsStandNextToMinions_ApplyQueueDamageToMechs_QueueIsFilled()
    {
        var mech1 = CreateMechAt(new Point(0, 0));
        var mech2 = CreateMechAt(new Point(0, 2));
        var damageCards = new DamageCard[] { new GlitchCard(1, 2), new StuckControlsCard(Rotation.Minus90Degrees), new StuckControlsCard(Rotation.Plus180Degrees) };
        var gameState = QueueDamageStateWith(
            new[] { mech1, mech2 },
            new []
            {
                MinionState.CreateNew(new Point(0, 1), Orientation.South),
                MinionState.CreateNew(new Point(1, 2), Orientation.South),
            },
            damageCards
        );


        var expectedToBeAppliedDamageCards = new Dictionary<MechState, IImmutableList<DamageCard>>
        {
            { mech1, new[] { damageCards[0] }.ToImmutableList() },
            { mech2, new[] { damageCards[1], damageCards[2] }.ToImmutableList() }
        };

        var expectedGameState = gameState with
        {
            ToBeAppliedDamageCards = expectedToBeAppliedDamageCards.ToImmutableDictionary(),
            DamageCards = ImmutableList<DamageCard>.Empty,
            GamePhase = new ApplyDamageToMechsPhase()
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new QueueDamageToMechsAction());

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public void MechsHasApplyGlitchQueued_ApplyGlitch_QueueIsEmptyAndSlotsAreSwapped()
    {
        var mech1 = CreateMechAt(new Point(0, 0)) with
        {
            CommandLine = CommandLine.Empty.
                PlaceCard(0, new BlazeCard()).
                PlaceCard(2, new RipsawCard()).
                PlaceCard(4, new StuckControlsCard(Rotation.NoRotation))
        };
        var damageCard = new GlitchCard(0, 2);
        var toBeAppliedDamageCards = new Dictionary<MechState, IImmutableList<DamageCard>>
        {
            { mech1, new DamageCard[] { damageCard }.ToImmutableList() }
        };
        var gameState =  GameState.Initial with
        {
            MechStates = new[] { mech1 }.ToImmutableList(),
            MinionStates = ImmutableList<MinionState>.Empty,
            DamageCards = ImmutableList<DamageCard>.Empty,
            GamePhase = new ApplyDamageToMechsPhase(),
            ToBeAppliedDamageCards = toBeAppliedDamageCards.ToImmutableDictionary()
        };
        
        var expectedMech1State = mech1 with
        {
            CommandLine = mech1.CommandLine.SwapSlots(0, 2),
        };
        var expectedGameState = gameState with
        {
            MechStates = new[] { expectedMech1State }.ToImmutableList(),
            ToBeAppliedDamageCards = ImmutableDictionary<MechState, IImmutableList<DamageCard>>.Empty,
            DamageCards = ImmutableList<DamageCard>.Empty,
            GamePhase = new PickDrawableCardsPhase()
        };

        var (_, newGameState) = GameStateTransformer.ApplySpecific(gameState, new ApplyGlitchDamageToMechAction(0, damageCard));

        newGameState.Should().BeEquivalentTo(expectedGameState);
    }

    private static MechState CreateMechAt(Point position)
        => new(Guid.NewGuid(), position, Orientation.North, CommandLine.Empty);

    private static GameState QueueDamageStateWith(IEnumerable<MechState> mechs, IEnumerable<MinionState> minions, IEnumerable<DamageCard> damageCards) =>
        GameState.Initial with
        {
            GamePhase = new QueueDamageToMechsPhase(),
            MechStates = mechs.ToImmutableList(),
            MinionStates = minions.ToImmutableList(),
            DamageCards = damageCards.ToImmutableList()
        };
    
}