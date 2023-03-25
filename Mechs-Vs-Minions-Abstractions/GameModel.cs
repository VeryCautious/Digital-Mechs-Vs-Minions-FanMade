using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Abstractions;

public class GameModel<TGameState>
{
    private readonly IActionPrompter<TGameState> _prompter;
    private readonly IGameStateTransformer<TGameState> _transformer;
    private readonly List<(Guid ID, Transition Transition, TGameState GameState)> _transitions;

    public TGameState CurrentGameState => _transitions.Last().GameState;

    public void Load(TGameState gameState)
    {
        _transitions.Clear();
        _transitions.Add((Guid.NewGuid(), Transition.None, gameState));
    }

    public GameModel(IGameStateTransformer<TGameState> transformer,
        IActionPrompter<TGameState> prompter)
    {
        _transformer = transformer;
        _prompter = prompter;
        _transitions = new List<(Guid, Transition, TGameState)>();
    }

    public IImmutableList<(Guid id, Transition transition, TGameState gameState)> TransitionsAfter(Guid? lastSeenTransition)
    {
        var indexOfLastSeen = _transitions.IndexOf(_transitions.SingleOrDefault(t => t.ID == lastSeenTransition));

        return _transitions.Skip(++indexOfLastSeen).ToImmutableList();
    }

    public void Apply(StateAction transaction)
    {
        var (transition, newGameState) = _transformer.Apply(_transitions.Last().GameState, transaction);
        _transitions.Add((Guid.NewGuid(), transition, newGameState));
    }

    public Task<StateAction> GetNextStateAction(TGameState gameState, IController controller)
    {
        return _prompter.GetActionFrom(gameState, controller);
    }
}