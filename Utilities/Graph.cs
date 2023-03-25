using System.Collections.Immutable;

namespace Utilities;

public sealed class Graph<TVertex> where TVertex : IEquatable<TVertex>
{
    private int _vertexCounter;
    private readonly Dictionary<TVertex, int> _vertices;
    private readonly Dictionary<int, List<TVertex>> _edges;

    public Graph()
    {
        _vertexCounter = 0;
        _vertices = new Dictionary<TVertex, int>();
        _edges = new Dictionary<int, List<TVertex>>();
    }

    public void AddVertex(TVertex v)
    {
        if (_vertices.ContainsKey(v)) return;
        var id = _vertexCounter++;
        _vertices.Add(v, id);
        _edges.Add(id, new List<TVertex>());
    }

    public void AddVertices(IImmutableList<TVertex> vertices)
    {
        foreach (var v in vertices) AddVertex(v);
    }

    public void TryCreateEdge(TVertex v1, TVertex v2)
    {
        if (!_vertices.ContainsKey(v1) || !_vertices.ContainsKey(v2)) return;
        if (AreAdjacent(v1, v2)) return;

        _edges[_vertices[v1]].Add(v2);
        _edges[_vertices[v2]].Add(v1);
    }

    public bool AreAdjacent(TVertex v1, TVertex v2) => _edges[_vertices[v1]].Contains(v2) && _edges[_vertices[v2]].Contains(v1);

    public bool AreConnected(TVertex v1, TVertex v2)
    {
        var seen = new HashSet<TVertex>(new List<TVertex> { v1 });
        var q = new Queue<TVertex>(new List<TVertex> { v1 });
        while (q.Count > 0)
        {
            var v = q.Dequeue();
            if (v.Equals(v2)) return true;
            var newNeighbors = _edges[_vertices[v]].Where(vertex => !seen.Contains(vertex));

            foreach (var n in newNeighbors)
            {
                seen.Add(n);
                q.Enqueue(n);
            }
        }
        return false;
    }

    public IEnumerable<IEnumerable<TVertex>> GetConnectedComponents()
    {
        var components = new List<IEnumerable<TVertex>>();
        var unseen = new HashSet<TVertex>(_vertices.Keys);
        
        while (unseen.Count > 0)
        {
            var start = unseen.Take(1).First();
            var comp = GetComponentOf(start);
            unseen = unseen.Where(v => !comp.Contains(v)).ToHashSet();
            components.Add(comp);
        }

        return components;
    }

    private IEnumerable<TVertex> GetComponentOf(TVertex start)
    {
        var seen = new HashSet<TVertex>(new List<TVertex> { start });
        var q = new Queue<TVertex>(new List<TVertex> { start });
        while (q.Count > 0)
        {
            var v = q.Dequeue();
            var newNeighbors = _edges[_vertices[v]].Where(vertex => !seen.Contains(vertex));

            foreach (var n in newNeighbors)
            {
                seen.Add(n);
                q.Enqueue(n);
            }
        }
        return seen;
    }
}
