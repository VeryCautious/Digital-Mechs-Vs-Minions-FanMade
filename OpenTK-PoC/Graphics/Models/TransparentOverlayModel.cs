using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class TransparentOverlayModel : RenderModel<OverlayInstance>
{
    private class DummyDrawable : IDrawable
    {
        public void Bind()
        {
        }

        public void Unbind()
        {
        }

        public void Dispose()
        {
        }

        public void Load()
        {
        }

        public void Draw()
        {
        }
    }

    private static RenderComponents GetRenderComponents() =>
        new(
            new DummyDrawable(),
            ShaderProgramFactory.CreatePrimitiveShader(),
            Array.Empty<IRenderModelComponent>()
        );

    private static RenderParameters GetRenderParameters() =>
        new(Matrix4.Identity, true);

    public TransparentOverlayModel() : base(GetRenderComponents(), GetRenderParameters())
    {
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(OverlayInstance instance)
    {
        return ImmutableList.Create<Uniform>();
    }
}