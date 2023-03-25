using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal sealed class RenderModelBindings : IDisposable
{
    private readonly IReadOnlyList<IBindable> _bindings;

    public RenderModelBindings(IImmutableList<IBindable> bindings)
    {
        _bindings = bindings;
    }

    public void BindAll()
    {
        foreach (var binding in _bindings)
        {
            binding.Bind();
        }
    }

    public void UnbindAll()
    {
        foreach (var binding in _bindings)
        {
            binding.Unbind();
        }
    }

    public void Dispose() => UnbindAll();
}
