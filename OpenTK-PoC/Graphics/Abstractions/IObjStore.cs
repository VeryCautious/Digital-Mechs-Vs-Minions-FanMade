using Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IObjStore
{
    IImmutableList<Triangle> Get(string filepath);
    bool HasLoaded(string filepath);
    void Add(string texturePath);
}

