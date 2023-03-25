using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using System.Collections.Concurrent;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

public class GameStateQueue : IGameStateQueue
{
    private readonly ConcurrentQueue<(Transition,GameState)> _gameStates;

    public GameStateQueue()
    {
        _gameStates = new ConcurrentQueue<(Transition, GameState)>();
    }

    public bool GetNext(out Transition transition, out GameState gameState)
    {
        if (_gameStates.TryDequeue(out var tuple))
        {
            transition = tuple.Item1;
            gameState = tuple.Item2;
            return true;
        }

        transition = null!;
        gameState = null!;
        return false;
    }

    public bool PeekGameState(out GameState gameState)
    {
        if (_gameStates.TryPeek(out var tuple))
        {
            gameState = tuple.Item2;
            return true;
        }

        gameState = null!;
        return false;
    }

    public void Push(Transition transition, GameState gameState)
    {
        _gameStates.Enqueue((transition, gameState));
    }
}