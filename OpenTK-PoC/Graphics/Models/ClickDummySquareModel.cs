using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class ClickDummySquareModel : RenderModel<ClickableInstance>
{

    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public ClickDummySquareModel(ICameraUniformData cameraUniformData) : base(
        ConstructRenderModelComponents(), ConstructRenderModelParameters()
    ) {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
        => new(Matrix4.Identity, true);

    private static RenderComponents ConstructRenderModelComponents()
    {
        var shaderProgram = ShaderProgramFactory.CreateSelectableShader();

        var primitiveVertices = new List<PrimitiveVertex>();

        var color = Color4.Red;
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(0, 0, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(1, 0, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(0, 1, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(1, 1, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(1, 0, 0), color));
        primitiveVertices.Add(new PrimitiveVertex(new Vector3(0, 1, 0), color));

        var geometryComponent = new ManualGeometryComponent<PrimitiveVertex>(shaderProgram, primitiveVertices.ToArray(), null);

        return new RenderComponents(geometryComponent, shaderProgram, Array.Empty<IRenderModelComponent>());
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(ClickableInstance instance)
    {
        var colorUniform = new Color4Uniform("instanceColor", instance.Color);
        return _positionalInstanceUniformSet.GetUniformsFor(instance).Add(colorUniform);
    }
}