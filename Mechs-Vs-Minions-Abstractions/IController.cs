namespace Mechs_Vs_Minions_Abstractions;

public interface IController
{
    Task<FieldSelectionResult> Prompt(SelectFieldFrom userPrompt);
    Task<CardSelectionResult> Prompt(SelectCardFrom userPrompt);
    Task<SelectCommandSlotResult> Prompt(SelectCommandSlotFrom userPrompt);
    Task<SelectRotationResult> Prompt(SelectRotationFrom userPrompt);
    Task<ConfirmResult> Prompt(Confirm userPrompt);
    Task<MetaGameRequest> DequeueMetaGameInteraction(CancellationToken cancellation);
}
