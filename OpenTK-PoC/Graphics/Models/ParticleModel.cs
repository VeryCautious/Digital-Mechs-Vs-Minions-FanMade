using System.Collections.Immutable;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class ParticleModel : RenderModel<ParticleInstance>
{
    private readonly ParticleUniformSet _particleUniformSet;
    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public ParticleModel(ICameraUniformData cameraUniformData, ParticleUniformSet particleUniformSet) : base(ConstructRenderModelComponents(), ConstructRenderModelParameters())
    {
        _particleUniformSet = particleUniformSet;
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters() => new(Matrix4.CreateTranslation(-.5f, -.5f, 0f) * Matrix4.CreateScale(0.05f), false);

    private static RenderComponents ConstructRenderModelComponents()
    {
        var shaderProgram = ShaderProgramFactory.CreateParticleShaderProgram();

        var primitiveVertices = new List<PrimitiveVertex>();

        var vertexPos = new[]
        {
            new Vector3(-1.0f,-1.0f,1.0f),
            new Vector3(1.0f,-1.0f,1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(1.0f,1.0f,1.0f),
            new Vector3(1.0f,-1.0f,1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(-1.0f,-1.0f,-1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,-1.0f),

            new Vector3(1.0f,1.0f,-1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,-1.0f),


            new Vector3(1.0f, -1.0f,-1.0f),
            new Vector3(1.0f, 1.0f,-1.0f),
            new Vector3(1.0f, -1.0f,1.0f),

            new Vector3(1.0f, 1.0f,1.0f),
            new Vector3(1.0f, 1.0f,-1.0f),
            new Vector3(1.0f, -1.0f,1.0f),

            new Vector3(-1.0f, -1.0f,-1.0f),
            new Vector3(-1.0f, 1.0f,-1.0f),
            new Vector3(-1.0f, -1.0f,1.0f),

            new Vector3(-1.0f, 1.0f,1.0f),
            new Vector3(-1.0f, 1.0f,-1.0f),
            new Vector3(-1.0f, -1.0f,1.0f),


            new Vector3(-1.0f,1.0f,-1.0f),
            new Vector3(1.0f,1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(1.0f,1.0f,1.0f),
            new Vector3(1.0f,1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(-1.0f,-1.0f,-1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,-1.0f,1.0f),

            new Vector3(1.0f,-1.0f,1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,-1.0f,1.0f)
        };

        primitiveVertices.AddRange(vertexPos.Select(pos => new PrimitiveVertex(pos, Color4.Red)));

        var geometryComponent = new ManualGeometryComponent<PrimitiveVertex>(shaderProgram, primitiveVertices.ToArray(), null);

        return new RenderComponents(geometryComponent, shaderProgram, Array.Empty<IRenderModelComponent>());
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(ParticleInstance instance)
    {
        _particleUniformSet.ParticleColorUniform.Color = instance.Color;
        return _positionalInstanceUniformSet.GetUniformsFor(instance).Union(_particleUniformSet.GetUniforms()).ToImmutableList();
    }
}
