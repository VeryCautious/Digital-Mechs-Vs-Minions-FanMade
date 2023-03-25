using System.Collections.Immutable;
using System.Diagnostics;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using ObjLoader.Loader.Data.Elements;
using ObjVertexData = ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

public class ObjFileLoader
{
    private readonly string _modelFilePath;

    public ObjFileLoader(string modelFilePath)
    {
        _modelFilePath = modelFilePath;
    }


    public IImmutableList<Triangle> LoadFirstGroup()
    {
        var res = Load(_modelFilePath);

        PrintInfo(res);

        EnsureNormals(res);

        var faces = res.Groups[0].Faces;
        if (faces == null) return ImmutableList<Triangle>.Empty;

        return faces
            .SelectMany(face => TrianglesFrom(face, res))
            .ToImmutableList();
    }
    private static LoadResult Load(string modelFilePath)
    {
        var f = new ObjLoaderFactory();
        var l = f.Create(new MaterialNullStreamProvider());
        var s = new FileStream(modelFilePath, FileMode.Open, FileAccess.Read);
        return l.Load(s);
    }

    private void PrintInfo(LoadResult res)
    {
        if (res == null) throw new Exception("Call Init first");
        
        Debug.WriteLine(_modelFilePath);
        var s = new Stopwatch();
        s.Start();
        Debug.WriteLine($"Normals: {res.Normals.Count}");
        var maxX = res.Vertices.Select(v => v.X).Max();
        var minX = res.Vertices.Select(v => v.X).Min();
        var maxY = res.Vertices.Select(v => v.Y).Max();
        var minY = res.Vertices.Select(v => v.Y).Min();
        var maxZ = res.Vertices.Select(v => v.Z).Max();
        var minZ = res.Vertices.Select(v => v.Z).Min();
        Debug.WriteLine($"X: [{minX},{maxX}]");
        Debug.WriteLine($"Y: [{minY},{maxY}]");
        Debug.WriteLine($"Z: [{minZ},{maxZ}]");
        Debug.WriteLine($"middle: {(maxX + minX) / 2.0f} {(maxY + minY) / 2.0f} {(maxZ + minZ) / 2.0f}");
        Debug.WriteLine($"dims: {maxX - minX} {maxY - minY} {maxZ - minZ}");
        s.Stop();
        Debug.WriteLine($"Debug-output took {s.ElapsedMilliseconds}ms");
    }

    private static void EnsureNormals(LoadResult res)
    {
        if (res.Normals.Count > 0) return;

        var adjFaceIndicesPerVertex = GetAdjacentFaceIndicesPerVertex(res);

        var faceNormals = res.Groups[0].Faces.Select(face => CalcFaceNormal(face, res)).ToList();

        res.Normals = Enumerable.Range(0, res.Vertices.Count)
            .Select(vIndex => CalcVertexNormal(adjFaceIndicesPerVertex[vIndex], faceNormals))
            .Select(normalVector => NormalFromVector(normalVector))
            .ToList();
    }

    private static List<int>[] GetAdjacentFaceIndicesPerVertex(LoadResult res)
    {
        var adjFaceIndices = new List<int>[res.Vertices.Count].Select(_ => new List<int>()).ToArray();
        var faces = res.Groups[0].Faces;
        for (var fIndex = 0; fIndex < faces.Count; fIndex++)
        {
            var face = faces[fIndex];
            var vertexIndicesInRes = Enumerable.Range(0, face.Count)
                .Select(i => face[i].VertexIndex - 1)
                .ToArray();

            foreach (var vi in vertexIndicesInRes) adjFaceIndices[vi].Add(fIndex);
        }
        return adjFaceIndices;
    }

    private static Vector3 CalcFaceNormal(Face face, LoadResult res)
    {
        var vertexIndicesInRes = Enumerable.Range(0, face.Count)
                .Select(i => face[i].VertexIndex - 1)
                .ToArray();

        var vPos = Enumerable.Range(0, 3)
            .Select(i => VectorFromVertex(res.Vertices[vertexIndicesInRes[i]]))
            .ToArray();

        var a = vPos[1] - vPos[0];
        var b = vPos[2] - vPos[0];

        return Vector3.Cross(a, b);
    }

