using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

public interface IGameStateProvider
{
    bool GetNext(out Transition transition, out GameState gameState);

    bool PeekGameState(out GameState gameState);
}