using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal class ParticleUniformSet : IParticleUniformData
{
    public readonly Color4Uniform ParticleColorUniform;

    public Color4 ParticleColor => ParticleColorUniform.Color;
    
    public ParticleUniformSet()
    {
        ParticleColorUniform = new Color4Uniform("particleColor");
    }

    public IImmutableList<Uniform> GetUniforms()
    {
        return ImmutableList.Create<Uniform>(ParticleColorUniform);
    }
}