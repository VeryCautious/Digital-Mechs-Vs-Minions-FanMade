using System.Collections.Immutable;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;

namespace Mechs_Vs_Minions_GameLogic;

internal static class GamePhaseTransitions
{
    public static GameState WithNextPhaseFrom(this GameState gameState, MechPlayActionsPhase mechPlayActionsPhase)
        => gameState with
        {
            GamePhase = NextGamePhaseFrom(gameState, mechPlayActionsPhase)
        };

    public static GameState WithNextPhaseFrom(this GameState gameState, QueueDamageToMechsPhase _)
        => gameState with
        {
            GamePhase = gameState.ToBeAppliedDamageCards.Any() ? new ApplyDamageToMechsPhase() : new PickDrawableCardsPhase()
        };

    public static GameState WithNextPhaseFrom(this GameState gameState, ApplyDamageToMechsPhase _)
        => gameState with
        {
            GamePhase = gameState.ToBeAppliedDamageCards.Any() ? new ApplyDamageToMechsPhase() : new PickDrawableCardsPhase()
        };

    private static GamePhase NextGamePhaseFrom(GameState gameState, MechPlayActionsPhase mechPlayActionsPhase)
    {
        if (!gameState.MinionStates.Any()) return new EndOfGamePhase();

        if (mechPlayActionsPhase.SlotToPlay + 1 < CommandLine.SlotCount)
        {
            return new MechPlayActionsPhase(mechPlayActionsPhase.MechIndex, mechPlayActionsPhase.SlotToPlay + 1);
        }
        
        if (mechPlayActionsPhase.MechIndex + 1 < gameState.MechStates.Count)
        {
            return new MechPlayActionsPhase(mechPlayActionsPhase.MechIndex + 1, 0);
        }
        
        return new MinionMovePhase();
    }

    public static GamePhase NextGamePhaseFrom(MinionMovePhase _)
    {
        return new QueueDamageToMechsPhase();
    }

    public static GamePhase NextGamePhaseFrom(IImmutableList<PlayableCard> drawableCards, PickDrawableCardsPhase _)
        => new DrawCardPhase(0, drawableCards);

    public static GamePhase NextGamePhaseFrom(GameState gameState, DrawCardPhase drawCardPhase, PlayableCard removedPlayableCard)
    {
        var index = drawCardPhase.DrawableCards.IndexOf(removedPlayableCard, 0, drawCardPhase.DrawableCards.Count, EqualityComparer<PlayableCard>.Default);

        if (index == -1) {
            throw new ArgumentException($"The playableCard {removedPlayableCard.GetType()} is not drawable");
        }

        if (drawCardPhase.MechIndex + 1 < gameState.MechStates.Count)
        {
            return new DrawCardPhase(drawCardPhase.MechIndex + 1, drawCardPhase.DrawableCards.RemoveAt(index));
        }
        
        return new MechPlayActionsPhase(0, 0);
    }

    internal static GamePhase NextGamePhaseFrom(EndOfGamePhase endOfGamePhase)
    {
        return new EndOfGamePhase();
    }
}
