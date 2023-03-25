using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal class Color4Uniform : Uniform
{
    public Color4 Color;

    public Color4Uniform(string name) : this(name, Color4.White)
    {
    }

    public Color4Uniform(string name, Color4 color) : base(name)
    {
        Color = color;
    }

    protected override void SetGlVariable(int location)
    {
        GL.Uniform4(location, Color.R, Color.G, Color.B, Color.A);
    }
}