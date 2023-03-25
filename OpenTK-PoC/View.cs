using System.Collections.Immutable;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.GameWindow;

namespace Mechs_Vs_Minions_Graphics;

internal class View : IView<GameState>
{
    
    private Guid? _lastSeenGuid;
    private readonly GameWindow _gameWindow;
    private readonly IGameStateQueue _gameStateStore;

    public View(GameWindow gameWindow, IGameStateQueue gameStateStore)
    {
        _gameWindow = gameWindow;
        _gameStateStore = gameStateStore;
    }

    public Task<Guid?> GetLastSeenState()
    {
        return Task.FromResult(_lastSeenGuid);
    }

    public Task SendNewTransitions(IImmutableList<(Guid, Transition, GameState)> stateTransitions)
    {
        _lastSeenGuid = stateTransitions[^1].Item1;

        foreach (var stateTransition in stateTransitions){
            _gameStateStore.Push(stateTransition.Item2, stateTransition.Item3);
        }

        return Task.CompletedTask;
    }

    public void Run(CancellationToken cancellationToken)
    {
        _gameWindow.Display(cancellationToken);
        _gameWindow.Dispose();
    }
}