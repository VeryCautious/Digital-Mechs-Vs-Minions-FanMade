namespace Mechs_Vs_Minions_Abstractions;

public interface IIdentifiable<out TId>
{
    public TId Id { get; }
}