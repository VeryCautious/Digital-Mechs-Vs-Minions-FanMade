using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal class CameraUniformSet : ICameraUniformData
{
    public readonly Matrix4Uniform ProjectionUniform;
    public readonly Matrix4Uniform ViewUniform;
    public readonly Vector3Uniform CameraPositionUniform;

    public Matrix4 ProjectionMatrix => ProjectionUniform.Matrix;
    public Matrix4 ViewMatrix => ViewUniform.Matrix;
    public Vector3 CameraPositionVector => CameraPositionUniform.Vector;

    public CameraUniformSet()
    {
        ProjectionUniform = new Matrix4Uniform("projectionMatrix");
        ViewUniform = new Matrix4Uniform("viewMatrix");
        CameraPositionUniform = new Vector3Uniform("cameraPos");
    }

    public IImmutableList<Uniform> GetUniforms()
    {
        return ImmutableList.Create<Uniform>(ProjectionUniform, ViewUniform, CameraPositionUniform);
    }
}