using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Immutable;
using System.Drawing;
using Utilities;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal class GameBoardModel : RenderModel<PositionalInstance>
{
    private const string GrassTexturePath = "Models/grass.png";
    private const string TextureSampler = "textureSampler";
    private readonly PositionalInstanceUniformSet _positionalInstanceUniformSet;

    public static GameBoardModel Generate(ITexturesStore texturesStore, ICameraUniformData cameraUniformData, GameBoard gameBoard) => new GameBoardModel(texturesStore, cameraUniformData, gameBoard);

    private GameBoardModel(ITexturesStore texturesStore, ICameraUniformData cameraUniformData, GameBoard gameBoard) : base(ConstructRenderModelComponents(texturesStore, gameBoard), ConstructRenderModelParameters())
    {
        _positionalInstanceUniformSet = new PositionalInstanceUniformSet(BaseTransformation, cameraUniformData);
    }

    private static RenderParameters ConstructRenderModelParameters()
        => new(Matrix4.Identity, false);

    private static RenderComponents ConstructRenderModelComponents(ITexturesStore texturesStore, GameBoard gameBoard)
    {
        var shaderProgram = ShaderProgramFactory.CreateGameBoardShader();

        var board = GameBoardGenerator.Generate(gameBoard);

        var (vertexArray, normalArray, indexArray) = board.DataWithVertexNormals();

        var vertices = vertexArray.Zip(normalArray)
            .Select(entry => new ProceduralVertex(entry.First, entry.Second, Color4.SandyBrown))
            .ToArray();

        var geometryComponent = new ManualGeometryComponent<ProceduralVertex>(shaderProgram, vertices.ToArray(), indexArray);

        var texture = new Texture(
            shaderProgram,
            TextureSampler,
            texturesStore,
            GrassTexturePath,
            TextureUnit.Texture0
        );

        return new RenderComponents(geometryComponent, shaderProgram, new IRenderModelComponent[] { texture, ShadowMapTextureBuffer.GlobalShadowMaps });
    }

    protected override void OnBeforeParentLoad()
    {
    }

    protected override IImmutableList<Uniform> GetUniformsFrom(PositionalInstance instance)
    {
        return _positionalInstanceUniformSet.GetUniformsFor(instance);
    }
}


internal class GameBoardGenerator
{
    private readonly IImmutableDictionary<Point, BoardTile> _tiles;
    private readonly Mesh _mesh;
    private readonly Dictionary<Point, Quad> _quads;

    private record Quad(Guid Bl, Guid Br, Guid Tl, Guid Tr);

    public static Mesh Generate(GameBoard gameBoard) => new GameBoardGenerator(gameBoard).GenerateMesh();
    private GameBoardGenerator(GameBoard gameBoard)
    {
        _tiles = gameBoard.BoardTiles.Union(GetSurroundingTiles(gameBoard.BoardTiles)).ToImmutableDictionary();
        _mesh = new Mesh();
        _quads = new Dictionary<Point, Quad>();
    }

    private IDictionary<Point, BoardTile> GetSurroundingTiles(IImmutableDictionary<Point, BoardTile> boardTiles) =>
        boardTiles.Keys
            .SelectMany(point => point.AllNeighbors())
            .Distinct()
            .Where(point => !boardTiles.ContainsKey(point))
            .ToDictionary(
                point => point,
                _ => (BoardTile) new BoardEdgeTile()
            );

    private Mesh GenerateMesh()
    {
        _tiles.ToList().ForEach(entry => AddQuad(entry.Value, entry.Key));

        AddEdgeQuads();
        AddCornerQuads();
        RemoveOffsetFromBorderVertices();

        return _mesh;
    }

    private void AddEdgeQuads()
    {
        foreach (var tilePos in _tiles.Keys)
        {
            var baseQuad = _quads[tilePos];
            if (_tiles.ContainsKey(tilePos.RightNeighbor()))
            {
                var neighborQuad = _quads[tilePos.RightNeighbor()];
                var quad = new Quad(baseQuad.Br, neighborQuad.Bl, baseQuad.Tr, neighborQuad.Tl);
                AddQuadFace(quad);
            }
            if (_tiles.ContainsKey(tilePos.TopNeighbor()))
            {
                var neighborQuad = _quads[tilePos.TopNeighbor()];
                var quad = new Quad(baseQuad.Tl, baseQuad.Tr, neighborQuad.Bl, neighborQuad.Br);
                AddQuadFace(quad);
            }

        }
    }

