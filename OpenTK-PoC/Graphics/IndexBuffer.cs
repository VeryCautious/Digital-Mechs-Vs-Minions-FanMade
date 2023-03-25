using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics;

sealed class IndexBuffer
{
    private readonly PrimitiveType _primitiveType;

    private readonly int _indexSize;
    private readonly List<int> _indices;
    private int[]? _indicesArray;
    private int? _handle;
    private int Handle => GetHandle();

    private int GetHandle()
    {
        _handle ??= GL.GenBuffer();
        return _handle ?? throw new ArgumentNullException();
    }

    public IndexBuffer(PrimitiveType primitiveType = PrimitiveType.Triangles)
    {
        _indices = new();
        _indexSize = 4;
        _primitiveType = primitiveType;
    }

    public void AddIndex(params int[] faceIndices) => _indices.AddRange(faceIndices);

    public void AddIndices(IEnumerable<int> indexEnumerable) =>
        _indices.AddRange(indexEnumerable);

    public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);

    public void BufferData()
    {
        _indicesArray = _indices.ToArray();
        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            (IntPtr)(_indexSize * _indices.Count),
            _indicesArray,
            BufferUsageHint.StaticRead
        );
    }

    public void Draw() => GL.DrawElements(_primitiveType, _indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
}
