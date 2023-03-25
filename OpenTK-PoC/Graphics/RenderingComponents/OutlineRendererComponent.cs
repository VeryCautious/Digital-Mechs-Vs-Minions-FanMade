using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal class OutlineRendererComponent
{
    private readonly IDrawable _hostDrawable;
    private readonly ShaderProgramComponent _hostShaderProgram;

    private readonly IDrawable _outlineDrawable;
    private readonly ShaderProgramComponent _outlineShaderProgram;


    public OutlineRendererComponent(IDrawable hostDrawable, ShaderProgramComponent hostShaderProgram, IDrawable outlineDrawable, ShaderProgramComponent outlineShaderProgram)
    {
        _hostDrawable = hostDrawable;
        _hostShaderProgram = hostShaderProgram;
        _outlineDrawable = outlineDrawable;
        _outlineShaderProgram = outlineShaderProgram;
    }

    private void BindOutline()
    {
        _hostShaderProgram.Unbind();
        _hostDrawable.Unbind();
        
        _outlineShaderProgram.Bind();
        _outlineDrawable.Bind();
    }

    private void UnbindOutline()
    {   
        _outlineShaderProgram.Unbind();
        _outlineDrawable.Unbind();

        _hostShaderProgram.Bind();
        _hostDrawable.Bind();
    }

    public void Load()
    {
        _outlineShaderProgram.Load();
        _outlineDrawable.Load();
    }

    public void RenderOutlinedInstanceWith(params Uniform[] uniforms)
    {
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
        GL.StencilMask(0xFF);
        GL.Enable(EnableCap.DepthTest);

        RenderHostWith(uniforms);

        GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
        GL.StencilMask(0x00);

        RenderMaskedWith(uniforms);

        GL.StencilMask(0xFF);
        GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
        GL.Enable(EnableCap.DepthTest);
    }

    private void RenderHostWith(Uniform[] uniforms)
    {
        
        foreach (var uniform in uniforms)
        {
            uniform.Set(_hostShaderProgram);
        }
        _hostDrawable.Draw();

    }

    private void RenderMaskedWith(Uniform[] uniforms)
    {
        BindOutline();
        
        foreach (var uniform in uniforms)
        {
            uniform.Set(_outlineShaderProgram);
        }
        _outlineDrawable.Draw();

        UnbindOutline();
    }
}