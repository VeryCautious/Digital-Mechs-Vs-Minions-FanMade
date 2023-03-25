
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Overlays;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;
internal class CommandLineModel : OverlayModel
{
    public const string TexturePath = "Models/commandline.png";
    public CommandLineModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        textureStore,
        overlayUniformSet
    )
    { }

    /// <summary>
    /// slot and level start with 1
    /// </summary>
    public OverlaySceneGraphRectangle GetCardPosition(Card card, int slot, int level)
    {
        return CardModel.GetRectData(card, TexturesStore) with
        {
            XTranslate = -0.822f + (slot - 1) * (0.866f + 0.822f) / (6 - 1),
            YTranslate = 0.444f - (level - 1) * 0.222f,
            MaxWidthPercentage = 176.0f / TextureWidth,
            MaxHeightPercentage = 245.0f / TextureHeight,
        };
    }

    public OverlaySceneGraphRectangle GetRectData => OverlaySceneGraphRectangle.Default with { 
        HwRatio = TextureHeight / (float) TextureWidth
    };
}
