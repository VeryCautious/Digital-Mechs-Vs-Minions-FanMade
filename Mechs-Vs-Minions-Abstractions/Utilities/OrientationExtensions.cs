using System.Drawing;
using Mechs_Vs_Minions_Abstractions.GameStates;

namespace Mechs_Vs_Minions_Abstractions.Utilities;

public static class OrientationExtensions
{

    public static Orientation OrientationIn(this Orientation baseOrientation, Direction direction) =>
        (baseOrientation, direction) switch
        {
            (Orientation.North, Direction.Forward) => Orientation.North,
            (Orientation.North, Direction.Backward) => Orientation.South,
            (Orientation.North, Direction.Left) => Orientation.West,
            (Orientation.North, Direction.Right) => Orientation.East,
            (Orientation.South, Direction.Forward) => Orientation.South,
            (Orientation.South, Direction.Backward) => Orientation.North,
            (Orientation.South, Direction.Left) => Orientation.East,
            (Orientation.South, Direction.Right) => Orientation.West,
            (Orientation.East, Direction.Forward) => Orientation.East,
            (Orientation.East, Direction.Backward) => Orientation.West,
            (Orientation.East, Direction.Left) => Orientation.North,
            (Orientation.East, Direction.Right) => Orientation.South,
            (Orientation.West, Direction.Forward) => Orientation.West,
            (Orientation.West, Direction.Backward) => Orientation.East,
            (Orientation.West, Direction.Left) => Orientation.South,
            (Orientation.West, Direction.Right) => Orientation.North,
            _ => throw new ArgumentOutOfRangeException()
        };

    /// <summary>
    /// This function returns the position where an entity would land if it started at StartPosition (orientated in orientation) and moves steps
    /// The function does not consider any obsticals or if the target is a valid field.
    /// </summary>
    public static Point GetNewPositionFrom(this Point startPosition, Orientation orientation, Direction direction, int steps)
        => orientation.OrientationIn(direction) switch
        {
            Orientation.North => new Point(startPosition.X, startPosition.Y + steps),
            Orientation.South => new Point(startPosition.X, startPosition.Y - steps),
            Orientation.West => new Point(startPosition.X - steps, startPosition.Y),
            Orientation.East => new Point(startPosition.X + steps, startPosition.Y),
            _ => throw new NotImplementedException()
        };

    public static Orientation Turn(this Orientation baseOrientation, Rotation rotation)
        => (baseOrientation, rotation) switch
        {
            (Orientation.North, Rotation.Minus90Degrees) => Orientation.West,
            (Orientation.North, Rotation.Plus90Degrees) => Orientation.East,
            (Orientation.North, Rotation.Plus180Degrees) => Orientation.South,
            (Orientation.North, Rotation.NoRotation) => Orientation.North,
            (Orientation.South, Rotation.Minus90Degrees) => Orientation.East,
            (Orientation.South, Rotation.Plus90Degrees) => Orientation.West,
            (Orientation.South, Rotation.Plus180Degrees) => Orientation.North,
            (Orientation.South, Rotation.NoRotation) => Orientation.South,
            (Orientation.East, Rotation.Minus90Degrees) => Orientation.North,
            (Orientation.East, Rotation.Plus90Degrees) => Orientation.South,
            (Orientation.East, Rotation.Plus180Degrees) => Orientation.West,
            (Orientation.East, Rotation.NoRotation) => Orientation.East,
            (Orientation.West, Rotation.Minus90Degrees) => Orientation.South,
            (Orientation.West, Rotation.Plus90Degrees) => Orientation.North,
            (Orientation.West, Rotation.Plus180Degrees) => Orientation.East,
            (Orientation.West, Rotation.NoRotation) => Orientation.West,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static float ToFloatAngle(this Orientation orientation) => (int)orientation * (MathF.PI / 2.0f);
    /* public static float AngleBetweenOrientations(Orientation startOrientation, Orientation endOrientation)
    {
        if (startOrientation == endOrientation) return MathF.Tau;

        var startAngle = startOrientation.ToFloatAngle();
        var endAngle = endOrientation.ToFloatAngle();
        var angleBetween = endAngle - startAngle;
        if (angleBetween > MathF.Tau)
        {
            angleBetween = -(angleBetween - MathF.Tau);
        }
        return angleBetween;
    } */

    public static float AngleBetweenOrientations(Orientation startOrientation, Orientation endOrientation)
    {
        return (startOrientation, endOrientation) switch
        {
            (Orientation.South, Orientation.South) => MathF.Tau,
            (Orientation.North, Orientation.North) => MathF.Tau,
            (Orientation.East, Orientation.East) => MathF.Tau,
            (Orientation.West, Orientation.West) => MathF.Tau,

            (Orientation.South, Orientation.North) => MathF.PI,
            (Orientation.North, Orientation.South) => MathF.PI,
            (Orientation.East, Orientation.West) => MathF.PI,
            (Orientation.West, Orientation.East) => MathF.PI,

            (Orientation.South, Orientation.East) => -MathF.PI / 2.0f,
            (Orientation.East, Orientation.North) => -MathF.PI / 2.0f,
            (Orientation.North, Orientation.West) => -MathF.PI / 2.0f,
            (Orientation.West, Orientation.South) => -MathF.PI / 2.0f,

            (Orientation.East, Orientation.South) => MathF.PI / 2.0f,
            (Orientation.North, Orientation.East) => MathF.PI / 2.0f,
            (Orientation.West, Orientation.North) => MathF.PI / 2.0f,
            (Orientation.South, Orientation.West) => MathF.PI / 2.0f,

            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
