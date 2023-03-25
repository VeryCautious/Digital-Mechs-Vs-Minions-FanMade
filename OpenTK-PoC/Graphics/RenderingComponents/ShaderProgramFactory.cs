using OpenTK.Graphics.OpenGL4;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal static class ShaderProgramFactory
{
    public static ShaderProgramComponent CreateNormalMapShaderProgram() => CreateDefaultBundle("NormalMap");

    public static ShaderProgramComponent CreatePhongShaderProgram() => CreateDefaultBundle("Phong");

    public static ShaderProgramComponent CreateOutlineShaderProgram() => CreateDefaultBundle("Outline");

    public static ShaderProgramComponent CreateOverlayShaderProgram() => CreateDefaultBundle("Overlay");

    public static ShaderProgramComponent CreateParticleShaderProgram() => CreateDefaultBundle("Particle");

    public static ShaderProgramComponent CreateSkyBoxShaderProgram() => CreateDefaultBundle("SkyBox");

    public static ShaderProgramComponent CreatePrimitiveShader() => CreateDefaultBundle("Primitive");

    public static ShaderProgramComponent CreateRockShader() => CreateDefaultBundle("Rock");

    public static ShaderProgramComponent CreateGameBoardShader() => CreateDefaultBundle("GameBoard");

    public static ShaderProgramComponent CreateGameBoardRockShader() => CreateDefaultBundle("GameBoardRock");

    private static ShaderProgramComponent CreateDefaultBundle(string name)
    {
        var vertexShader = new Shader(ShaderType.VertexShader, File.ReadAllText($"Shaders/{name}Shader.vert"));
        var fragmentShader = new Shader(ShaderType.FragmentShader, File.ReadAllText($"Shaders/{name}Shader.frag"));
        return new ShaderProgramComponent(vertexShader, fragmentShader);
    }

    public static ShaderProgramComponent CreateProceduralSandShader() => CreateProceduralTextureBundle("ProceduralSand");

    private static ShaderProgramComponent CreateProceduralTextureBundle(string fragmentShaderName)
    {
        var vertexShader = new Shader(ShaderType.VertexShader, File.ReadAllText($"Shaders/ProceduralTextureShader.vert"));
        var fragmentShader = new Shader(ShaderType.FragmentShader, File.ReadAllText($"Shaders/{fragmentShaderName}Shader.frag"));
        return new ShaderProgramComponent(vertexShader, fragmentShader);
    }

    public static ShaderProgramComponent CreateSelectableShader()
    {
        var vertexShader = new Shader(ShaderType.VertexShader, File.ReadAllText($"Shaders/SelectableShader.vert"));
        var fragmentShader = new Shader(ShaderType.FragmentShader, File.ReadAllText($"Shaders/PrimitiveShader.frag"));
        return new ShaderProgramComponent(vertexShader, fragmentShader);
    }
}