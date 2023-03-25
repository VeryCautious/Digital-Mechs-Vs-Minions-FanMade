using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

public interface IGameStateQueue : IGameStateProvider
{
    void Push(Transition transition, GameState gameState);
}