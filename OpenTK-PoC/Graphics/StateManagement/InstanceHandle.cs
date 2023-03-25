namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

// ReSharper disable once NotAccessedPositionalProperty.Global
internal record InstanceHandle(Guid Id);

// ReSharper disable once UnusedTypeParameter
internal record Handle<TInstance>(Guid Id) : InstanceHandle(Id)
    where TInstance : Instance ;