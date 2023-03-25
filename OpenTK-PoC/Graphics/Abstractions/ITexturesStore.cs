using StbImageSharp;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface ITexturesStore
{
    ImageResult GetTexture(string filepath);
    bool HasTextureLoaded(string filepath);
    void AddTexture(string texturePath);
}
