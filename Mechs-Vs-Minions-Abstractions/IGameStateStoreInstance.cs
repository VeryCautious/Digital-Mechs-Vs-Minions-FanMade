namespace Mechs_Vs_Minions_Abstractions;

public interface IGameStateStoreInstance<TGameState>
{
    Task<TGameState?> Load();
    Task Save(TGameState gameState);
}