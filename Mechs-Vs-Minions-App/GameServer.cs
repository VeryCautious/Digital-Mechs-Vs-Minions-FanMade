using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions;
using System.Diagnostics;

namespace Mechs_Vs_Minions_App;

internal class GameServer
{
    private const int HearBeatFrequencyInMs = 100;

    private readonly GameModel<GameState> _gameModel;
    private readonly IView<GameState> _view;
    private readonly IController _controller;
    private readonly IGameStateStoreInstance<GameState> _gameStateStoreInstance;
    private readonly Stopwatch _stopwatch = new();

    public GameServer(GameModel<GameState> gameModel, IView<GameState> view, IController controller, IGameStateStore<GameState> gameStateStore)
    {
        _gameModel = gameModel;
        _view = view;
        _controller = controller;
        _gameStateStoreInstance = gameStateStore.GetInstanceGameStateStore(Guid.Empty);
    }

    public async Task Run(CancellationToken cancellationToken) {
        
        _gameModel.Load( await _gameStateStoreInstance.Load() ?? GameState.InitialRandom);

        while (true)
        {
            var elapsedMilliseconds = await ServeClients();
            Console.WriteLine($"{elapsedMilliseconds}ms cycle");
            var timeLeft = HearBeatFrequencyInMs - elapsedMilliseconds;

            if (timeLeft > 0)
            {
                await Task.Delay(timeLeft, cancellationToken);
            }
            else {
                Console.WriteLine($"Server-Heartbeat is {-timeLeft}ms overdue!");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

        }

        Console.WriteLine("Server shutdown");
    }

    private async Task<int> ServeClients()
    {
        try
        {
            _stopwatch.Restart();
            var newTransition = _gameModel.TransitionsAfter(await _view.GetLastSeenState());
            await _view.SendNewTransitions(newTransition);
            _stopwatch.Stop();

            var stateAction = await _gameModel.GetNextStateAction(newTransition.Last().gameState, _controller);

            _stopwatch.Start();
            _gameModel.Apply(stateAction);
            await _gameStateStoreInstance.Save(_gameModel.CurrentGameState);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Saving Game");
            await _gameStateStoreInstance.Save(_gameModel.CurrentGameState);
            Console.WriteLine("Saved Game before shutdown");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _stopwatch.Stop();
        }

        return (int)_stopwatch.ElapsedMilliseconds;
    }
}
