using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class LightModel : RenderModel<MinionInstance>
{
    private const string TexturePath = "Models/minions.png";
    private const string ModelPath = "Models/minions.obj";
    private const string TextureSampler = "textureSampler";
    
    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public LightModel(
        ITexturesStore texturesStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData
    ) : base(ConstructRenderModelComponents(texturesStore, objStore), ConstructRenderModelParameters())
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
    {
        var baseTransform = Matrix4.CreateScale(0.1f) * Matrix4.CreateRotationX(MathF.PI) * Matrix4.CreateTranslation(0f, -0.35f, 0f);
        return new RenderParameters(baseTransform, false);
    }

    private static RenderComponents ConstructRenderModelComponents(ITexturesStore texturesStore, IObjStore objStore)
    {
        var shaderProgram = ShaderProgramFactory.CreatePhongShaderProgram();
        var geometryComponent = new TexturedVertexGeometryComponent(shaderProgram, objStore, ModelPath);
        var texture = new Texture(
            shaderProgram,
            TextureSampler,
            texturesStore,
            TexturePath,
            TextureUnit.Texture0
        );
        
        return new RenderComponents(geometryComponent, shaderProgram, new IRenderModelComponent[] { texture });
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(MinionInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}