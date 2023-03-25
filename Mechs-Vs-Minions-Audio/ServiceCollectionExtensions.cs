using Mechs_Vs_Minions_Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mechs_Vs_Minions_Audio;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudio(this IServiceCollection sc)
    {
        sc.AddSingleton<IAudioStore,AudioStore>();
        AudioPlayer.Load();
        return sc;
    }
}