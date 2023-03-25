using Mechs_Vs_Minions_Abstractions.GameStates.Cards;

namespace Mechs_Vs_Minions_Abstractions.GameStates;

public abstract record BoardTile(bool Passable);
public sealed record ObstacleTile() : BoardTile(false);
public sealed record RepairTile() : BoardTile(true);
public sealed record BasicTile() : BoardTile(true);
public sealed record SpawnTile(GameColor Color) : BoardTile(true);