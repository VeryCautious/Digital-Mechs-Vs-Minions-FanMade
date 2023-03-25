using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal sealed class Matrix4Uniform : Uniform
{
    public Matrix4 Matrix;

    public Matrix4Uniform(string name) : this(name, Matrix4.Identity)
    {
    }

    private Matrix4Uniform(string name, Matrix4 matrix) : base(name)
    {
        Matrix = matrix;
    }

    protected override void SetGlVariable(int location)
    {
        GL.UniformMatrix4(location, false, ref Matrix);
    }
}