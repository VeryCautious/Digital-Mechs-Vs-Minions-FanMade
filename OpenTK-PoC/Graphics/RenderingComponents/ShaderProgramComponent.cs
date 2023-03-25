using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal sealed class ShaderProgramComponent : IRenderModelComponent
{
    private int _handle;
    private readonly Shader[] _shaders;

    public ShaderProgramComponent(params Shader[] shaders)
    {
        this._shaders = shaders;
    }

    public int GetAttributeLocation(string name) => GL.GetAttribLocation(_handle, name);

    public int GetUniformLocation(string name) => GL.GetUniformLocation(_handle, name);

    public void Load()
    {
        _handle = GL.CreateProgram();

        foreach (var shader in _shaders) {
            shader.Load();
            GL.AttachShader(_handle, shader.Handle);
        }
        
        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_handle);
            Console.WriteLine(infoLog);
        }

        foreach (var shader in _shaders)
            GL.DetachShader(_handle, shader.Handle);
    }

    public void Bind() => GL.UseProgram(_handle);

    public void Unbind() => GL.UseProgram(0);

    public void Dispose()
    {
        //TODO
    }

}
