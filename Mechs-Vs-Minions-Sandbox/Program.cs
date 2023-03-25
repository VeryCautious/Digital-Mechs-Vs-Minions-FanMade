using Mechs_Vs_Minions_Audio;
using Mechs_Vs_Minions_GameLogic;
using Mechs_Vs_Minions_Graphics;
using Mechs_Vs_Minions_Sandbox;
using Mechs_Vs_Minions_Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(
    services =>
        services.AddHostedService<TestWorker>().
            AddGraphics().
            AddGameLogic().
            AddStorage().
            AddAudio()
);

using var host = builder.Build();

host.Run();