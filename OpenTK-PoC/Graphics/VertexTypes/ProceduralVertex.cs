using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;

internal struct ProceduralVertex : IVertex<ProceduralVertex>
{
    public readonly Vector3 Position;
    public readonly Vector3 Normal;
    public readonly Color4 Color;

    public ProceduralVertex(Vector3 position, Vector3 normal, Color4 color)
    {
        Position = position;
        Normal = normal;
        Color = color;
    }

    public IEnumerable<VertexAttribute<ProceduralVertex>> GetAttributes()
    {
        return new[]
        {
            new VertexAttribute<ProceduralVertex>("vPosition", typeof(Vector3), VertexAttribPointerType.Float, 0),
            new VertexAttribute<ProceduralVertex>("vNormal", typeof(Vector3), VertexAttribPointerType.Float, 12),
            new VertexAttribute<ProceduralVertex>("vColor", typeof(Vector4), VertexAttribPointerType.Float, 24),
        };
    }
}
