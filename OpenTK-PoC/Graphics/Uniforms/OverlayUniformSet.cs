using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal class OverlayUniformSet : IOverlayUniformData
{
    public readonly Matrix4Uniform ProjectionUniform;
    public readonly Matrix4Uniform ModelProjectionUniform;

    public Matrix4 ProjectionMatrix => ProjectionUniform.Matrix;

    public OverlayUniformSet()
    {
        ProjectionUniform = new Matrix4Uniform("overlayProjectionMatrix");
        ModelProjectionUniform = new Matrix4Uniform("modelProjectionMatrix");
    }

    public IImmutableList<Uniform> GetUniforms()
    {
        return ImmutableList.Create<Uniform>(ProjectionUniform, ModelProjectionUniform);
    }
}