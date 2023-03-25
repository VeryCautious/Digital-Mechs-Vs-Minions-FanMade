using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class GameBoardRockModel : RenderModel<PositionalInstance>
{
    private const string GrassTexturePath = "Models/cracks.jpg";
    private const string TextureSampler = "textureSampler";

    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public static GameBoardRockModel Generate(ITexturesStore texturesStore, ICameraUniformData cameraUniformData) => new GameBoardRockModel(texturesStore, cameraUniformData);

    private GameBoardRockModel(ITexturesStore texturesStore, ICameraUniformData cameraUniformData) : base(ConstructRenderModelComponents(texturesStore), ConstructRenderModelParameters())
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
        => new(Matrix4.Identity, false);

    private static RenderComponents ConstructRenderModelComponents(ITexturesStore texturesStore)
    {
        var shaderProgram = ShaderProgramFactory.CreateGameBoardRockShader();

        var (rock, colors) = GameBoardRockGenerator.Generate();

        var (vertexArray, normalArray, indexArray) = rock.DataWithVertexNormals();

        var primitiveVertices = vertexArray.Zip(normalArray, colors)
            .Select(entry => new ProceduralVertex(entry.First, entry.Second, entry.Third))
            .ToArray();

        var geometryComponent = new ManualGeometryComponent<ProceduralVertex>(shaderProgram, primitiveVertices.ToArray(), indexArray);

        var texture = new Texture(
            shaderProgram,
            TextureSampler,
            texturesStore,
            GrassTexturePath,
            TextureUnit.Texture0
        );

        return new RenderComponents(geometryComponent, shaderProgram, new IRenderModelComponent[] { texture });
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(PositionalInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}

internal class GameBoardRockGenerator
{
    public static (Mesh mesh, List<Color4> colors) Generate() => new GameBoardRockGenerator().GenerateMesh();
    private GameBoardRockGenerator()
    {
    }

    private (Mesh mesh, List<Color4> colors) GenerateMesh()
    {
        var rock = SphereGenerator.Generate(4);

        ApplyXzNoise(rock);

        rock.ProjectToPlane(Vector3.Zero, -Vector3.UnitY);

        ApplyNoiseOnBottomSide(rock, out var colors);

        return (rock, colors);
    }

    private static void ApplyXzNoise(Mesh rock)
    {
        var radialNoise = new Noise3D(4.0f);
        var radialNoiseList = rock.VertexIds
            .Select(rock.GetVertexPosition)
            .Select(v => new Vector3(v.X, 0, v.Z))
            .Select(radialNoise.Generate)
            .ToImmutableList();

        foreach (var (vid, radialNoiseValue) in rock.VertexIds.Zip(radialNoiseList))
        {
            var v = rock.GetVertexPosition(vid);
            var factor = 0.25f * radialNoiseValue + 0.95f;
            rock.UpdateVertex(vid, new Vector3(v.X * factor, v.Y, v.Z * factor));
        }
    }

    private static void ApplyNoiseOnBottomSide(Mesh rock, out List<Color4> colors)
    {
        var noise = new Noise3D(2.0f);
        var smallNoise = new Noise3D(20.0f);

        var noiseList = rock.VertexIds
            .Select(rock.GetVertexPosition)
            .Select(noise.Generate)
            .ToImmutableList();

        var smallNoiseList = rock.VertexIds
            .Select(rock.GetVertexPosition)
            .Select(smallNoise.Generate)
            .ToImmutableList();

        colors = new List<Color4>();
        for (var i = 0; i < rock.VertexIds.Count; i++)
        {
            var vid = rock.VertexIds[i];
            var v = rock.GetVertexPosition(vid);
            var noiseValue = noiseList[i];
            var smallNoiseValue = smallNoiseList[i];
            if (v.Y < 0.01)
            {
                var gray = GetGrayValue(noiseValue, 0);
                colors.Add(new Color4(gray, gray, gray, 1.0f));
            }
            else
            {
                rock.UpdateVertex(vid, (1.0f + v.Y * (0.5f * noiseValue + 0.3f * smallNoiseValue)) * v);
                var gray = GetGrayValue(noiseValue, smallNoiseValue);
                colors.Add(new Color4(gray, gray, gray, 1.0f));
            }
        }
    }

    private static float GetGrayValue(float noise, float smallNoise)
    {
        return 0.1f + 0.35f * noise + 0.25f * smallNoise;
    }
}
