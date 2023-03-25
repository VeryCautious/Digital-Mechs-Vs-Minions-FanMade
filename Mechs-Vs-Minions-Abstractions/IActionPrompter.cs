namespace Mechs_Vs_Minions_Abstractions;

public interface IActionPrompter<in TGameState>
{
    Task<StateAction> GetActionFrom(TGameState gameState, IController controller);
}