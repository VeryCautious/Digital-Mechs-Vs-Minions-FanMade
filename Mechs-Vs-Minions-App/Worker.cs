using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Microsoft.Extensions.Hosting;

namespace Mechs_Vs_Minions_App;

public class Worker : BackgroundService
{
    private readonly IView<GameState> _view;
    private readonly GameModel<GameState> _gameModel;
    private readonly IGameStateStore<GameState> _gameStateStore;
    private readonly  IController _controller;

    public Worker(IView<GameState> view, GameModel<GameState> gameModel, IGameStateStore<GameState> gameStateStore, IController controller)
    {
        _view = view;
        _gameModel = gameModel;
        _gameStateStore = gameStateStore;
        _controller = controller;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tokenSource = new CancellationTokenSource();

        GameServer server = new(_gameModel, _view, _controller, _gameStateStore);

        var serveRun = server.Run(tokenSource.Token);

        _view.Run(tokenSource.Token);

        tokenSource.Cancel();

        await serveRun;
    }
}