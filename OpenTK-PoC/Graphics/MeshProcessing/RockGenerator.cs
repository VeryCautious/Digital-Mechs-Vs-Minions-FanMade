using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal static class RockGenerator
{

    public static (Mesh, ImmutableList<float>, ImmutableList<float>) Generate(int resolutionLevel)
    {
        var mesh = SphereGenerator.Generate(resolutionLevel);

        var noise = new Noise3D(2.0f);
        var smallNoise = new Noise3D(20.0f);

        var noiseList = mesh.VertexIds
            .Select(mesh.GetVertexPosition)
            .Select(noise.Generate)
            .ToImmutableList();

        var smallNoiseList = mesh.VertexIds
            .Select(mesh.GetVertexPosition)
            .Select(smallNoise.Generate)
            .ToImmutableList();

        foreach (var (vid,f,sf) in mesh.VertexIds.Zip(noiseList, smallNoiseList))
        {
            var v = mesh.GetVertexPosition(vid);
            mesh.UpdateVertex(vid, (0.4f + 0.45f*f + 0.25f*sf) * v);
        }

        mesh.Normalize();

        return (mesh, noiseList, smallNoiseList);
    }
}
