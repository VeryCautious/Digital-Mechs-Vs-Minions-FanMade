using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class SkyBoxModel : RenderModel<PositionalInstance>
{
    private const string TextureSampler = "textureSampler";
    
    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;
    public SkyBoxModel(ITexturesStore texturesStore, ICameraUniformData cameraUniformData) : base(ConstructRenderModelComponents(texturesStore), ConstructRenderModelParameters())
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData, true);
    }

    private static RenderParameters ConstructRenderModelParameters()
    {
        return new RenderParameters(Matrix4.Identity, true);
    }

    private static RenderComponents ConstructRenderModelComponents(ITexturesStore texturesStore)
    {
        var shaderProgram = ShaderProgramFactory.CreateSkyBoxShaderProgram();

        var primitiveVertices = new List<PrimitiveVertex>();

        var vertexPos = new[]
        {
            new Vector3(-1.0f,-1.0f,1.0f),
            new Vector3(1.0f,-1.0f,1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(1.0f,1.0f,1.0f),
            new Vector3(1.0f,-1.0f,1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(-1.0f,-1.0f,-1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,-1.0f),

            new Vector3(1.0f,1.0f,-1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,-1.0f),


            new Vector3(1.0f, -1.0f,-1.0f),
            new Vector3(1.0f, 1.0f,-1.0f),
            new Vector3(1.0f, -1.0f,1.0f),

            new Vector3(1.0f, 1.0f,1.0f),
            new Vector3(1.0f, 1.0f,-1.0f),
            new Vector3(1.0f, -1.0f,1.0f),

            new Vector3(-1.0f, -1.0f,-1.0f),
            new Vector3(-1.0f, 1.0f,-1.0f),
            new Vector3(-1.0f, -1.0f,1.0f),

            new Vector3(-1.0f, 1.0f,1.0f),
            new Vector3(-1.0f, 1.0f,-1.0f),
            new Vector3(-1.0f, -1.0f,1.0f),


            new Vector3(-1.0f,1.0f,-1.0f),
            new Vector3(1.0f,1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(1.0f,1.0f,1.0f),
            new Vector3(1.0f,1.0f,-1.0f),
            new Vector3(-1.0f,1.0f,1.0f),

            new Vector3(-1.0f,-1.0f,-1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,-1.0f,1.0f),

            new Vector3(1.0f,-1.0f,1.0f),
            new Vector3(1.0f,-1.0f,-1.0f),
            new Vector3(-1.0f,-1.0f,1.0f)
        };

        primitiveVertices.AddRange(vertexPos.Select(pos => new PrimitiveVertex(pos, Color4.Red)));

        var texturePaths = new[]
        {
            "Models/CubeMap/right.jpg",
            "Models/CubeMap/left.jpg",
            "Models/CubeMap/bottom.jpg",
            "Models/CubeMap/top.jpg",
            "Models/CubeMap/front.jpg",
            "Models/CubeMap/back.jpg"
        };

        var geometryComponent = new ManualGeometryComponent<PrimitiveVertex>(shaderProgram, primitiveVertices.ToArray(), null);
        var texture = new CubeMap(
            shaderProgram,
            TextureSampler,
            texturesStore,
            texturePaths,
            TextureUnit.Texture0
        );

        return new RenderComponents(geometryComponent, shaderProgram, new IRenderModelComponent[] { texture });
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(PositionalInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}