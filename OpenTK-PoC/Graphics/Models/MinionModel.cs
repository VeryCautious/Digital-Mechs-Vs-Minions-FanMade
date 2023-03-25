using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal abstract class MinionModel : GameFigureModel<MinionInstance>
{

    public MinionModel(
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
    ) { }

    protected override void OnBeforeParentLoad()
    {
    }
}

internal class BlueMinionModel : MinionModel
{
    private const string TexturePath = "BlueMinion/geist-schild-blau_5k_u1_v1_diffuse.png";
    private const string NormalMapPath = "BlueMinion/geist-schild-blau_5k_u1_v1_normal.png";
    private const string ModelPath = "BlueMinion/geist-schild-blau_5k.obj";

    private static readonly Matrix4 BaseTransform =
            Matrix4.CreateTranslation(0.0001983107f, -0.014110909f, -0.00012652483f) *
            Matrix4.CreateRotationY(MathF.PI) *
            Matrix4.CreateRotationZ(MathF.PI) *
            Matrix4.CreateTranslation(0.0f, -0.014110909f, 0.0f) *
            Matrix4.CreateScale(20f);

    public BlueMinionModel(
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

internal class RedMinionModel : MinionModel
{
    private const string TexturePath = "RedMinion/geist-axt-rot_5k_u1_v1_diffuse.png";
    private const string NormalMapPath = "RedMinion/geist-axt-rot_5k_u1_v1_normal.png";
    private const string ModelPath = "RedMinion/geist-axt-rot_5k.obj";

    private static readonly Matrix4 BaseTransform =
            Matrix4.CreateTranslation(0.0001983107f, -0.014110909f, -0.00012652483f) *
            Matrix4.CreateRotationY(MathF.PI) *
            Matrix4.CreateRotationZ(MathF.PI) *
            Matrix4.CreateTranslation(0.0f, -0.014110909f, 0.0f) *
            Matrix4.CreateScale(20f);

    public RedMinionModel(
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