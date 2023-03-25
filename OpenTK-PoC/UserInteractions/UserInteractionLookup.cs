using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

namespace Mechs_Vs_Minions_Graphics.UserInteractions;

internal class UserInteractionLookup : IUserInteractionLookup
{
    
    private readonly Dictionary<InstanceHandle, UserInteraction> _interactionLookup;

    public UserInteractionLookup()
    {
        _interactionLookup = new Dictionary<InstanceHandle, UserInteraction>();
    }

    public void Add(InstanceHandle handle, UserInteraction interaction)
    {
        _interactionLookup.Add(handle, interaction);
    }

    public void Remove(InstanceHandle handle)
    {
        _interactionLookup.Remove(handle);
    }

    public UserInteraction? GetInteractionFrom(InstanceHandle handle)
    {
        _interactionLookup.TryGetValue(handle, out var interaction);
        return interaction;
    }
}