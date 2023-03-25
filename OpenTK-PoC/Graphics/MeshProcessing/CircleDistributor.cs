using System.Collections.Immutable;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal record Circle(Vector2 Position, float Radius);
internal static class CircleDistributor
{
    private const int SampleResolution = 6;
    private const float SampleDistance = 1f / (SampleResolution - 1f);
    private const float SampleDistanceHalved = SampleDistance / 2f;

    public static IEnumerable<Circle> GenerateRandomCirclesInTiles(IImmutableList<Vector2i> tiles)
    {
        var tileSet = new HashSet<Vector2i>(tiles);
        var candidates = GeneratePoints(tileSet, new Random());
        var circleSet = new CircleSet(tiles, candidates);

        circleSet.Fill();

        return circleSet.Circles;
    }

    #region Samples

    private static IEnumerable<Vector2> GeneratePoints(HashSet<Vector2i> tileSet, Random rnd) 
        => GetInnerPoints(tileSet)
        .Union(GetEdgePoints(tileSet))
        .Union(GetCornerPoints(tileSet))
        .Select(v => Shift(v, rnd));

    private static IEnumerable<Vector2> GetInnerPoints(HashSet<Vector2i> tilesSet)
    {
        var points = new List<Vector2>();
        foreach (var tile in tilesSet)
        {
            for (var x = 1; x < SampleResolution-1; x++)
            {
                for (var y = 1; y < SampleResolution-1; y++)
                {
                    points.Add(new Vector2(x, y) * SampleDistance + tile);
                }
            }
        }
        return points;
    }
    private static IEnumerable<Vector2> GetEdgePoints(HashSet<Vector2i> tilesSet)
    {
        var points = new List<Vector2>();
        foreach (var tile in tilesSet)
        {
            if (tilesSet.Contains(tile + Vector2i.UnitX))
            {
                for (var y = 1; y < SampleResolution - 1; y++)
                {
                    points.Add(new Vector2(1, y) * SampleDistance + tile);
                }
            }

            if (tilesSet.Contains(tile + Vector2i.UnitY))
            {
                for (var x = 1; x < SampleResolution - 1; x++)
                {
                    points.Add(new Vector2(x, 1) * SampleDistance + tile);
                }
            }
        }
        return points;
    }

    private static IEnumerable<Vector2> GetCornerPoints(HashSet<Vector2i> tilesSet)
    {
        var points = new List<Vector2>();
        foreach (var tile in tilesSet)
        {
            if (
                !tilesSet.Contains(tile + Vector2i.UnitX) ||
                !tilesSet.Contains(tile + Vector2i.UnitY) ||
                !tilesSet.Contains(tile + Vector2i.One)
            ) continue;
            
            for (var y = 1; y < SampleResolution - 1; y++)
            {
                points.Add(new Vector2(1, 1) * SampleDistance + tile);
            }
        }
        return points;
    }

    #endregion Samples

    private static Vector2 Shift(Vector2 vector, Random rnd)
    {
        var angle = (float) rnd.NextDouble() * MathF.Tau;
        var diff = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * SampleDistanceHalved;
        return vector + diff;
    }

    private class CircleSet
    {
        private readonly HashSet<Vector2i> _tiles;
        private IList<Vector2> _candidates;
        private readonly IList<Circle> _circles;

        private const float MinRadius = 0.1f;

        public CircleSet(IEnumerable<Vector2i> tiles, IEnumerable<Vector2> possiblePositions)
        {
            _tiles = new HashSet<Vector2i>(tiles);
            _candidates = possiblePositions.ToList();
            _circles = new List<Circle>();
        }

        public IEnumerable<Circle> Circles => _circles;

        public void Fill()
        {
            while (_candidates.Count > 0)
            {
                CreateNewCircle();
            }
        }

        private void CreateNewCircle()
        {
            if (!_candidates.Any()) return;

            var possibleCircleRadii = _candidates.Select(CalcMaxRadius).ToList();

            var sampleIndex = Math.Max(SampleFromDistribution(possibleCircleRadii), 0);

            var position = _candidates[sampleIndex];
            var radius = possibleCircleRadii[sampleIndex];

            var sampledCircle = new Circle(position, radius);
            _circles.Add(sampledCircle);

            RemoveAllCandidatesCoveredBy(sampledCircle);
        }

        private static int SampleFromDistribution(List<float> list)
        {
            var accList = AccumulatedList(list);
            var sum = accList.Last();
            var sample = (float)new Random().NextDouble() * sum;
            var sampleIndex = accList.BinarySearch(sample);
            return ~sampleIndex;
        }

        private static List<float> AccumulatedList(List<float> list)
        {
            var acc = new List<float>();
            var sum = 0.0f;

            foreach (var value in list)
            {
                sum += value;
                acc.Add(sum);
            }

            return acc;
        }

        private void RemoveAllCandidatesCoveredBy(Circle circle)
        {
            _candidates = _candidates.Where(candidate => !IsCovered(candidate, circle)).ToList();
        }

        private float CalcMaxRadius(Vector2 pos) {
            if (!_circles.Any())
            {
                return CalcDistanceToBorder(pos, _tiles);
            }

            var result = Math.Min(
                CalcDistanceToBorder(pos, _tiles),
                CalcDistanceToClosestCircle(pos, _circles)
            );

            return result;
        }

        private static float CalcDistanceToBorder(Vector2 pos, IReadOnlySet<Vector2i> tileSet)
        {
            List<float> possibleRadii = new();

            var tile = new Vector2i((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
            var innerX = pos.X - tile.X;
            var innerY = pos.Y - tile.Y;

            if (!tileSet.Contains(tile + Vector2i.UnitX)) possibleRadii.Add(1.0f - innerX);
            if (!tileSet.Contains(tile + Vector2i.UnitY)) possibleRadii.Add(1.0f - innerY);
            if (!tileSet.Contains(tile - Vector2i.UnitX)) possibleRadii.Add(innerX);
            if (!tileSet.Contains(tile - Vector2i.UnitY)) possibleRadii.Add(innerY);

            if (!tileSet.Contains(tile + Vector2i.UnitX + Vector2i.UnitY)) possibleRadii.Add(new Vector2(1.0f - innerX, 1.0f - innerY).Length);
            if (!tileSet.Contains(tile + Vector2i.UnitX - Vector2i.UnitY)) possibleRadii.Add(new Vector2(1.0f - innerX, innerY).Length);
            if (!tileSet.Contains(tile - Vector2i.UnitX + Vector2i.UnitY)) possibleRadii.Add(new Vector2(innerX, 1.0f - innerY).Length);
            if (!tileSet.Contains(tile - Vector2i.UnitX - Vector2i.UnitY)) possibleRadii.Add(new Vector2(innerX, innerY).Length);

            return possibleRadii.Min();
        }

        private static float CalcDistanceToClosestCircle(Vector2 pos, IEnumerable<Circle> circles) 
            => circles.Select(circle => (circle.Position - pos).Length - circle.Radius).Min();

        private static bool IsCovered(Vector2 pos, Circle circle) 
            => circle.Radius + MinRadius >= (pos - circle.Position).Length;

    }
}