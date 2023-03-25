using OpenTK.Graphics.OpenGL4;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

namespace Mechs_Vs_Minions_Graphics.Graphics;

internal sealed class VertexArray<TVertex>
    where TVertex : struct
{
    private int? _handle;
    private int Handle => GetHandle();

    private int GetHandle()
    {
        if (_handle == null) {
            GL.GenVertexArrays(1, out int handle);
            _handle = handle;
        }
            
        return _handle ?? throw new ArgumentNullException();
    }

    public void SetAttributes(
        VertexBuffer<TVertex> vertexBuffer,
        ShaderProgramComponent program,
        params VertexAttribute<TVertex>[] attributes
    ) {
        Bind();

        vertexBuffer.Bind();

        foreach (var attribute in attributes)
            attribute.Set(program);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Bind() => GL.BindVertexArray(Handle);
}
