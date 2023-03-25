using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using System.Collections.Immutable;
using OpenTK.Graphics.OpenGL4;
using Mechs_Vs_Minions_Graphics.Graphics.Animations;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class PolylineModel : RenderModel<PositionalInstance>
{

    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public PolylineModel(ICameraUniformData cameraUniformData) : base(
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

        // hard coded stuff
        var curve = new Vector3BezierCurve(new List<Vector3>
        {
            new Vector3(-4.5f, -1.0f, -0.5f),
            new Vector3(-4.5f, -1.7071068f, -1.2071068f),
            new Vector3(2.5f, -1.7071068f, 1.7928934f),
            new Vector3(2.5f, -1, 2.5f),
        });
        var knots = curve.Subdivide(3);

        var curveColor = Color4.Goldenrod;
        var primitiveVertices = knots.Select(knot => new PrimitiveVertex(knot, curveColor)).Union(curve.Knots.Select(knot => new PrimitiveVertex(knot, Color4.BlueViolet)).Reverse());

        var geometryComponent = new ManualGeometryComponent<PrimitiveVertex>(
            shaderProgram,
            primitiveVertices.ToArray(),
            null,
            PrimitiveType.LineStrip);

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