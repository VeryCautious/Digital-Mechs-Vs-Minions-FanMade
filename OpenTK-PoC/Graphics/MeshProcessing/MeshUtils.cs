using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

internal class MeshUtils
{
    public static Vector3 MiddleOf(Vector3 a, Vector3 b) => 0.5f * (a + b);
}
