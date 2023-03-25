namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IVertex<T> where T : struct
{
    IEnumerable<VertexAttribute<T>> GetAttributes();
}