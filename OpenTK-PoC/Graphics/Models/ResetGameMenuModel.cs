using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class ResetGameMenuModel : OverlayModel
{
    public const string TexturePath = "Models/reset_game.png";
    public ResetGameMenuModel(ITexturesStore textureStore, OverlayUniformSet overlayUniformSet) : base(
        texturePath: TexturePath,
        textureStore,
        overlayUniformSet
    )
    { }
}