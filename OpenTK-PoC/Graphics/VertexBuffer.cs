using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics;

internal sealed class VertexBuffer<TVertex> where TVertex : struct
{
    private readonly int _vertexSize;
    private readonly List<TVertex> _vertices;
    private int? _handle;
    private TVertex[]? _verticesArray;

    public VertexBuffer()
    {
        _verticesArray = null;
        _vertices = new List<TVertex>();
        _vertexSize = Marshal.SizeOf(typeof(TVertex));
    }

    private int Handle => GetHandle();

    public int Count => _vertices.Count;

    private int GetHandle()
    {
        _handle ??= GL.GenBuffer();
        return _handle ?? throw new ArgumentNullException();
    }

    public void AddVertex(TVertex v)
    {
        _vertices.Add(v);
    }

    public void AddVertices(IEnumerable<TVertex> vertices)
    {
        _vertices.AddRange(vertices);
    }

    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
    }

    public void BufferData()
    {
        _verticesArray = _vertices.ToArray();
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            (IntPtr)(_vertexSize * _vertices.Count),
            _verticesArray,
            BufferUsageHint.StreamDraw
        );
    }

    public void Draw()
    {
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
    }
}