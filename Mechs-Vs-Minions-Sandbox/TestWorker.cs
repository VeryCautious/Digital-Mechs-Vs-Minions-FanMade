using System.Collections.Immutable;
using System.Drawing;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Microsoft.Extensions.Hosting;

namespace Mechs_Vs_Minions_Sandbox;

public class TestWorker : BackgroundService
{
    private readonly IView<GameState> _view;

    public TestWorker(IView<GameState> view)
    {
        _view = view;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var gs = GameState.Initial;
        var transition0 = new ApplyGlitchDamageToMechTransition(gs.MechStates.First().Id);
        var transition1 = new UseBlazeTransition(gs.MechStates.First().Id, new Point(0, 0), new Point(0, 0),
            ImmutableList.Create<MoveTransition>(), ImmutableList.Create(gs.MinionStates.First()),
            ImmutableList.Create(gs.MinionStates.Last()));
        var transition2 = new UseScytheTransition(gs.MechStates.First().Id, Orientation.North, Orientation.South,
            ImmutableList.Create(gs.MinionStates.First()));
        var transition3 = new UseAimBotTransition(gs.MechStates.First().Id, gs.MechStates.First().Position, gs.MinionStates.First().Position,
            ImmutableList.Create(gs.MinionStates.First()));
        await _view.SendNewTransitions(
            ImmutableList.Create<(Guid, Transition, GameState)>((Guid.NewGuid(), Transition.None, gs)));

        var transitions = new Transition[] { transition0, transition1, transition2, transition3 };
        for (var i = 0; i < 15; i++)
            foreach (var transition in transitions)
                await _view.SendNewTransitions(
                    ImmutableList.Create<(Guid, Transition, GameState)>((Guid.NewGuid(), transition, gs)));

        _view.Run(CancellationToken.None);
    }
}