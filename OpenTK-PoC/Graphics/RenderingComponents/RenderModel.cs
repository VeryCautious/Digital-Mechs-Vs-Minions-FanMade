using OpenTK.Graphics.OpenGL4;
using System.Collections.Immutable;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal abstract class RenderModel<T>
    where T : Instance
{
    protected readonly ShaderProgramComponent ShaderProgram;
    protected readonly IDrawable Drawable;

    private readonly IImmutableList<IRenderModelComponent> _renderComponents;
    public Matrix4 BaseTransformation { get; }
    public readonly bool HasTransparency;

    protected record RenderComponents(IDrawable Drawable, ShaderProgramComponent ShaderProgram, IRenderModelComponent[] RenderModelComponents);

    protected record RenderParameters(Matrix4 BaseTransformation, bool HasTransparency);

    private RenderModel(IDrawable drawable, ShaderProgramComponent shaderProgram, Matrix4 baseTransformation, bool hasTransparency, params IRenderModelComponent[] renderModelComponents)
    {
        Drawable = drawable;
        ShaderProgram = shaderProgram;
        BaseTransformation = baseTransformation;
        HasTransparency = hasTransparency;
        _renderComponents = renderModelComponents.Append(drawable).Append(shaderProgram).Reverse().ToImmutableList();
    }

    protected RenderModel(RenderComponents renderComponents, RenderParameters renderParameters) :
        this(renderComponents.Drawable, renderComponents.ShaderProgram, renderParameters.BaseTransformation, renderParameters.HasTransparency, renderComponents.RenderModelComponents)
    { }

    public void Load()
    {
        OnBeforeParentLoad();

        foreach (var component in _renderComponents)
            component.Load();
    }

    public RenderModelBindings Bind()
    {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

        RenderModelBindings bindings = new(_renderComponents.ToImmutableList<IBindable>());
        bindings.BindAll();
        return bindings;
    }

    /// <summary>
    /// Executed before the parent has loaded all its components
    /// </summary>
    protected abstract void OnBeforeParentLoad();

    protected abstract IImmutableList<Uniform> GetUniformsFrom(T instance);

    public virtual void RenderInstanceWith(T instance, IEnumerable<Uniform> staticUniforms)
    {
        foreach (var uniform in staticUniforms.Union(GetUniformsFrom(instance)))
        {
            uniform.Set(ShaderProgram);
        }
        Drawable.Draw();
    }
}
