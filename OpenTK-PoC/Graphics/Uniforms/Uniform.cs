using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

namespace Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

internal abstract class Uniform
{
    private readonly string _name;

    protected Uniform(string name)
    {
        _name = name;
    }

    public void Set(ShaderProgramComponent program)
    {
        var location = program.GetUniformLocation(_name);

        if (location == -1)
        {
            return;
        }

        SetGlVariable(location);
    }

    protected abstract void SetGlVariable(int location);
}