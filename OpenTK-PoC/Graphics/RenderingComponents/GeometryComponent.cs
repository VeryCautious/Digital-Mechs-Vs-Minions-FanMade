using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal abstract class GeometryComponent<TVertex> : IDrawable where TVertex : struct
{

    protected readonly VertexArray<TVertex> VertexArray;
    protected readonly VertexBuffer<TVertex> VertexBuffer;
    protected readonly IndexBuffer IndexBuffer;

    protected readonly ShaderProgramComponent ShaderProgram;

    protected GeometryComponent(ShaderProgramComponent shaderProgram, PrimitiveType primitiveType = PrimitiveType.Triangles)
    {
        ShaderProgram = shaderProgram;
        VertexArray = new VertexArray<TVertex>();
        VertexBuffer = new VertexBuffer<TVertex>();
        IndexBuffer = new IndexBuffer(primitiveType);
    }

    public abstract void Load();

    public void Bind()
    {
        VertexBuffer.Bind();
        VertexArray.Bind();
        IndexBuffer.Bind();
    }

    public void Unbind()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Dispose()
    {
        //TODO DeleteBuffers
    }

    public void Draw()
    {
        IndexBuffer.Draw();
    }
}
