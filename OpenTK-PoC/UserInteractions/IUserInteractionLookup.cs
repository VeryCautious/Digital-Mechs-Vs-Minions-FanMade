using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

namespace Mechs_Vs_Minions_Graphics.UserInteractions;

internal interface IUserInteractionLookup
{
    void Add(InstanceHandle handle, UserInteraction interaction);
    void Remove(InstanceHandle handle);
    UserInteraction? GetInteractionFrom(InstanceHandle handle);
}