using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Abstractions;

public interface IView<TGameState>
{
    void Run(CancellationToken cancellationToken);
    Task SendNewTransitions(IImmutableList<(Guid, Transition, TGameState)> stateTransitions);
    Task<Guid?> GetLastSeenState();
}
