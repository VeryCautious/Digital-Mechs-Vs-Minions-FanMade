using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal class TexturedVertexGeometryComponent : GeometryComponent<TexturedVertexForNormalMap>
{
    private readonly IObjStore _objStore;
    private readonly string _objFilePath;

    public TexturedVertexGeometryComponent(ShaderProgramComponent shaderProgram, IObjStore objStore, string objFilePath)
        : base(shaderProgram)
    {
        _objStore = objStore;
        _objFilePath = objFilePath;
    }

    public override void Load()
    {
        _objStore.Add(_objFilePath);

        var triangles = _objStore.Get(_objFilePath);

        foreach (var triangle in triangles)
        {
            foreach (var vertex in triangle.AsList())
            {
                IndexBuffer.AddIndex(VertexBuffer.Count);
                VertexBuffer.AddVertex(new TexturedVertexForNormalMap(vertex.Position, vertex.TextureCoordinate, vertex.Normal, vertex.Tangent, vertex.Bitangent));
            }
        }

        VertexArray.SetAttributes(
            VertexBuffer,
            ShaderProgram,
            new VertexAttribute<TexturedVertexForNormalMap>("vPosition", typeof(Vector3), VertexAttribPointerType.Float, 0),
            new VertexAttribute<TexturedVertexForNormalMap>("vUv", typeof(Vector2), VertexAttribPointerType.Float, 12),
            new VertexAttribute<TexturedVertexForNormalMap>("vNormal", typeof(Vector3), VertexAttribPointerType.Float, 20),
            new VertexAttribute<TexturedVertexForNormalMap>("vTangent", typeof(Vector3), VertexAttribPointerType.Float, 32),
            new VertexAttribute<TexturedVertexForNormalMap>("vBitangent", typeof(Vector3), VertexAttribPointerType.Float, 44)
        );

        VertexBuffer.Bind();
        VertexArray.Bind();
        IndexBuffer.Bind();
        IndexBuffer.BufferData();
        VertexBuffer.BufferData();
    }

}
