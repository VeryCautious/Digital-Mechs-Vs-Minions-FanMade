using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class RockModel : RenderModel<PositionalInstance>
{
    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public static RockModel Generate(ICameraUniformData cameraUniformData) => new RockModel(cameraUniformData);

    private RockModel(ICameraUniformData cameraUniformData) : base(ConstructRenderModelComponents(), ConstructRenderModelParameters())
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
        => new(Matrix4.Identity, false);

    private static RenderComponents ConstructRenderModelComponents()
    {
        var shaderProgram = ShaderProgramFactory.CreateRockShader();
        
        var (rock, noiseList, smallNoiseList) = RockGenerator.Generate(3);
        
        var (vertexArray, normalArray, indexArray) = rock.DataWithVertexNormals();

        var colors = noiseList.Zip(smallNoiseList, (nn, sn) => new { Noise = nn, SmallNoise = sn })
            .Select(entry => GetGrayValue(entry.Noise, entry.SmallNoise))
            .ToList();
        var primitiveVertices = vertexArray.Zip(normalArray, colors)
            .Select(entry => new ProceduralVertex(entry.First, entry.Second, new Color4(entry.Third, entry.Third, entry.Third, 1.0f)))
            .ToArray();

        var geometryComponent = new ManualGeometryComponent<ProceduralVertex>(shaderProgram, primitiveVertices.ToArray(), indexArray);

        return new RenderComponents(geometryComponent, shaderProgram, Array.Empty<IRenderModelComponent>());
    }

    private static float GetGrayValue(float noise, float smallNoise)
    {
        return 0.1f + 0.35f * noise + 0.25f * smallNoise;
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(PositionalInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}
