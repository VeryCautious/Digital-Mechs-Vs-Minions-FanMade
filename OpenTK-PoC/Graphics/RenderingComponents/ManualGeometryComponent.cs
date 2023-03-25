using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents
{
    internal class ManualGeometryComponent<T> : GeometryComponent<T>
        where T : struct, IVertex<T>
    {

        private readonly T[] _vertexList;
        private readonly int[] _indexList;

        public ManualGeometryComponent(
            ShaderProgramComponent shaderProgram,
            T[] vertexList,
            int[]? indexList,
            PrimitiveType primitiveType = PrimitiveType.Triangles
        ) : base(shaderProgram, primitiveType)
        {
            _vertexList = vertexList;
            _indexList = indexList ?? Enumerable.Range(0, _vertexList.Length).ToArray();
        }

        public override void Load()
        {
            VertexBuffer.AddVertices(_vertexList);
            IndexBuffer.AddIndices(_indexList);

            VertexArray.SetAttributes(
                VertexBuffer,
                ShaderProgram,
                _vertexList[0].GetAttributes().ToArray()
            );

            VertexBuffer.Bind();
            VertexArray.Bind();
            IndexBuffer.Bind();
            IndexBuffer.BufferData();
            VertexBuffer.BufferData();
        }
    }
}
