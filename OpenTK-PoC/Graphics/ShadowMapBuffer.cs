using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Mechs_Vs_Minions_Graphics.Graphics;

internal class ShadowMapBuffer
{
    
    private int _frameBuffer;
    private Matrix4 _oldProj;
    private string _oldCamera = CameraManager.DefaultPoseName;
    private readonly LightUniformSet _lightUniformSet;
    private readonly CameraUniformSet _cameraUniformSet;
    private readonly CameraManager _cameraManager;

    public ShadowMapBuffer(LightUniformSet lightUniformSet, CameraUniformSet cameraUniformSet, CameraManager cameraManager)
    {
        _lightUniformSet = lightUniformSet;
        _cameraUniformSet = cameraUniformSet;
        _cameraManager = cameraManager;
    }

    public void Load()
    {
        var sp = ShaderProgramFactory.CreatePhongShaderProgram();
        sp.Load();
        var loc = sp.GetUniformLocation(ShadowMapTextureBuffer.ShadowSamplerName);

        _frameBuffer = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);

        ShadowMapTextureBuffer.GlobalShadowMaps = new ShadowMapTextureBuffer(loc, ShadowMapTextureBuffer.Size.X, ShadowMapTextureBuffer.Size.Y, TextureUnit.Texture1);
        ShadowMapTextureBuffer.GlobalShadowMaps.Load();
        ShadowMapTextureBuffer.GlobalShadowMaps.Bind();

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, ShadowMapTextureBuffer.GlobalShadowMaps.Handle, 0);

        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    public void Bind(Vector3 lightPosition)
    {
        _cameraUniformSet.CameraPositionUniform.Vector = lightPosition;
        _cameraUniformSet.ViewUniform.Matrix = Matrix4.LookAt(lightPosition, lightPosition + Vector3.UnitY, Vector3.UnitZ);
        _lightUniformSet.ViewUniform.Matrix = _cameraUniformSet.ViewUniform.Matrix;

        _oldProj = _cameraUniformSet.ProjectionUniform.Matrix;
        _oldCamera = _cameraManager.CurrentPoseName;
        var newProj = Matrix4.CreatePerspectiveFieldOfView(2.0f, ShadowMapTextureBuffer.Size.X / (float)ShadowMapTextureBuffer.Size.Y, 0.1f, 100f);
        _lightUniformSet.ProjectionUniform.Matrix = newProj;
        _cameraUniformSet.ProjectionUniform.Matrix = newProj;

        GL.Viewport(0, 0, ShadowMapTextureBuffer.Size.X, ShadowMapTextureBuffer.Size.Y);
        ShadowMapTextureBuffer.GlobalShadowMaps.Bind();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);

        GL.Enable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void Unbind()
    {
        ShadowMapTextureBuffer.GlobalShadowMaps.Bind();
        GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 0, 0, ShadowMapTextureBuffer.Size.X, ShadowMapTextureBuffer.Size.Y);
        GL.GenerateTextureMipmap(ShadowMapTextureBuffer.GlobalShadowMaps.Handle);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _cameraUniformSet.ProjectionUniform.Matrix = _oldProj;
        _cameraManager.CurrentPoseName = _oldCamera;
    }

    public void SaveToFile()
    {
        var pixels = new float[ShadowMapTextureBuffer.Size.X * ShadowMapTextureBuffer.Size.Y];
        GL.ReadPixels(0, 0, ShadowMapTextureBuffer.Size.X, ShadowMapTextureBuffer.Size.Y, PixelFormat.DepthComponent, PixelType.Float, pixels);

        var min = pixels.Min();
        var max = pixels.Max();

        if (Math.Abs(min - max) < 0.0000001)
        {
            min = 0;
            max = 1;
        }

#pragma warning disable CA1416
        var bm = new Bitmap(ShadowMapTextureBuffer.Size.X, ShadowMapTextureBuffer.Size.Y);
        var savePath = Path.GetInvalidPathChars().
            Aggregate($"saves/ShadowMap_{DateTime.Now.ToLongTimeString()}.png", (current, invalidPathChar) => current.Replace(invalidPathChar, 'c')).
            Replace(':', '-');

        for (var y = 0; y < ShadowMapTextureBuffer.Size.Y; y++)
        {
            for (var x = 0; x < ShadowMapTextureBuffer.Size.X; x++)
            {
                var pf = pixels[y * 500 + x];
                var p = (int)(Math.Min(Math.Max((pf - min) / (max-min), 0), 1) * 255);
                bm.SetPixel(x, ShadowMapTextureBuffer.Size.Y - (y+1), Color.FromArgb(255, p, p, p));
            }

            Console.WriteLine();
        }

        var fs = File.OpenWrite(savePath);
        bm.Save(fs, ImageFormat.Png);
        fs.Close();
        bm.Dispose();
#pragma warning restore CA1416
    }
}