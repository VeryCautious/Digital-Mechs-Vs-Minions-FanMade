namespace Mechs_Vs_Minions_Abstractions.GameStates.Cards;

public interface ISlotableCard
{
};

public abstract record Card;
public abstract record PlayableCard(GameColor Color) : Card, ISlotableCard;
public sealed record OmniStompCard() : PlayableCard(GameColor.Green);
public sealed record AimBotCard() : PlayableCard(GameColor.Green);
public sealed record BlazeCard() : PlayableCard(GameColor.Red);
public sealed record ScytheCard() : PlayableCard(GameColor.Blue);
public sealed record RipsawCard() : PlayableCard(GameColor.Blue);
public sealed record SpeedCard() : PlayableCard(GameColor.Yellow);
public abstract record DamageCard : Card;
public abstract record OneTimeUseDamageCard : DamageCard;
/// <summary>
/// Swap command slots of Index1 and Index2
/// </summary>
public sealed record GlitchCard(int Index1, int Index2) : OneTimeUseDamageCard;
public abstract record CommandLineDamageCard : DamageCard, ISlotableCard;
/// <summary>
/// Rotate uncontrollable
/// </summary>
public sealed record StuckControlsCard(Rotation Rotation) : CommandLineDamageCard;