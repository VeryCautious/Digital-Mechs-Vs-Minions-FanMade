using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using System.Collections.Immutable;
using System.Drawing;

namespace Mechs_Vs_Minions_Abstractions.GameStates;

public sealed record GameState(
    IImmutableList<MinionState> MinionStates,
    IImmutableList<MechState> MechStates,
    GamePhase GamePhase,
    GameBoard Board,
    IImmutableList<PlayableCard> CardDeck,
    IImmutableList<DamageCard> DamageCards,
    IImmutableDictionary<MechState, IImmutableList<DamageCard>> ToBeAppliedDamageCards,
    int KilledMinions
)
{

    public static GameState Initial => 
        new(
            ImmutableList.Create(
                MinionState.CreateNew(new(1, 3), Orientation.South),
                MinionState.CreateNew(new(3, 2), Orientation.South),
                MinionState.CreateNew(new(0, 5), Orientation.South),
                MinionState.CreateNew(new(4, 4), Orientation.South)
            ),
            ImmutableList.Create(
                MechState.CreateNew(new(0, 0), Orientation.North),
                MechState.CreateNew(new(3, 1), Orientation.East),
                MechState.CreateNew(new(0, 7), Orientation.South)
            ),
            new PickDrawableCardsPhase(),
            GameBoard.Initial(new Point(8,8)),
            GetCardDeck(),
            GetDamageDeck(),
            ImmutableDictionary<MechState, IImmutableList<DamageCard>>.Empty,
            0);


    public static GameState MechModels =>
        new(
            ImmutableList.Create(
                MinionState.CreateNew(new(2, 2), Orientation.South),
                MinionState.CreateNew(new(2, 3), Orientation.South)
            ),
            ImmutableList.Create(
                MechState.CreateNew(new(3, 1), Orientation.West),
                MechState.CreateNew(new(3, 2), Orientation.West),
                MechState.CreateNew(new(3, 3), Orientation.West),
                MechState.CreateNew(new(3, 4), Orientation.West)
            ),
            new PickDrawableCardsPhase(),
            GameBoard.Initial(new Point(5, 6)),
            GetCardDeck(),
            GetDamageDeck(),
            ImmutableDictionary<MechState, IImmutableList<DamageCard>>.Empty,
            0
        );

    public static GameState Test =>
        new(
            ImmutableList.Create(
                MinionState.CreateNew(new(2, 0), Orientation.South),
                MinionState.CreateNew(new Point(1,1), Orientation.West),
                MinionState.CreateNew(new Point(0,0), Orientation.West)
            ),
            ImmutableList.Create(
                MechState.CreateNew(new(1, 0), Orientation.South)
            ),
            new PickDrawableCardsPhase(),
            GameBoard.Initial(new Point(5, 6)),
            Enumerable.Repeat(new SpeedCard(), 10).OfType<PlayableCard>().ToImmutableList(),
            GetDamageDeck(),
            ImmutableDictionary<MechState, IImmutableList<DamageCard>>.Empty,
            0
        );

    public static GameState InitialRandom => Generate(3, 4, new Point(10, 8), 20);

    private static GameState Generate(int mechNumber, int minionNumber, Point boardSize, int obstacleNumber)
    {
        var rand = new Random();
        var figurePositions = GameBoard.Initial(boardSize).BoardTiles
            .OrderBy(_ => rand.Next())
            .Select(entry => entry.Key)
            .Take(mechNumber + minionNumber)
            .ToImmutableList();

        var mechPositions = figurePositions.Take(mechNumber).ToArray();
        var minionPositions = figurePositions.Skip(mechNumber).ToArray();

        var mechStates = mechPositions.Select(position =>
            MechState.CreateNew(position, GetRandomOrientation(rand))).ToImmutableList();

        var minionStates = minionPositions.Select(pos => MinionState.CreateNew(pos, GetRandomOrientation(rand))).ToImmutableList();

        var gameBoard = GameBoard.Generate(boardSize, figurePositions, obstacleNumber);

        return new GameState(
            minionStates,
            mechStates,
            new PickDrawableCardsPhase(),
            gameBoard,
            GetCardDeck(),
            GetDamageDeck(),
            ImmutableDictionary<MechState, IImmutableList<DamageCard>>.Empty,
            0
        );

    }

    private static Orientation GetRandomOrientation(Random rand) 
        => Enum.GetValues<Orientation>().MinBy(_ => rand.Next());

    private static Rotation GetRandomRotation(Random rand) 
        => Enum.GetValues<Rotation>().MinBy(_ => rand.Next());

    private static IImmutableList<PlayableCard> GetCardDeck()
    {
        var cards = new List<PlayableCard>();

        cards.AddRange(Enumerable.Repeat(new OmniStompCard(), 10));
        cards.AddRange(Enumerable.Repeat(new BlazeCard(), 10));
        cards.AddRange(Enumerable.Repeat(new ScytheCard(), 10));
        cards.AddRange(Enumerable.Repeat(new RipsawCard(), 10));
        cards.AddRange(Enumerable.Repeat(new SpeedCard(), 10));
        cards.AddRange(Enumerable.Repeat(new AimBotCard(), 10));

        var rnd = new Random();
        return cards.OrderBy(_ => rnd.Next()).ToImmutableList();
    }

    private static IImmutableList<DamageCard> GetDamageDeck()
    {
        var cards = new List<Func<DamageCard>>();
        var rnd = new Random();

        GlitchCard GlitchFactory() => new(rnd.Next(0, CommandLine.SlotCount), rnd.Next(0, CommandLine.SlotCount));
        StuckControlsCard StuckControlsFactory() => new(GetRandomRotation(rnd));

        cards.AddRange(Enumerable.Repeat(GlitchFactory, 10));
        cards.AddRange(Enumerable.Repeat(StuckControlsFactory, 10));

        return cards.Select(factory => factory()).OrderBy(_ => rnd.Next()).ToImmutableList();
    }
}
