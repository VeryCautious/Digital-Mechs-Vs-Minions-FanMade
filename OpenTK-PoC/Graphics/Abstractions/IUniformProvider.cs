using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IUniformProvider
{
    IImmutableList<Uniform> GetUniforms();
}