using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

namespace Mechs_Vs_Minions_Graphics.Graphics.Models;

internal sealed class InstanceStore<TInstance> : IInstanceFactory<TInstance>
    where TInstance : Instance
{
    private readonly IDictionary<Handle<TInstance>, TInstance> _instances;

    public InstanceStore()
    {
        _instances = new Dictionary<Handle<TInstance>, TInstance>();
    }

    public void Update(Handle<TInstance> handle, Func<TInstance, TInstance> mutator)
    {
        if (!_instances.TryGetValue(handle, out var instance))
        {
            throw new ArgumentOutOfRangeException(nameof(handle), "Instance is not present");
        }
        
        _instances[handle] = mutator(instance);
    }

    public void Update(Handle<TInstance> handle, TInstance overridingInstance)
    {
        Update(handle, (_) => overridingInstance);
    }

    public Handle<TInstance> CreateInstance(Func<TInstance> factory)
    {
        var handle = new Handle<TInstance>(Guid.NewGuid());
        _instances[handle] = factory();
        return handle;
    }

    public void DestroyInstance(Handle<TInstance> handle)
        => _instances.Remove(handle);

    public IImmutableList<TInstance> GetInstances() 
        => _instances.Values.ToImmutableList();

    public TInstance Get(Handle<TInstance> handle)
        => _instances[handle];

    public bool Contains(Handle<TInstance> handle) => _instances.ContainsKey(handle);

    public void Destroy(ImmutableList<Handle<TInstance>> handles)
    {
        foreach (var handle in handles)
        {
            DestroyInstance(handle);
        }
    }

    public void Clear() => _instances.Clear();
}