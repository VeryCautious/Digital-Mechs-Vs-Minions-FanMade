using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal partial class Mesh
{
    public void Normalize()
    {
        var xMin = _vertices.Values.Select(v => v.X).Min();
        var xMax = _vertices.Values.Select(v => v.X).Max();
        var yMin = _vertices.Values.Select(v => v.Y).Min();
        var yMax = _vertices.Values.Select(v => v.Y).Max();
        var zMin = _vertices.Values.Select(v => v.Z).Min();
        var zMax = _vertices.Values.Select(v => v.Z).Max();

        var largestDimension = (new List<float> { xMax - xMin, yMax - yMin, zMax - zMin }).Max();

        var center = new Vector3(
            (xMax + xMin) / 2.0f,
            (yMax + yMin) / 2.0f,
            (zMax + zMin) / 2.0f
        );

        UpdateAllVertices(v => v - center);
        UpdateAllVertices(v => v * 2.0f / largestDimension);
    }

    public void ProjectToPlane(Vector3 pointInPlane, Vector3 normal)
    {
        UpdateAllVertices(v => ProjectPointToPlane(v, pointInPlane, normal));
    }

    /// <summary>
    /// Projects the point onto the given plane, if the point lies above the plan
    /// </summary>
    /// <param name="planeNormal"></param> has to be normalized
    /// <returns></returns>
    private Vector3 ProjectPointToPlane(Vector3 point, Vector3 pointInPlane, Vector3 planeNormal)
    {
        var distanceToPlane = DistanceToPlane(point, pointInPlane, planeNormal);
        if (distanceToPlane <= 0) return point;

        var basePoint = point - distanceToPlane * planeNormal;
        return basePoint;
    }

    private float DistanceToPlane(Vector3 point, Vector3 pointInPlane, Vector3 planeNormal) => Vector3.Dot(planeNormal, point - pointInPlane);

    private void UpdateAllVertices(Func<Vector3, Vector3> func)
    {
        foreach (var v in _vertices) UpdateVertex(v.Key, func(v.Value));
    }
}
