using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Microsoft.Extensions.DependencyInjection;

namespace Mechs_Vs_Minions_Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection sc)
    {
        sc.AddScoped<ISerializer,JsonSerializer>();
        sc.AddScoped<IGameStateStore<GameState>, GameStateStore>();

        return sc;
    }
}