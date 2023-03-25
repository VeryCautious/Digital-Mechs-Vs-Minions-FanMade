using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using StbImageSharp;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

internal class TexturesStore : ITexturesStore
{
    private readonly Dictionary<string, ImageResult> _textures;

    public TexturesStore()
    {
        _textures = new Dictionary<string, ImageResult>();
    }

    public ImageResult GetTexture(string filepath) => _textures[filepath];

    public bool HasTextureLoaded(string filepath) => _textures.ContainsKey(filepath);

    public void AddTexture(string texturePath)
    {
        if (_textures.ContainsKey(texturePath))
        {
            return;
        }

        var image = LoadImageFromFile(texturePath);
        _textures.Add(texturePath, image);
    }

    public static ImageResult LoadImageFromFile(string texturePath, bool flipY = true)
    {
        StbImage.stbi_set_flip_vertically_on_load(flipY ? 1 : 0);
        return ImageResult.FromStream(File.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);
    }
}
