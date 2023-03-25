using System.Collections.Immutable;
using System.Drawing;

namespace Utilities;

public static class PointExtensions
{
    public static IImmutableList<Point> PointsAround(this Point basePoint)
    {
        var points = new List<Point>();
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                points.Add(new Point(basePoint.X + x, basePoint.Y + y));
            }
        }

        return points.ToImmutableList();
    }

    public static IImmutableList<Point> GetDirectlyAdjacent(this Point basePoint)
    {
        var points = new List<Point>();
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                if (Math.Abs(x) == Math.Abs(y))
                {
                    continue;
                }

                points.Add(new Point(basePoint.X + x, basePoint.Y + y));
            }
        }

        return points.ToImmutableList();
    }

    public static float DistanceTo(this Point a, Point b) => MathF.Sqrt(MathF.Pow(a.X - b.X, 2) + MathF.Pow(a.Y - b.Y, 2));

    public static Point RightNeighbor(this Point basePoint) => new Point(basePoint.X + 1, basePoint.Y);

    public static Point LeftNeighbor(this Point basePoint) => new Point(basePoint.X - 1, basePoint.Y);

    public static Point BottomNeighbor(this Point basePoint) => new Point(basePoint.X, basePoint.Y - 1);

    public static Point TopNeighbor(this Point basePoint) => new Point(basePoint.X, basePoint.Y + 1);

    public static Point TopRightNeighbor(this Point basePoint) => new Point(basePoint.X + 1, basePoint.Y + 1);

    public static IEnumerable<Point> AllNeighbors(this Point basePoint) => 
        from x in Enumerable.Range(-1, 3)
        from y in Enumerable.Range(-1, 3)
        where !(x == 0 && y == 0)
        select new Point(basePoint.X + x, basePoint.Y + y);

    public static float Length(this Point point) => MathF.Sqrt(MathF.Pow(point.X, 2) + MathF.Pow(point.Y, 2));

    public static float DistanceTo(this PointF point, PointF basePoint) => MathF.Sqrt(MathF.Pow(point.X - basePoint.X, 2) + MathF.Pow(point.Y - basePoint.Y, 2));
}