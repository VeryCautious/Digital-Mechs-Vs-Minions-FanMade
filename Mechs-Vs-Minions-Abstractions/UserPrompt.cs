using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;

namespace Mechs_Vs_Minions_Abstractions;

public abstract record UserPrompt;
public sealed record SelectFieldFrom(IImmutableList<Point> Fields) : UserPrompt;
public sealed record SelectCardFrom(IImmutableList<PlayableCard> Cards) : UserPrompt;
public sealed record SelectCommandSlotFrom(IImmutableList<int> SlotIndices, CommandLine CommandLine) : UserPrompt;
public sealed record SelectRotationFrom(IImmutableList<Rotation> Rotations) : UserPrompt;
public sealed record Confirm(string Description) : UserPrompt;