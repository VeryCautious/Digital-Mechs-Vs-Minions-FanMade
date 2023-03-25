using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;

internal struct PrimitiveVertex : IVertex<PrimitiveVertex>
{
    public readonly Vector3 Position;
    public readonly Color4 Color;

    public PrimitiveVertex(Vector3 position, Color4 color)
    {
        Position = position;
        Color = color;
    }

    public IEnumerable<VertexAttribute<PrimitiveVertex>> GetAttributes()
    {
        return new[]
        {
            new VertexAttribute<PrimitiveVertex>("vPosition", typeof(Vector3), VertexAttribPointerType.Float, 0),
            new VertexAttribute<PrimitiveVertex>("vColor", typeof(Vector4), VertexAttribPointerType.Float, 12)
        };
    }
}
