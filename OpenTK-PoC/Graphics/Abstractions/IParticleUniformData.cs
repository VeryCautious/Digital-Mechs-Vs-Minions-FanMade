using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IParticleUniformData : IUniformProvider
{
    Color4 ParticleColor { get; }
}