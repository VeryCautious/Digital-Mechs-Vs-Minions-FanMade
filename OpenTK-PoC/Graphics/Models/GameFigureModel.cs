using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal abstract class GameFigureModel<T> : RenderModel<T> where T : PositionalInstance
{
    private const string Dir = "Toaster";
    // private const string dir = "Production";
    private const string TextureSampler = "textureSampler";
    private const string NormalSampler = "normalSampler";

    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    protected string FullTexturePath;
    protected string FullNormalMapPath;
    protected string FullModelPath;

    public GameFigureModel(
        ITexturesStore textureStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData,
        string texturePath,
        string normalMapPath,
        string modelPath,
        Matrix4 baseTransform
    ) : base(
        ConstructRenderModelComponents(textureStore, objStore, GetFullPath(texturePath), GetFullPath(normalMapPath), GetFullPath(modelPath)),
        ConstructRenderModelParameters(baseTransform)
    )
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
        FullTexturePath = GetFullPath(texturePath);
        FullNormalMapPath = GetFullPath(normalMapPath);
        FullModelPath = GetFullPath(modelPath);
    }

    private static string GetFullPath(string pathEnd) => Path.Combine("Models", Dir, pathEnd);

    private static RenderComponents ConstructRenderModelComponents(
        ITexturesStore texturesStore,
        IObjStore objStore,
        string texturePath,
        string normalMapPath,
        string modelPath
    )
    {
        var shaderProgram = ShaderProgramFactory.CreateNormalMapShaderProgram();

        var geometryComponent = new TexturedVertexGeometryComponent(shaderProgram, objStore, modelPath);

        var texture = new Texture(
            shaderProgram,
            TextureSampler,
            texturesStore,
            texturePath,
            TextureUnit.Texture0
        );

        var normalTexture = new Texture(
            shaderProgram,
            NormalSampler,
            texturesStore,
            normalMapPath,
            TextureUnit.Texture1
        );

        return new RenderComponents(
            geometryComponent,
            shaderProgram,
            new IRenderModelComponent[] { texture, normalTexture }
        );
    }

    private static RenderParameters ConstructRenderModelParameters(Matrix4 baseTransform)
    {
        return new RenderParameters(baseTransform, false);
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(T instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}
