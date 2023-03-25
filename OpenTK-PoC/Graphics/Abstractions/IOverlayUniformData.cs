using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IOverlayUniformData : IUniformProvider
{
    Matrix4 ProjectionMatrix { get; }
}