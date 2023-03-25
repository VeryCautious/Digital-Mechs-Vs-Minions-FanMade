using System.Drawing;

namespace Mechs_Vs_Minions_Abstractions.GameStates;

public sealed record MinionState(
    Guid Id,
    Point Position,
    Orientation Orientation
) : IIdentifiable<Guid>
{
    public static MinionState CreateNew(Point position, Orientation orientation) => new(Guid.NewGuid(), position, orientation);
};
