namespace Mechs_Vs_Minions_Abstractions;

public interface IGameStateTransformer<TGameState>
{
    (Transition transition, TGameState newGameState) Apply(TGameState newGameState, StateAction transition);
}
