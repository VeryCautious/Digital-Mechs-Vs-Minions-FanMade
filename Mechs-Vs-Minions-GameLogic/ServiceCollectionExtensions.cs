using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mechs_Vs_Minions_GameLogic;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameLogic(this IServiceCollection sc)
    {
        sc.AddScoped<IActionPrompter<GameState>,ActionPrompter>();
        sc.AddScoped<IGameStateTransformer<GameState>,GameStateTransformer>();
        sc.AddScoped(
            sp =>
                new GameModel<GameState>(
                    sp.GetRequiredService<IGameStateTransformer<GameState>>(),
                    sp.GetRequiredService<IActionPrompter<GameState>>()
                ));
        return sc;
    }

}