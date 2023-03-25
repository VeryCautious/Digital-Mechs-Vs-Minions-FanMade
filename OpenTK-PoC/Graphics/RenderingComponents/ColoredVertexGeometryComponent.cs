using ObjLoader.Loader.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using System.Diagnostics;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal class ColoredVertexGeometryComponent : GeometryComponent<PrimitiveVertex>
{
    private readonly string _modelFilePath;

    public ColoredVertexGeometryComponent(ShaderProgramComponent shaderProgram, string modelFilePath)
        : base(shaderProgram)
    {
        _modelFilePath = modelFilePath;
    }

    public override void Load()
    {
        var f = new ObjLoaderFactory();
        var l = f.Create(new MaterialNullStreamProvider());
        var s = new FileStream(_modelFilePath, FileMode.Open);
        var res = l.Load(s);

        foreach (var face in res.Groups.First().Faces)
        {
            Debug.Assert(face.Count == 3);
            for (var i = 0; i < face.Count; i++)
            {
                IndexBuffer.AddIndex(VertexBuffer.Count);

                var v = res.Vertices[face[i].VertexIndex - 1];
                VertexBuffer.AddVertex(new PrimitiveVertex(new Vector3(v.X, v.Y, v.Z), new Color4(v.X, v.Y, v.Z, 1.0f)));
            }
        }

        VertexArray.SetAttributes(
            VertexBuffer,
            ShaderProgram,
            new VertexAttribute<PrimitiveVertex>("vPosition", typeof(Vector3), VertexAttribPointerType.Float, 0),
            new VertexAttribute<PrimitiveVertex>("vColor", typeof(Vector4), VertexAttribPointerType.Float, 12)
        );

        VertexBuffer.Bind();
        VertexArray.Bind();
        IndexBuffer.Bind();
        IndexBuffer.BufferData();
        VertexBuffer.BufferData();
    }
}