    private void AddCornerQuads()
    {
        foreach (var tilePos in _tiles.Keys)
        {
            var baseQuad = _quads[tilePos];
            _quads.TryGetValue(tilePos.RightNeighbor(), out var rightNeighborQuad);
            _quads.TryGetValue(tilePos.TopNeighbor(), out var topNeighborQuad);
            _quads.TryGetValue(tilePos.TopRightNeighbor(), out var topRightNeighborQuad);

            if (rightNeighborQuad != null && topNeighborQuad != null && topRightNeighborQuad != null)
            {
                AddQuadFace(new Quad(baseQuad.Tr, rightNeighborQuad.Tl, topNeighborQuad.Br, topRightNeighborQuad.Bl));
            }
        }
    }

    private void RemoveOffsetFromBorderVertices()
    {
        foreach (var entry in _tiles)
        {
            var tilePos = entry.Key;
            var quad = _quads[entry.Key];
            var offset = GetTileOffset(entry.Value);

            if (!_quads.ContainsKey(tilePos.LeftNeighbor()))
            {
                var blVertex = _mesh.GetVertexPosition(quad.Bl);
                var tlVertex = _mesh.GetVertexPosition(quad.Tl);
                var diff = new Vector3(-offset, 0.0f, 0.0f);
                _mesh.UpdateVertex(quad.Bl, blVertex + diff);
                _mesh.UpdateVertex(quad.Tl, tlVertex + diff);
            }
            if (!_quads.ContainsKey(tilePos.RightNeighbor()))
            {
                var brVertex = _mesh.GetVertexPosition(quad.Br);
                var trVertex = _mesh.GetVertexPosition(quad.Tr);
                var diff = new Vector3(+offset, 0.0f, 0.0f);
                _mesh.UpdateVertex(quad.Br, brVertex + diff);
                _mesh.UpdateVertex(quad.Tr, trVertex + diff);
            }
            if (!_quads.ContainsKey(tilePos.BottomNeighbor()))
            {
                var blVertex = _mesh.GetVertexPosition(quad.Bl);
                var brVertex = _mesh.GetVertexPosition(quad.Br);
                var diff = new Vector3(0.0f, -offset, 0.0f);
                _mesh.UpdateVertex(quad.Bl, blVertex + diff);
                _mesh.UpdateVertex(quad.Br, brVertex + diff);
            }
            if (!_quads.ContainsKey(tilePos.TopNeighbor()))
            {
                var tlVertex = _mesh.GetVertexPosition(quad.Tl);
                var trVertex = _mesh.GetVertexPosition(quad.Tr);
                var diff = new Vector3(0.0f, +offset, 0.0f);
                _mesh.UpdateVertex(quad.Tl, tlVertex + diff);
                _mesh.UpdateVertex(quad.Tr, trVertex + diff);
            }
        }
    }

    private void AddQuad(BoardTile tile, Point pos)
    {
        var height = GetTileHeight(tile);
        var offset = GetTileOffset(tile);

        var quad = new Quad(
            _mesh.AddVertex(new Vector3(pos.X     + offset, pos.Y +     offset, height)),
            _mesh.AddVertex(new Vector3(pos.X + 1 - offset, pos.Y +     offset, height)),
            _mesh.AddVertex(new Vector3(pos.X     + offset, pos.Y + 1 - offset, height)),
            _mesh.AddVertex(new Vector3(pos.X + 1 - offset, pos.Y + 1 - offset, height))
        );

        AddQuadFace(quad);

        _quads.Add(pos, quad);
    }

    private void AddQuadFace(Quad quad)
    {
        _mesh.AddFace(quad.Bl, quad.Tl, quad.Br);
        _mesh.AddFace(quad.Tl, quad.Tr, quad.Br);
    }

    private static float GetTileOffset(BoardTile tile) => tile switch
    {
        BasicTile => 0.1f,
        ObstacleTile => 0.01f,
        BoardEdgeTile => 0.45f,
        _ => 0f
    };

    private static float GetTileHeight(BoardTile tile) => tile switch
    {
        BasicTile => 0.00f,
        ObstacleTile => -0.1f,
        BoardEdgeTile => 0.5f,
        _ => 0f
    };
}

internal record BoardEdgeTile() : BoardTile(false);