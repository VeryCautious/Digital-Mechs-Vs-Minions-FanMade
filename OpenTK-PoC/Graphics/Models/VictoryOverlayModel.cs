using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Overlays;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;
internal class VictoryOverlayModel : OverlayModel
{
    public const string TexturePath = "Models/victory.png";
    public VictoryOverlayModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        textureStore,
        overlayUniformSet
    )
    { }

    public OverlaySceneGraphRectangle GetRectData => OverlaySceneGraphRectangle.Default with
    {
        HwRatio = TextureHeight / (float)TextureWidth
    };
}

