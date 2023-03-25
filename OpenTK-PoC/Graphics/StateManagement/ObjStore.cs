using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

internal class ObjStore : IObjStore
{
    private readonly Dictionary<string, IImmutableList<Triangle>> _store;

    public ObjStore()
    {
        _store = new Dictionary<string, IImmutableList<Triangle>>();
    }

    public IImmutableList<Triangle> Get(string filepath) => _store[filepath];

    public bool HasLoaded(string filepath) => _store.ContainsKey(filepath);

    public void Add(string filePath)
    {
        if (_store.ContainsKey(filePath)) return;

        var triangles = LoadObjFromFile(filePath);
        _store.Add(filePath, triangles);
    }

    private static IImmutableList<Triangle> LoadObjFromFile(string filePath)
    {
        var modelLoader = new ObjFileLoader(filePath);
        return modelLoader.LoadFirstGroup();
        
    }
}
