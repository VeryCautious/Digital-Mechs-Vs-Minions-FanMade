using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.VertexTypes;

public sealed record Triangle(Vertex Vertex1, Vertex Vertex2, Vertex Vertex3)
{
    public IImmutableList<Vertex> AsList() => ImmutableList.Create(Vertex1, Vertex2, Vertex3);
};