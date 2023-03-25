using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal class ShadowMapTextureBuffer : IRenderModelComponent
{
    public static ShadowMapTextureBuffer GlobalShadowMaps = null!;
    public const string ShadowSamplerName = "shadowSampler";
    public static Vector2i Size = new(500, 500);

    private readonly TextureUnit _textureTarget;
    private readonly int _uniformLocation;
    private readonly int _width;
    private readonly int _height;
    public int Handle { get; private set; }

    public ShadowMapTextureBuffer(int uniformLocation, int width, int height, TextureUnit textureTarget)
    {
        _textureTarget = textureTarget;
        _uniformLocation = uniformLocation;
        _width = width;
        _height = height;
    }

    public void Load()
    {
        Handle = GL.GenTexture();
        Bind();

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.DepthComponent16,
            _width,
            _height,
            0,
            PixelFormat.DepthComponent,
            PixelType.Float,
            Enumerable.Repeat(.5f, _height * _width).ToArray()
        );

        GL.GenerateTextureMipmap(Handle);
    }

    public void Update(float[] newData)
    {
        Bind();

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.DepthComponent16,
            _width,
            _height,
            0,
            PixelFormat.DepthComponent,
            PixelType.Float,
            newData
        );

        GL.GenerateTextureMipmap(Handle);
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
        GL.BindTexture(TextureTarget.Texture2D, Handle);
        GL.Uniform1(_uniformLocation, From(_textureTarget));
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