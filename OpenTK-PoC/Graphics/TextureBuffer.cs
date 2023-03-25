using OpenTK.Graphics.OpenGL4;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using StbImageSharp;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

namespace Mechs_Vs_Minions_Graphics.Graphics;

internal sealed class Texture : IRenderModelComponent
{
    private readonly ITexturesStore _texturesStore;
    private readonly TextureUnit _textureTarget;
    private readonly ShaderProgramComponent _shaders;
    private readonly string _textureName;
    private readonly string _texturePath;
    private int _handle;

    public int TextureWidth => Image.Width;
    public int TextureHeight => Image.Height;
    private ImageResult Image => _texturesStore.GetTexture(_texturePath);

    public Texture(ShaderProgramComponent shaders, string textureName, ITexturesStore texturesStore, string texturePath, TextureUnit textureTarget)
    {
        _textureTarget = textureTarget;
        _shaders = shaders;
        _textureName = textureName;
        _texturesStore = texturesStore;
        _texturePath = texturePath;
    }

    public void Load()
    {
        _texturesStore.AddTexture(_texturePath);

        _handle = GL.GenTexture();
        Bind();
        
        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            Image.Width,
            Image.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            Image.Data
        );
        
        GL.GenerateTextureMipmap(_handle);
    }

    private static int From(TextureUnit textureTarget) => textureTarget switch
    {
        TextureUnit.Texture0 => 0,
        TextureUnit.Texture1 => 1,
        TextureUnit.Texture2 => 2,
        _ => throw new NotImplementedException()
    };

    public void Bind()
    {
        GL.ActiveTexture(_textureTarget);
        GL.BindTexture(TextureTarget.Texture2D, _handle);
        GL.Uniform1(_shaders.GetUniformLocation(_textureName), From(_textureTarget));
    }

    public void Unbind()
    {
        GL.ActiveTexture(_textureTarget);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose()
    {
        //TODO delete
    }
}
