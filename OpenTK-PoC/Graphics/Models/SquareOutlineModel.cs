using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using System.Collections.Immutable;
using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class SquareOutlineModel : RenderModel<BoardTileInstance>
{

    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public SquareOutlineModel(ICameraUniformData cameraUniformData) : base(
        ConstructRenderModelComponents(), ConstructRenderModelParameters()
    )
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
        => new(Matrix4.Identity, true);

    private static RenderComponents ConstructRenderModelComponents()
    {
        var shaderProgram = ShaderProgramFactory.CreatePrimitiveShader();

        var primitiveVertices = new List<PrimitiveVertex>();

        var color = Color4.Goldenrod;
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.1f, 0.1f, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.9f, 0.1f, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.9f, 0.9f, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(0.1f, 0.9f, 0), color));

        var geometryComponent = new ManualGeometryComponent<PrimitiveVertex>(
            shaderProgram, 
            primitiveVertices.ToArray(), 
            null, 
            PrimitiveType.LineLoop);

        return new RenderComponents(geometryComponent, shaderProgram, Array.Empty<IRenderModelComponent>());
    }

    protected override void OnBeforeParentLoad()
    {
    }

    public override void RenderInstanceWith(BoardTileInstance instance, IEnumerable<Uniform> staticUniforms)
    {
        if (!instance.IsOutlined) return;
        base.RenderInstanceWith(instance, staticUniforms);
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(BoardTileInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}