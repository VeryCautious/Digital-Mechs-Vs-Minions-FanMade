using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Overlays;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;
internal class CardModel : OverlayModel
{
    protected CardModel(string texturePath, ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(texturePath, texturesStore, overlayUniformSet)
    {
    }

    public static Type GetCardModelType(Card playableCard)
    {
        return playableCard switch
        {
            OmniStompCard => typeof(OmnistompCardModel),
            BlazeCard => typeof(BlazeCardModel),
            ScytheCard => typeof(ScytheCardModel),
            RipsawCard => typeof(RipsawCardModel),
            SpeedCard => typeof(SpeedCardModel),
            AimBotCard => typeof(AimBotCardModel),
            GlitchCard => typeof(DamageCardModel),
            StuckControlsCard => typeof(DamageCardModel),
            _ => throw new NotImplementedException()
        };
    }

    private static float GetHwRatio(Card card, ITexturesStore texturesStore)
    {
        var texturePath = card switch
        {
            OmniStompCard => OmnistompCardModel.TexturePath,
            BlazeCard => BlazeCardModel.TexturePath,
            ScytheCard => ScytheCardModel.TexturePath,
            RipsawCard => RipsawCardModel.TexturePath,
            SpeedCard => SpeedCardModel.TexturePath,
            AimBotCard => AimBotCardModel.TexturePath,
            StuckControlsCard => DamageCardModel.TexturePath,
            GlitchCard => DamageCardModel.TexturePath,
            _ => throw new NotImplementedException()
        };
        var texture = texturesStore.GetTexture(texturePath);
        return texture.Height / (float)texture.Width;
    }

    public static OverlaySceneGraphRectangle GetRectData(Card card, ITexturesStore texturesStore) 
        => OverlaySceneGraphRectangle.Default with { HwRatio = GetHwRatio(card, texturesStore) };

}

internal class DamageCardModel : CardModel
{
    public const string TexturePath = "Models/Cards/damage.png";
    public DamageCardModel(ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        texturesStore,
        overlayUniformSet
    )
    { }
}

internal class AimBotCardModel : CardModel
{
    public const string TexturePath = "Models/Cards/aimbot-card.png";
    public AimBotCardModel(ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        texturesStore,
        overlayUniformSet
    )
    { }
}

internal class RipsawCardModel : CardModel
{
    public const string TexturePath = "Models/Cards/ripsaw-card.png";
    public RipsawCardModel(ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        texturesStore,
        overlayUniformSet
    )
    { }
}

internal class BlazeCardModel : CardModel
{
    public const string TexturePath = "Models/Cards/blaze-card.png";
    public BlazeCardModel(ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        texturesStore,
        overlayUniformSet
    )
    { }
}

internal class OmnistompCardModel : CardModel
{
    public const string TexturePath = "Models/Cards/omnistomp-card.png";
    public OmnistompCardModel(ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        texturesStore,
        overlayUniformSet
    )
    { }
}

internal class SpeedCardModel : CardModel
{
    public const string TexturePath = "Models/Cards/speed-card.png";
    public SpeedCardModel(ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        texturesStore,
        overlayUniformSet
    )
    { }
}

internal class ScytheCardModel : CardModel
{
    public const string TexturePath = "Models/Cards/scythe-card.png";
    public ScytheCardModel(ITexturesStore texturesStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        texturesStore,
        overlayUniformSet
    )
    { }
}
