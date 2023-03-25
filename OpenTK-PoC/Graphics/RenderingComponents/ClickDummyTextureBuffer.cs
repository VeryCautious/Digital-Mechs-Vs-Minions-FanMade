using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal class ClickDummyTextureBuffer
{
    private int _width;
    private int _height;

    private int _frameBuffer;
    private int _renderedTexture;

    public ClickDummyTextureBuffer(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Load()
    {
        _frameBuffer = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);

        _renderedTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _renderedTexture);
        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            _width,
            _height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            Enumerable.Repeat(.5f, _width * _height).ToArray());

        GL.GenerateTextureMipmap(_renderedTexture);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _renderedTexture, 0);
        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

        Unbind();
    }

    public void Resize(Vector2i size)
    {
        _width = size.X;
        _height = size.Y;
        Load();
    }

    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, _renderedTexture);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);
    }

    public void Unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public Color4 GetColorAt(Vector2i pixelCoordinates)
    {
        var x = pixelCoordinates.X;
        var y = _height - 1 - pixelCoordinates.Y;

        var pixel = new byte[4];
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);
        GL.ReadPixels(x, y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, pixel);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        return new Color4(pixel[0], pixel[1], (int)pixel[2], 0xFF); // int-cast is necessary!
    }

    public void SaveToImage(string filePath)
    {
        byte[] pc = new byte[4 * _width * _height];
        GL.ReadPixels(0, 0, _width, _height, PixelFormat.Rgba, PixelType.UnsignedByte, pc);

        var bm = new Bitmap(_width, _height);
        var savePath = Path.GetInvalidPathChars().
            Aggregate(filePath, (current, invalidPathChar) => current.Replace(invalidPathChar, 'c'));

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var r = pc[4 * ((_height - 1 - y) * _width + x)];
                var g = pc[4 * ((_height - 1 - y) * _width + x) + 1];
                var b = pc[4 * ((_height - 1 - y) * _width + x) + 2];
                var a = pc[4 * ((_height - 1 - y) * _width + x) + 3];
                bm.SetPixel(x, y, Color.FromArgb(a, r, g, b));
            }
        }

        var fs = File.OpenWrite(savePath);
        bm.Save(fs, ImageFormat.Png);
        fs.Close();
        bm.Dispose();
    }
}
