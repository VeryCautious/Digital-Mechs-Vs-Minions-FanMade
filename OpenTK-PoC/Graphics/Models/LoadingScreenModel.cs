using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Overlays;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class LoadingScreenModel : OverlayModel
{
    public const string TexturePath = "Models/mechs-vs-minions-loading.jpg";
    public LoadingScreenModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) : base(
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