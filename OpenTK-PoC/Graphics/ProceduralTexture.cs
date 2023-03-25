using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Mechs_Vs_Minions_Graphics.Graphics;

internal class ProceduralTexture
{
    private int _frameBuffer;
    private int _renderedTexture;
    private int _vertexBuffer;

    private readonly int _width;
    private readonly int _height;
    private readonly ShaderProgramComponent _program;

    public ProceduralTexture(int width, int height, ShaderProgramComponent program)
    {
        _width = width;
        _height = height;
        _program = program;
    }

    private static readonly float[] FullScreenTriangles = new float[] {
        -1.0f, -1.0f, 0.0f,
         1.0f, -1.0f, 0.0f,
        -1.0f,  1.0f, 0.0f,
        -1.0f,  1.0f, 0.0f,
         1.0f, -1.0f, 0.0f,
         1.0f,  1.0f, 0.0f,
    };

    public void Generate(string targetFilePath)
    {
        Init();
        Draw();
        SaveImage(targetFilePath);
        Unbind();
    }

    private void Init()
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
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _renderedTexture, 0);
        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

        _program.Load();

        _vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * FullScreenTriangles.Length, FullScreenTriangles, BufferUsageHint.StaticDraw);
    }

    private void Draw()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);
        GL.Viewport(0, 0, _width, _height);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _program.Bind();

        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    private void SaveImage(string targetFilePath)
    {
        var pc = new byte[4 * _width * _height];
        GL.ReadPixels(0, 0, _width, _height, PixelFormat.Rgba, PixelType.UnsignedByte, pc);

        Console.WriteLine(pc.Distinct().Count());

        var bm = new Bitmap(_width, _height);
        var savePath = Path.GetInvalidPathChars().
            Aggregate(targetFilePath, (current, invalidPathChar) => current.Replace(invalidPathChar, 'c'));

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

    private void Unbind()
    {
        _program.Unbind();
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
}