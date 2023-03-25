using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal class NoUniformProvider : IUniformProvider
{
    public IImmutableList<Uniform> GetUniforms()
    {
        return ImmutableList.Create<Uniform>();
    }
}