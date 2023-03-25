using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal class SphereGenerator
{
    public static Mesh Generate(int resolutionLevel)
    {
        var mesh = Icosahedron();
        var sub = new MeshSubdivider();

        for (var i = 0; i < resolutionLevel; i++) mesh = sub.Subdivide(mesh);

        mesh.Normalize();

        return mesh;
    }

    /// <summary>
    /// from https://www.danielsieger.com/blog/2021/01/03/generating-platonic-solids.html
    /// and https://www.danielsieger.com/blog/2021/03/27/generating-spheres.html
    /// </summary>
    private static Mesh Icosahedron()
    {
        var mesh = new Mesh();

        float phi = (1.0f + MathF.Sqrt(5.0f)) * 0.5f; // golden ratio
        float a = 1.0f;
        float b = 1.0f / phi;

        // add vertices
        var v1 = mesh.AddVertex(new Vector3(0, b, -a).Normalized());
        var v2 = mesh.AddVertex(new Vector3(b, a, 0).Normalized());
        var v3 = mesh.AddVertex(new Vector3(-b, a, 0).Normalized());
        var v4 = mesh.AddVertex(new Vector3(0, b, a).Normalized());
        var v5 = mesh.AddVertex(new Vector3(0, -b, a).Normalized());
        var v6 = mesh.AddVertex(new Vector3(-a, 0, b).Normalized());
        var v7 = mesh.AddVertex(new Vector3(0, -b, -a).Normalized());
        var v8 = mesh.AddVertex(new Vector3(a, 0, -b).Normalized());
        var v9 = mesh.AddVertex(new Vector3(a, 0, b).Normalized());
        var v10 = mesh.AddVertex(new Vector3(-a, 0, -b).Normalized());
        var v11 = mesh.AddVertex(new Vector3(b, -a, 0).Normalized());
        var v12 = mesh.AddVertex(new Vector3(-b, -a, 0).Normalized());

        // add triangles
        mesh.AddFace(v3, v2, v1);
        mesh.AddFace(v2, v3, v4);
        mesh.AddFace(v6, v5, v4);
        mesh.AddFace(v5, v9, v4);
        mesh.AddFace(v8, v7, v1);
        mesh.AddFace(v7, v10, v1);
        mesh.AddFace(v12, v11, v5);
        mesh.AddFace(v11, v12, v7);
        mesh.AddFace(v10, v6, v3);
        mesh.AddFace(v6, v10, v12);
        mesh.AddFace(v9, v8, v2);
        mesh.AddFace(v8, v9, v11);
        mesh.AddFace(v3, v6, v4);
        mesh.AddFace(v9, v2, v4);
        mesh.AddFace(v10, v3, v1);
        mesh.AddFace(v2, v8, v1);
        mesh.AddFace(v12, v10, v7);
        mesh.AddFace(v8, v11, v7);
        mesh.AddFace(v6, v12, v5);
        mesh.AddFace(v11, v9, v5);

        return mesh;
    }
}
