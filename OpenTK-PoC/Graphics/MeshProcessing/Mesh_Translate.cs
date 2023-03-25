using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal partial class Mesh
{
    public (Vector3[] vertexArray, Vector3[] normalArray, int[] indexArray) DataWithVertexNormals()
    {
        var vertices = _vertices.ToList();

        var vertexArray = vertices.Select(entry => entry.Value).ToArray();

        var vToIndex = vertices
            .Select(entry => entry.Key)
            .Select((guid, index) => (guid, index))
            .ToDictionary(entry => entry.guid, entry => entry.index);

        var indexArray = _faces.Values
            .SelectMany(triple => new List<Guid> { triple.Item1, triple.Item2, triple.Item3 })
            .Select(vid => vToIndex[vid])
            .ToArray();

        var faceNormals = _faces.Keys.ToDictionary(
            fid => fid,
            fid => CalcFaceNormal(fid)
        );

        var vertexFaceIncidences = GetVertexFaceIncidences();

        var vertexNormals = vertices.Select(vertex => CalcVertexNormal(vertexFaceIncidences[vertex.Key], faceNormals)).ToArray();

        return (vertexArray, vertexNormals, indexArray);
    }

    public (Vector3[] vertexArray, Vector3[] normalArray) DataWithFaceNormals()
    {
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        
        foreach (var faceEntry in _faces)
        {
            var faceNormal = CalcFaceNormal(faceEntry.Key);

            vertices.Add(GetVertexPosition(faceEntry.Value.Item1));
            normals.Add(faceNormal);

            vertices.Add(GetVertexPosition(faceEntry.Value.Item2));
            normals.Add(faceNormal);

            vertices.Add(GetVertexPosition(faceEntry.Value.Item3));
            normals.Add(faceNormal);
        }

        return (vertices.ToArray(), normals.ToArray());
    }

    private Vector3 CalcFaceNormal(Guid faceId)
    {
        var (vid1, vid2, vid3) = GetVerticesOfFace(faceId);
        var v1 = GetVertexPosition(vid1);
        var v2 = GetVertexPosition(vid2);
        var v3 = GetVertexPosition(vid3);

        var a = v2 - v1;
        var b = v3 - v1;

        return Vector3.Cross(a, b).Normalized();
    }

    private Dictionary<Guid, List<Guid>> GetVertexFaceIncidences()
    {
        var vertexFaceIncidences = _vertices.Keys.ToDictionary(vid => vid, _ => new List<Guid>());
        foreach (var entry in _faces)
        {
            vertexFaceIncidences[entry.Value.Item1].Add(entry.Key);
            vertexFaceIncidences[entry.Value.Item2].Add(entry.Key);
            vertexFaceIncidences[entry.Value.Item3].Add(entry.Key);
        }
        return vertexFaceIncidences;
    }

    private Vector3 CalcVertexNormal(List<Guid> faceIds, Dictionary<Guid, Vector3> faceNormals)
    {
        return faceIds.Select(fId => faceNormals[fId])
            .Aggregate(
                Vector3.Zero,
                (sum, vec) => sum + vec
            )
            .Normalized();
    }
}
