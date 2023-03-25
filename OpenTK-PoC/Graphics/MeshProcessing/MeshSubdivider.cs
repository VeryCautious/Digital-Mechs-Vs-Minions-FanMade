
namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal class MeshSubdivider
{
    private readonly Dictionary<(Guid, Guid), Guid> _subdivVertices;

    public MeshSubdivider()
    {
        _subdivVertices = new Dictionary<(Guid, Guid), Guid>();
    }

    public Mesh Subdivide(Mesh mesh)
    {
        foreach (var fId in mesh.FaceIds)
        {
            var (id1, id2, id3) = mesh.GetVerticesOfFace(fId);

            var id4 = GetOrCreateSubdivideId(id1, id2, mesh);
            var id5 = GetOrCreateSubdivideId(id2, id3, mesh);
            var id6 = GetOrCreateSubdivideId(id3, id1, mesh);

            mesh.AddFace(id1, id4, id6);
            mesh.AddFace(id4, id2, id5);
            mesh.AddFace(id6, id5, id3);
            mesh.AddFace(id4, id5, id6);

            mesh.RemoveFace(fId);
        }

        return mesh;
    }

    private Guid GetOrCreateSubdivideId(Guid id1, Guid id2, Mesh mesh)
    {
        if (_subdivVertices.ContainsKey((id1, id2))) return _subdivVertices[(id1, id2)];
        if (_subdivVertices.ContainsKey((id2, id1))) return _subdivVertices[(id2, id1)];
        var v1 = mesh.GetVertexPosition(id1);
        var v2 = mesh.GetVertexPosition(id2);
        var m = MeshUtils.MiddleOf(v1, v2).Normalized();
        var idm = mesh.AddVertex(m);
        _subdivVertices[(id1, id2)] = idm;
        return idm;
    }
}
