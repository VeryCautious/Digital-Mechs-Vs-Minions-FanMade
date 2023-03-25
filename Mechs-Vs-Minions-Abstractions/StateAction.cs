using System.Drawing;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;

namespace Mechs_Vs_Minions_Abstractions;

public abstract record StateAction;
public sealed record UseEmptySlotAction(int MechIndex) : StateAction;
public sealed record UseOmniStompAction(int MechIndex, Direction Direction, int Steps) : StateAction;
public sealed record UseBlazeAction(int MechIndex, int Steps) : StateAction;
public sealed record UseAimBotAction(int MechIndex, Point TargetField) : StateAction;
public sealed record DrawCardAction(int MechIndex, PlayableCard PlayableCard, int TargetCommandSlot) : StateAction;
public sealed record UseScytheAction(int MechIndex, Rotation Rotation, int MaxMinionKills) : StateAction;
public sealed record UseRipsawAction(int MechIndex, int MaxMinionKills) : StateAction;
public sealed record UseSpeedAction(int MechIndex, int Steps) : StateAction;
public sealed record UseStuckControlsAction(int MechIndex, Rotation Rotation) : StateAction;
public sealed record MoveMinionsAction(Orientation Orientation) : StateAction;
public sealed record QueueDamageToMechsAction : StateAction;
public sealed record ApplyGlitchDamageToMechAction(int MechIndex, GlitchCard GlitchCard) : StateAction;
public sealed record ApplyStuckControlsDamageToMechAction(int MechIndex, int TargetCommandSlot, StuckControlsCard StuckControlsCard) : StateAction;
public sealed record PickDrawableCardsAction(int Amount) : StateAction;
public sealed record EndOfGameAction : StateAction;
public abstract record MetaGameAction : StateAction;
public sealed record ResetGameAction : MetaGameAction;