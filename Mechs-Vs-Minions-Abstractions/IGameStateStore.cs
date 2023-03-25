using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Abstractions;

public interface IGameStateStore<TGameState>
{
    Task<IImmutableList<(string, Guid)>> GetSavedNames();
    Task<TGameState?> LoadGameState(Guid id);
    Task<Guid> SaveNew(TGameState gameState);
    Task Save(TGameState gameState, Guid id);
    IGameStateStoreInstance<TGameState> GetInstanceGameStateStore(Guid id);
}