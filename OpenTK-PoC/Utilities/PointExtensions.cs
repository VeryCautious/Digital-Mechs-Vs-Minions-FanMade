using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Utilities;

public static class PointExtensions
{
    public static Vector3 Transform(this Matrix4 transform, Vector3 vector)
    {
        var vec = new Vector4(vector, 1.0f) * transform;
        return Vector3.Multiply(vec.Xyz, 1 / vec.W);
    }
}