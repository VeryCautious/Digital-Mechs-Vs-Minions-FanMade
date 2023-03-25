using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions.GameStates;

namespace Mechs_Vs_Minions_Abstractions;

public abstract record Transition
{
    public static Transition None => new NoTransition();
};

public sealed record NoTransition : Transition;
public record MoveTransition(Guid Id, Point StartPosition, Point EndPosition) : Transition;
public sealed record UseAimBotTransition(Guid MechId, Point StartLocation, Point TargetLocation, IEnumerable<MinionState> ShotMinions) : Transition;
public sealed record UseOmniStompTransition(Guid MechId, Point StartPosition, Point EndPosition, IImmutableList<MoveTransition> Pushes, IImmutableList<MinionState> StompedMinions) : MoveTransition(MechId, StartPosition, EndPosition);
public sealed record UseSpeedTransition(Guid MechId, Point StartPosition, Point EndPosition, IImmutableList<MoveTransition> Pushes, IImmutableList<MinionState> StompedMinions) : MoveTransition(MechId, StartPosition, EndPosition);
public sealed record UseBlazeTransition(Guid MechId, Point StartPosition, Point EndPosition, IImmutableList<MoveTransition> Pushes, IImmutableList<MinionState> StompedMinions, IImmutableList<MinionState> BurnedMinions) : MoveTransition(MechId, StartPosition, EndPosition);
public sealed record UseScytheTransition(Guid MechId, Orientation StartOrientation, Orientation EndOrientation, IImmutableList<MinionState> SlicedMinions) : Transition;
public sealed record UseStuckControlsTransition(Guid MechId, Orientation StartOrientation, Orientation EndOrientation) : Transition;
public sealed record UseRipSawTransition(Guid MechId, Point StartPosition, Point EndPosition, IImmutableList<MinionState> SlicedMinions) : Transition;
public sealed record MinionMoveTransition(IImmutableList<MoveTransition> MoveTransitions) : Transition;
public sealed record ApplyGlitchDamageToMechTransition(Guid MechId) : Transition;
public sealed record ApplyStuckControlsToMechTransition(Guid MechId) : Transition;
public sealed record StartOfPlayActionPhaseTransition() : Transition;