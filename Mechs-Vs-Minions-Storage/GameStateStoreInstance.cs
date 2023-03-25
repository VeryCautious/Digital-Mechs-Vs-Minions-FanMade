using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;

namespace Mechs_Vs_Minions_Storage;

public class GameStateStoreInstance : IGameStateStoreInstance<GameState>
{
    private readonly IGameStateStore<GameState> _gameStateStore;
    private readonly Guid _id;

    public GameStateStoreInstance(Guid id, IGameStateStore<GameState> gameStateStore)
    {
        _id = id;
        _gameStateStore = gameStateStore;
    }

    public async Task<GameState?> Load()
    {
        return await _gameStateStore.LoadGameState(_id);
    }

    public async Task Save(GameState gameState)
    {
        await _gameStateStore.Save(gameState, _id);
    }
}