using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal sealed class CubeMap : IRenderModelComponent
{
    private readonly ITexturesStore _texturesStore;
    private readonly TextureUnit _textureTarget;
    private readonly ShaderProgramComponent _shaders;
    private readonly string _textureName;
    private readonly string[] _texturePaths;
    private int _handle;

    private int TextureWidth(int i) => Image(i).Width;
    private int TextureHeight(int i) => Image(i).Height;
    private ImageResult Image(int i) => _texturesStore.GetTexture(_texturePaths[i]);

    public CubeMap(ShaderProgramComponent shaders, string textureName, ITexturesStore texturesStore, string[] texturePaths, TextureUnit textureTarget)
    {
        _textureTarget = textureTarget;
        _shaders = shaders;
        _textureName = textureName;
        _texturesStore = texturesStore;
        _texturePaths = texturePaths;
    }

    public void Load()
    {
        foreach (var tex in _texturePaths)
        {
            _texturesStore.AddTexture(tex);
        }

        _handle = GL.GenTexture();
        Bind();

        for (var i = 0; i < _texturePaths.Length; i++)
        {
            GL.TexImage2D(
                TextureTarget.TextureCubeMapPositiveX + i,
                0,
                PixelInternalFormat.Rgba,
                TextureWidth(i),
                TextureHeight(i),
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                Image(i).Data
            );
        }

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
        GL.BindTexture(TextureTarget.TextureCubeMap, _handle);
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