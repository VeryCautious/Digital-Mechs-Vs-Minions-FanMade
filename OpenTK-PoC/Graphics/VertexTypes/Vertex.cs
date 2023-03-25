using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;

public sealed record Vertex(Vector3 Position, Vector2 TextureCoordinate, Vector3 Normal, Vector3 Tangent, Vector3 Bitangent);