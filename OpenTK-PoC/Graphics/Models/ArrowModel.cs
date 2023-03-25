using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Overlays;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;
internal abstract class ArrowModel : OverlayModel
{
    private const string TexturePath = "Models/arrow.png";

    protected ArrowModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet, float rotation) : base(
        texturePath: TexturePath,
        textureStore,
        overlayUniformSet,
        Matrix4.CreateRotationZ(rotation)
    )
    { }

    public OverlaySceneGraphRectangle GetRectData => OverlaySceneGraphRectangle.Default with
    {
        HwRatio = TextureHeight / (float)TextureWidth
    };
}

internal class DownArrowModel : ArrowModel
{
    public DownArrowModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) :
        base(textureStore, overlayUniformSet, 0.0f)
    {
    }
}

internal class RightArrowModel : ArrowModel
{
    public RightArrowModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) : 
        base(textureStore, overlayUniformSet, MathF.PI / 2.0f)
    {
    }
}

internal class LeftArrowModel : ArrowModel
{
    public LeftArrowModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) :
        base(textureStore, overlayUniformSet, - MathF.PI / 2.0f)
    {
    }
}

internal class TopArrowModel : ArrowModel
{
    public TopArrowModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) :
        base(textureStore, overlayUniformSet, MathF.PI)
    {
    }
}
