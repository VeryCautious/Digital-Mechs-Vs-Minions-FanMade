using OpenTK.Mathematics;
using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal class LightUniformSet : IUniformProvider
{
    public readonly Matrix4Uniform ProjectionUniform;
    public readonly Matrix4Uniform ViewUniform;
    public readonly Vector3Uniform LightPositionUniform;

    public Matrix4 ProjectionMatrix => ProjectionUniform.Matrix;
    public Matrix4 ViewMatrix => ViewUniform.Matrix;
    public Vector3 CameraPositionVector => LightPositionUniform.Vector;

    public LightUniformSet()
    {
        ProjectionUniform = new Matrix4Uniform("lightProjectionMatrix");
        ViewUniform = new Matrix4Uniform("lightViewMatrix");
        LightPositionUniform = new Vector3Uniform("lightPos");
    }

    public IImmutableList<Uniform> GetUniforms()
    {
        return ImmutableList.Create<Uniform>(ProjectionUniform, ViewUniform, LightPositionUniform);
    }
}