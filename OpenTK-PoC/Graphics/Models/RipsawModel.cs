using System.Collections.Immutable;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class RipsawModel : RenderModel<RipSawInstance>
{
    private const string RipsawTexturePath = "Models/Ripsaw.png";
    private const string TextureSampler = "textureSampler";

    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public RipsawModel(ITexturesStore texturesStore, ICameraUniformData cameraUniformData) : base(ConstructRenderModelComponents(texturesStore), ConstructRenderModelParameters())
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
        => new(Matrix4.CreateTranslation(-.5f, 0f, -.5f) * Matrix4.CreateScale(0.6f), true);

    private static RenderComponents ConstructRenderModelComponents(ITexturesStore texturesStore)
    {
        var shaderProgram = ShaderProgramFactory.CreatePhongShaderProgram();

        var primitiveVertices = new List<TexturedVertex>
        {
            new(new Vector3(0, 0, 0), new Vector2(0.0f, 0.0f), new Vector3(0f, -1.0f, 0f)),
            new(new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f), new Vector3(0f, -1.0f, 0f)),
            new(new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f), new Vector3(0f, -1.0f, 0f)),
            new(new Vector3(1, 0, 1), new Vector2(1.0f, 1.0f), new Vector3(0f, -1.0f, 0f)),
            new(new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f), new Vector3(0f, -1.0f, 0f)),
            new(new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f), new Vector3(0f, -1.0f, 0f))
        };

        var geometryComponent = new ManualGeometryComponent<TexturedVertex>(shaderProgram, primitiveVertices.ToArray(), null);
        var texture = new Texture(
            shaderProgram,
            TextureSampler,
            texturesStore,
            RipsawTexturePath,
            TextureUnit.Texture0
        );

        return new RenderComponents(geometryComponent, shaderProgram, new IRenderModelComponent[] { texture });
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(RipSawInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}
