namespace Mechs_Vs_Minions_Storage;

public interface ISerializer
{
    Task<T> Deserialize<T>(Stream stream);
    MemoryStream Serialize<T>(T obj);
}