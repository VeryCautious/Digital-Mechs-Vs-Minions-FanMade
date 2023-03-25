using System.Drawing;

namespace Mechs_Vs_Minions_Abstractions.GameStates;

public sealed record MechState (
    Guid Id,
    Point Position,
    Orientation Orientation,
    CommandLine CommandLine
) : IIdentifiable<Guid>
{
    public static MechState CreateNew(Point pos, Orientation orientation) => new(Guid.NewGuid(), pos, orientation, CommandLine.Empty);
}