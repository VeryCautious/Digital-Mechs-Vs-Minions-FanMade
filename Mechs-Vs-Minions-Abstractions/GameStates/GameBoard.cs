using System.Collections.Immutable;
using System.Drawing;
using Utilities;

namespace Mechs_Vs_Minions_Abstractions.GameStates;

public sealed record GameBoard(IImmutableDictionary<Point, BoardTile> BoardTiles)
{
    public static GameBoard Initial(Point boardSize) => new(GetTiles(boardSize).ToImmutableDictionary());

    public bool IsPassable(Point location) => BoardTiles.ContainsKey(location) && BoardTiles[location].Passable;

    public PointF GetCenter()
    {
        var xMin = (float) BoardTiles.Select(entry => entry.Key.X).Min();
        var xMax = (float) BoardTiles.Select(entry => entry.Key.X).Max() + 1;
        var yMin = (float) BoardTiles.Select(entry => entry.Key.Y).Min();
        var yMax = (float) BoardTiles.Select(entry => entry.Key.Y).Max() + 1;
        return new PointF((xMax + xMin) / 2.0f, (yMax + yMin) / 2.0f);
    }

    private static Dictionary<Point, BoardTile> GetTiles(Point boardSize)
    {
        var dict = new Dictionary<Point, BoardTile>();

        for (var x = 0; x < boardSize.X; x++)
        {
            for (var y = 0; y < boardSize.Y; y++)
            {
                dict.Add(new Point(x,y), new BasicTile());
            }
        }

        return dict;
    }

    public static GameBoard Generate(Point boardSize, ImmutableList<Point> figurePositions, int numberOfObstacles)
    {
        GameBoard gameBoard;
        do
        {
            var tiles = GetTiles(boardSize);
            var freeTiles = tiles.Where(tile => !figurePositions.Contains(tile.Key)).ToList();

            var rand = new Random();
            for (var i = 0; i < numberOfObstacles; i++)
            {
                var entry = freeTiles.ElementAt(rand.Next(0, freeTiles.Count));
                tiles[entry.Key] = new ObstacleTile();
            }

            gameBoard = new GameBoard(tiles.ToImmutableDictionary());
        } while (!gameBoard.AreFigurePositionsValid(figurePositions));

        return gameBoard;
        
    }

    private bool AreFigurePositionsValid(ImmutableList<Point> figurePositions)
    {
        var graph = ToGraph();
        for (var i = 1; i < figurePositions.Count; i++)
        {
            if (!graph.AreConnected(figurePositions[0], figurePositions[i])) return false;
        }
        return true;
    }

    private Graph<Point> ToGraph()
    {
        var graph = new Graph<Point>();

        var vertices = BoardTiles
            .Select(entry => entry.Key)
            .Where(IsPassable).ToImmutableList();

        graph.AddVertices(vertices);

        foreach (var v in vertices)
        {
            var neighbors = new List<Point>()
            {
                v with { X = v.X-1 },
                v with { Y = v.Y+1 },
                v with { X = v.X+1 },
                v with { Y = v.Y-1 }
            };
            neighbors.ForEach(n => graph.TryCreateEdge(v, n));
        }

        return graph;
    }
};