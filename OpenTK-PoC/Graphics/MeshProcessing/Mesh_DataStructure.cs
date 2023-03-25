using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal partial class Mesh
{
    private readonly Dictionary<Guid, Vector3> _vertices;
    private readonly Dictionary<Guid, (Guid, Guid, Guid)> _faces;

    public Mesh()
    {
        _vertices = new Dictionary<Guid, Vector3>();
        _faces = new Dictionary<Guid, (Guid, Guid, Guid)>();
    }

    public Guid AddVertex(Vector3 v)
    {
        var guid = Guid.NewGuid();
        _vertices[guid] = v;
        return guid;
    }
    public void UpdateVertex(Guid id, Vector3 newPosition)
    {
        if (!_vertices.ContainsKey(id)) return;
        _vertices[id] = newPosition;
    }

    public Vector3 GetVertexPosition(Guid id) => _vertices[id];

    public IImmutableList<Guid> VertexIds => _vertices.Keys.ToImmutableList();

    public void RemoveVertex(Guid id) => _vertices.Remove(id);

    public Guid AddFace(Guid id1, Guid id2, Guid id3)
    {
        var guid = Guid.NewGuid();
        _faces[guid] = (id1, id2, id3);
        return guid;
    }

    public IImmutableList<Guid> FaceIds => _faces.Keys.ToImmutableList();

    public (Guid id1, Guid id2, Guid id3) GetVerticesOfFace(Guid id)
    {
        var face = _faces[id];
        return (face.Item1, face.Item2, face.Item3);
    }

    public void RemoveFace(Guid id) => _faces.Remove(id);
}
