using System.Drawing;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;

namespace Mechs_Vs_Minions_Abstractions;

public abstract record PromptResult;
public sealed record FieldSelectionResult(Point SelectedField) : PromptResult;
public sealed record CardSelectionResult(PlayableCard PlayableCard) : PromptResult;
public sealed record SelectCommandSlotResult(int SlotIndex) : PromptResult;
public sealed record SelectRotationResult(Rotation Rotation) : PromptResult;
public sealed record ConfirmResult : PromptResult;
public abstract record MetaGameRequest;
public sealed record ResetGameRequest : MetaGameRequest;