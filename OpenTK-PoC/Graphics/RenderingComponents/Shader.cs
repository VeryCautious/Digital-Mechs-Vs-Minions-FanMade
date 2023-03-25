using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal sealed class Shader
{
    public int Handle { get; private set; }

    private readonly ShaderType _type;
    private readonly string _code;

    public Shader(ShaderType type, string code)
    {
        _type = type;
        _code = code;
    }

    public void Load() {
        Handle = GL.CreateShader(_type);
        GL.ShaderSource(Handle, _code);
        GL.CompileShader(Handle);

        GL.GetShader(Handle, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            var infoLog = GL.GetShaderInfoLog(Handle);
            Console.WriteLine(infoLog);
        }
    }

}