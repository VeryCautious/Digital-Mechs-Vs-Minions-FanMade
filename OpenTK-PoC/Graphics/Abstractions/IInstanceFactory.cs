using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IInstanceFactory<TInstance> : IInstanceMutator<TInstance>
    where TInstance : Instance
{
    Handle<TInstance> CreateInstance(Func<TInstance> instanceFactory);
    void DestroyInstance(Handle<TInstance> handle);
}