using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Abstractions.GameStates;

public abstract record GamePhase;
public sealed record DrawCardPhase(int MechIndex, IImmutableList<PlayableCard> DrawableCards) : GamePhase;
public sealed record PickDrawableCardsPhase : GamePhase;
public sealed record ApplyDamageToMechsPhase : GamePhase;
public sealed record MechPlayActionsPhase(int MechIndex, int SlotToPlay) : GamePhase;
public sealed record MinionMovePhase : GamePhase;
public sealed record QueueDamageToMechsPhase : GamePhase;
public sealed record EndOfGamePhase : GamePhase;
