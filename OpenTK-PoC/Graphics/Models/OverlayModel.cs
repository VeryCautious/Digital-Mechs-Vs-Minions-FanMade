using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal abstract class OverlayModel : RenderModel<OverlayInstance>
{
    protected readonly ITexturesStore TexturesStore;
    protected readonly int TextureWidth;
    protected readonly int TextureHeight;
    private readonly OverlayUniformSet _overlayUniformSet;

    protected OverlayModel(
        string texturePath,
        ITexturesStore textureStore,
        OverlayUniformSet overlayUniformSet,
        Matrix4 baseTransform
    ) : base(
        ConstructRenderModelComponents(texturePath, textureStore),
        ConstructRenderModelParameters(baseTransform)
    ) {
        var image = textureStore.GetTexture(texturePath);
        TextureWidth = image.Width;
        TextureHeight = image.Height;
        TexturesStore = textureStore;
        _overlayUniformSet = overlayUniformSet;
    }

    protected OverlayModel(
        string texturePath,
        ITexturesStore textureStore,
        OverlayUniformSet overlayUniformSet
    ) : this(
        texturePath,
        textureStore,
        overlayUniformSet,
        Matrix4.Identity
    ) { }

    private static RenderParameters ConstructRenderModelParameters(Matrix4 baseTransform) => new(baseTransform, true);

    private static RenderComponents ConstructRenderModelComponents(string texturePath, ITexturesStore textureStore)
    {
        var shaderProgram = ShaderProgramFactory.CreateOverlayShaderProgram();
        textureStore.AddTexture(texturePath);
        var image = textureStore.GetTexture(texturePath);

        var texture = new Texture(shaderProgram, "texture0", textureStore, texturePath, TextureUnit.Texture0);
        var hwRatio = (float) image.Height / image.Width;

        var texturedVertices = new List<TexturedVertex>
        {
            new(new Vector3(-1, -hwRatio, -1), new Vector2(0.0f, 0.0f), new Vector3()),
            new(new Vector3(-1, hwRatio, -1), new Vector2(0.0f, 1.0f), new Vector3()),
            new(new Vector3(1, -hwRatio, -1), new Vector2(1.0f, 0.0f), new Vector3()),
            new(new Vector3(1, hwRatio, -1), new Vector2(1.0f, 1.0f), new Vector3())
        };

        int[] indexList = { 0, 1, 2, 1, 2, 3 };

        ManualGeometryComponent<TexturedVertex> geometryComponent = new(shaderProgram, texturedVertices.ToArray(), indexList);

        return new RenderComponents(geometryComponent, shaderProgram, new IRenderModelComponent[] { texture });
    }

    protected override void OnBeforeParentLoad()
    {
    }
    protected override IImmutableList<Uniform> GetUniformsFrom(OverlayInstance instance)
    {
        _overlayUniformSet.ModelProjectionUniform.Matrix =
            BaseTransformation * instance.ModelTransform * _overlayUniformSet.ProjectionMatrix;
        return _overlayUniformSet.GetUniforms();
    }
}