    private static Vector3 CalcVertexNormal(List<int> adjFaceIndices, List<Vector3> faceNormals)
    {
        return adjFaceIndices
            .Select(fIndex => faceNormals[fIndex])
            .Aggregate(
                Vector3.Zero,
                (sum, vec) => sum + vec
            )
            .Normalized();
    }

    private static IImmutableList<Triangle> TrianglesFrom(Face face, LoadResult res)
    {
        var triangles = new List<Triangle>();
        for (var i = 0; i+2 < face.Count; i++)
        {
            triangles.Add(ToTriangle(face, res, new[] { i + 1, i + 2, 0 }));
        }
        return triangles.ToImmutableList();
    }

    private static Triangle ToTriangle(Face face, LoadResult res, IEnumerable<int> indices)
    {
        var indexArray = indices.ToArray();
        var vertexList = new List<Vertex>();

        var (vPositions, vTexture, vNormal) = LoadParametersFrom(face, res, indexArray);

        var (tangent, biTangent) = CalculateTangents(vPositions, vTexture);

        for (var i = 0; i < indexArray.Length; i++)
        {
            vertexList.Add(new Vertex(vPositions[i], vTexture[i], vNormal[i], tangent, biTangent));
        }

        return new Triangle(vertexList[0], vertexList[1], vertexList[2]);
    }

    private static (Vector3[] vPositions, Vector2[] vTexture, Vector3[] vNormal) LoadParametersFrom(
        Face face,
        LoadResult res,
        int[] indexArray
    )
    {
        var vPositions = new Vector3[3];
        var vTexture = new Vector2[3];
        var vNormal = new Vector3[3];

        for (var i = 0; i < indexArray.Length; i++)
        {
            var faceVertex = face[indexArray[i]];
            var v = res.Vertices[faceVertex.VertexIndex - 1];
            var n = res.Normals[GetNormalIndex(faceVertex) - 1];
            var t = res.Textures[faceVertex.TextureIndex - 1];

            vPositions[i] = VectorFromVertex(v);
            vNormal[i] = VectorFromNormal(n);
            vTexture[i] = new Vector2(t.X, t.Y);
        }

        return (vPositions, vTexture, vNormal);
    }

    /// <summary>
    /// Returns a valid normal index. If the actual normal index in the face vertex is not set (=0) then 
    /// vertex normals were calculated before by <c>EnsureNormals</c>. In this case, the normal index
    /// is equal to the vertex index of the face vertex.
    /// </summary>
    private static int GetNormalIndex(FaceVertex v) => v.NormalIndex != 0 ? v.NormalIndex : v.VertexIndex;

    private static (Vector3 tangent, Vector3 biTangent) CalculateTangents(Vector3[] vPositions, Vector2[] vTextures)
    {
        // http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-13-normal-mapping/#tangent-and-bitangent
        var deltaPos1 = vPositions[1] - vPositions[0];
        var deltaPos2 = vPositions[2] - vPositions[0];

        var dTex1 = vTextures[1] - vTextures[0];
        var dTex2 = vTextures[2] - vTextures[0];

        var r = 1.0f / (dTex1.X * dTex2.Y - dTex1.Y * dTex2.X);
        var tangent = (deltaPos1 * dTex2.Y - deltaPos2 * dTex1.Y) * r;
        var biTangent = (deltaPos2 * dTex1.X - deltaPos1 * dTex2.X) * r;

        tangent.Normalize();
        biTangent.Normalize();

        return (tangent, biTangent);
    }

    private static Vector3 VectorFromVertex(ObjVertexData.Vertex v) => new Vector3(v.X, v.Y, v.Z);
    private static Vector3 VectorFromNormal(ObjVertexData.Normal n) => new Vector3(n.X, n.Y, n.Z);

    private static ObjVertexData.Normal NormalFromVector(Vector3 v) => new ObjVertexData.Normal(v.X, v.Y, v.Z);
}