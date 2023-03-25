using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface ICameraUniformData : IUniformProvider
{
    Matrix4 ProjectionMatrix { get; }
    Matrix4 ViewMatrix { get; }
    Vector3 CameraPositionVector { get; }
}