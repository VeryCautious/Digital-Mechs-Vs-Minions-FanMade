using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;

internal struct TexturedVertex : IVertex<TexturedVertex>
{
    public readonly Vector3 Position;
    public readonly Vector2 Uv;
    public readonly Vector3 Normal;

    public TexturedVertex(Vector3 position, Vector2 uv, Vector3 normal)
    {
        Position = position;
        Uv = uv;
        Normal = normal;
    }

    public IEnumerable<VertexAttribute<TexturedVertex>> GetAttributes()
    {
        return new[]
        {
            new VertexAttribute<TexturedVertex>("vPosition", typeof(Vector3), VertexAttribPointerType.Float, 0),
            new VertexAttribute<TexturedVertex>("vUv", typeof(Vector2), VertexAttribPointerType.Float, 12),
            new VertexAttribute<TexturedVertex>("vNormal", typeof(Vector3), VertexAttribPointerType.Float, 20)
        };
    }
}

internal struct TexturedVertexForNormalMap
{
    public readonly Vector3 Position;
    public readonly Vector2 Uv;
    public readonly Vector3 Normal;
    public readonly Vector3 Tangent;
    public readonly Vector3 Bitangent;

    public TexturedVertexForNormalMap(Vector3 position, Vector2 uv, Vector3 normal, Vector3 tangent, Vector3 bitangent)
    {
        Position = position;
        Uv = uv;
        Normal = normal;
        Tangent = tangent; 
        Bitangent = bitangent;
    }
}
