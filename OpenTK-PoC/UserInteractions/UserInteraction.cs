using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using System.Drawing;

namespace Mechs_Vs_Minions_Graphics.UserInteractions;

internal record UserInteraction;
internal sealed record DrawableCardSelected(int CardIndex) : UserInteraction;
internal sealed record CommandLineSlotSelected(int SlotIndex) : UserInteraction;
internal sealed record ConfirmAction : UserInteraction;
internal sealed record DirectionSelected(Direction Direction) : UserInteraction;
internal sealed record RotationSelected(Rotation Rotation) : UserInteraction;
internal sealed record FieldSelected(Point Point) : UserInteraction;
internal sealed record MetaGameInteraction(MetaGameRequest Request) : UserInteraction;