using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal sealed class Vector3Uniform : Uniform
{
    public Vector3 Vector;

    public Vector3Uniform(string name) : this(name, Vector3.Zero) { }

    private Vector3Uniform(string name, Vector3 vector) : base(name)
    {
        Vector = vector;
    }

    protected override void SetGlVariable(int location)
    {
        GL.Uniform3(location, Vector.X, Vector.Y, Vector.Z);
    }
}