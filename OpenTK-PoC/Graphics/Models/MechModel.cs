using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal abstract class MechModel : GameFigureModel<MechInstance>
{
    private readonly OutlineRendererComponent _outlineRenderer;

    protected MechModel(
        ITexturesStore textureStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData,
        string texturePath,
        string normalMapPath,
        string modelPath,
        Matrix4 baseTransform
    ) : base(
        textureStore,
        objStore,
        cameraUniformData,
        texturePath,
        normalMapPath,
        modelPath,
        baseTransform
    )
    {
        var outlineShaderProgram = ShaderProgramFactory.CreateOutlineShaderProgram();
        var outlineDrawable = new TexturedVertexGeometryComponent(outlineShaderProgram, objStore, FullModelPath);
        _outlineRenderer = new OutlineRendererComponent(Drawable, ShaderProgram, outlineDrawable, outlineShaderProgram);
    }

    protected override void OnBeforeParentLoad()
    {
        _outlineRenderer.Load();
    }

    public override void RenderInstanceWith(MechInstance instance, IEnumerable<Uniform> staticUniforms)
    {
        if (instance.IsOutlined)
        {
            RenderOutlinedInstanceWith(GetUniformsFrom(instance).Union(staticUniforms).ToArray());
            return;
        }

        base.RenderInstanceWith(instance, staticUniforms);
    }

    private void RenderOutlinedInstanceWith(params Uniform[] uniforms)
    {
        _outlineRenderer.RenderOutlinedInstanceWith(uniforms);
    }

}

internal class TristanaModel : MechModel
{
    private const string TexturePath = "Tristana/mech-lila-herz_5k_u1_v1_diffuse.png";
    private const string NormalMapPath = "Tristana/mech-lila-herz_5k_u1_v1_normal.png";
    private const string ModelPath = "Tristana/mech-lila-herz_5k.obj";

    private static readonly Matrix4 BaseTransform =
            Matrix4.CreateTranslation(-0.07828903f, -21.369335f, 0.09f) *
            Matrix4.CreateRotationY(MathF.PI) *
            Matrix4.CreateRotationZ(MathF.PI) *
            Matrix4.CreateTranslation(0.0f, -21.369335f, 0.0f) *
            Matrix4.CreateScale(1f / 36f);
    public TristanaModel(
        ITexturesStore textureStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData
    ) : base(
        textureStore,
        objStore,
        cameraUniformData,
        TexturePath,
        NormalMapPath,
        ModelPath,
        BaseTransform
    ) { }
}

internal class HeimerdingerModel : MechModel
{
    private const string TexturePath = "Heimerdinger/mech-big-brain_5k_u1_v1_diffuse.png";
    private const string NormalMapPath = "Heimerdinger/mech-big-brain_5k_u1_v1_normal.png";
    private const string ModelPath = "Heimerdinger/mech-big-brain_5k.obj";

    private static readonly Matrix4 BaseTransform =
            Matrix4.CreateTranslation(0.8991561f, -16.921568f, 0.16949558f) *
            Matrix4.CreateRotationY(MathF.PI) *
            Matrix4.CreateRotationZ(MathF.PI) *
            Matrix4.CreateTranslation(0.0f, -16.921568f, 0.0f) *
            Matrix4.CreateScale(1f / 30f);
    public HeimerdingerModel(
        ITexturesStore textureStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData
    ) : base(
        textureStore,
        objStore,
        cameraUniformData,
        TexturePath,
        NormalMapPath,
        ModelPath,
        BaseTransform
    )
    { }
}

internal class CorkiModel : MechModel
{
    private const string TexturePath = "Corki/mech-helm-keule_5k_u1_v1_diffuse.png";
    private const string NormalMapPath = "Corki/mech-helm-keule_5k_u1_v1_normal.png";
    private const string ModelPath = "Corki/mech-helm-keule_5k.obj";

    private static readonly Matrix4 BaseTransform =
            Matrix4.CreateRotationY(MathF.PI) *
            Matrix4.CreateRotationZ(MathF.PI) *
            Matrix4.CreateScale(18f);
    public CorkiModel(
        ITexturesStore textureStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData
    ) : base(
        textureStore,
        objStore,
        cameraUniformData,
        TexturePath,
        NormalMapPath,
        ModelPath,
        BaseTransform
    )
    { }
}

internal class ZiggsModel : MechModel
{
    private const string TexturePath = "Ziggs/mech-kugel-rot_5k_u1_v1_diffuse.png";
    private const string NormalMapPath = "Ziggs/mech-kugel-rot_5k_u1_v1_normal.png";
    private const string ModelPath = "Ziggs/mech-kugel-rot_5k.obj";

    private static readonly Matrix4 BaseTransform =
            Matrix4.CreateTranslation(-0.081730366f, -15.009833f, 0.0f) *
            Matrix4.CreateRotationY(MathF.PI) *
            Matrix4.CreateRotationZ(MathF.PI) *
            Matrix4.CreateTranslation(0.0f, -15.009833f, 0.0f) *
            Matrix4.CreateScale(1f / 24f);
    public ZiggsModel(
        ITexturesStore textureStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData
    ) : base(
        textureStore,
        objStore,
        cameraUniformData,
        TexturePath,
        NormalMapPath,
        ModelPath,
        BaseTransform
    )
    { }
}