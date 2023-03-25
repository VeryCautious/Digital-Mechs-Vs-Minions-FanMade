using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class CoordinateSystemModel: RenderModel<PositionalInstance>
{
    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public CoordinateSystemModel(ICameraUniformData cameraUniformData) : base(ConstructRenderModelComponents(), ConstructRenderModelParameters())
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
        => new(Matrix4.Identity, false);

    private static RenderComponents ConstructRenderModelComponents()
    {
        var shaderProgram = ShaderProgramFactory.CreatePrimitiveShader();

        var primitiveVertices = new List<PrimitiveVertex>();

        for (var i = 0; i < 20; i++)
        {
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(i, 0.0f, 0.0f), Color4.Red));
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(i+1, 0.0f, 0.0f), Color4.Red));
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(i+1, .1f, 0.0f), Color4.Red));
        }

        for (var i = 0; i < 20; i++)
        {
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.0f, i, 0.0f), Color4.Yellow));
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.0f ,i+1, 0.0f ), Color4.Yellow));
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.0f, i+1, .1f), Color4.Yellow));
        }

        for (var i = 0; i < 20; i++)
        {
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.0f, 0.0f, i), Color4.Green));
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.0f , 0.0f, i+1 ), Color4.Green));
            primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.0f, .1f, i+1), Color4.Green));
        }
      

        var geometryComponent = new ManualGeometryComponent<PrimitiveVertex>(shaderProgram, primitiveVertices.ToArray(), null);
  
        return new RenderComponents(geometryComponent, shaderProgram, Array.Empty<IRenderModelComponent>());
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(PositionalInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}
