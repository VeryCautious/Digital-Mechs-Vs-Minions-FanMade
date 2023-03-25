using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IInstanceMutator<TInstance>
    where TInstance : Instance
{
    void Update(Handle<TInstance> handle, Func<TInstance, TInstance> mutator);
    void Update(Handle<TInstance> handle, TInstance overridingInstance);
}