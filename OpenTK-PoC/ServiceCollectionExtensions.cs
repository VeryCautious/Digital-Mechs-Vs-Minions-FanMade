using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.GameWindow;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using Mechs_Vs_Minions_Graphics.UserInteractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mechs_Vs_Minions_Graphics;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGraphics(this IServiceCollection sc)
    {
        sc.AddSingleton<IUserInteractionLookup, UserInteractionLookup>();

        sc.AddSingleton<GameStateQueue>();
        sc.AddSingleton<IGameStateQueue, GameStateQueue>(sp => sp.GetRequiredService<GameStateQueue>());
        sc.AddSingleton<IGameStateProvider, GameStateQueue>(sp => sp.GetRequiredService<GameStateQueue>());

        sc.AddSingleton<GameWindow>();
        sc.AddSingleton<IView<GameState>,View>();
        sc.AddSingleton<IController,GameWindow>(sp => sp.GetRequiredService<GameWindow>());

        sc.AddSingleton<TexturesStore>();
        sc.AddSingleton<ITexturesStore, TexturesStore>(sp => sp.GetRequiredService<TexturesStore>());

        sc.AddSingleton<ObjStore>();
        sc.AddSingleton<IObjStore, ObjStore>();

        sc.AddSingleton<LightUniformSet>();

        sc.AddSingleton<CameraUniformSet>();
        sc.AddSingleton<ICameraUniformData, CameraUniformSet>(sp => sp.GetRequiredService<CameraUniformSet>());

        sc.AddSingleton<OverlayUniformSet>();
        sc.AddSingleton<IOverlayUniformData, OverlayUniformSet>(sp => sp.GetRequiredService<OverlayUniformSet>());

        sc.AddSingleton<ParticleUniformSet>();
        sc.AddSingleton<IParticleUniformData, ParticleUniformSet>(sp => sp.GetRequiredService<ParticleUniformSet>());

        sc.AddSingleton<GraphicsModelStore>();
        sc.AddSingleton<IGraphicsModelProvider, GraphicsModelStore>(sp => sp.GetRequiredService<GraphicsModelStore>());
        return sc;
    }
}