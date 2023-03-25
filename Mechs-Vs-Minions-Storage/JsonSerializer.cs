using System.Text;
using Newtonsoft.Json;

namespace Mechs_Vs_Minions_Storage;

public class JsonSerializer : ISerializer
{
    private static JsonSerializerSettings SerializerSettings => new() { TypeNameHandling = TypeNameHandling.All};

    public async Task<T> Deserialize<T>(Stream stream)
    {
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        
        var jsonText = Encoding.UTF8.GetString(memoryStream.ToArray());
        
        var gameState = JsonConvert.DeserializeObject<T>(jsonText, SerializerSettings);
        
        return gameState ?? throw new ArgumentException();
    }

    public MemoryStream Serialize<T>(T obj)
    {
        var jsonText = JsonConvert.SerializeObject(obj, SerializerSettings);
        return new MemoryStream(Encoding.UTF8.GetBytes(jsonText));
    }

}