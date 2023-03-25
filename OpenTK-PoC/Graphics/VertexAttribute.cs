using OpenTK.Graphics.OpenGL4;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using System.Runtime.InteropServices;

namespace Mechs_Vs_Minions_Graphics.Graphics;

internal sealed class VertexAttribute<TVertex>
    where TVertex : struct
{
    private readonly string _name;
    private readonly int _size;
    private readonly VertexAttribPointerType _type;
    private readonly bool _normalize;
    private readonly int _stride;
    private readonly int _offset;

    public VertexAttribute(string name, Type attributeType, VertexAttribPointerType type, int offset, bool normalize = false)
    {
        _name = name;
        _type = type;
        _normalize = normalize;
        _offset = offset;
        _size = Marshal.SizeOf(attributeType) / 4;
        _stride = Marshal.SizeOf(typeof(TVertex));
    }

    public void Set(ShaderProgramComponent program)
    {
        int index = program.GetAttributeLocation(_name);

        GL.EnableVertexAttribArray(index);
        GL.VertexAttribPointer(index, _size, _type, _normalize, _stride, _offset);
    }
}
