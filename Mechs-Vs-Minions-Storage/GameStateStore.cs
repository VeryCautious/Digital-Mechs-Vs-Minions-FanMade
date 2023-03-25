using System.Collections.Immutable;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;

namespace Mechs_Vs_Minions_Storage;

public class GameStateStore : IGameStateStore<GameState>
{

    private readonly ISerializer _serializer;
    private const string FileEnding = ".gs";
    private static string BasePath => Directory.GetCurrentDirectory() + "/saves/";

    public GameStateStore(ISerializer serializer)
    {
        _serializer = serializer;
    }

    public Task<IImmutableList<(string, Guid)>> GetSavedNames()
    {
        var fileNames = Directory.GetFiles(BasePath, $"*{FileEnding}");
        var saves = fileNames.Select(ParseFileName).ToImmutableList();
        return Task.FromResult<IImmutableList<(string, Guid)>>(saves);
    }

    private static (string, Guid) ParseFileName(string fileName)
    {
        var nameTrimmed = fileName.Replace(FileEnding, "");
        return ("TODO - load Name", Guid.Parse(nameTrimmed));
    }

    private static string GenerateFileName(Guid id)
    {
        return $"{id}{FileEnding}";
    }

    public async Task<GameState?> LoadGameState(Guid id)
    {
        var fileName = GenerateFileName(id);

        if (!File.Exists(BasePath + fileName))
        {
            return null;
        }

        await using var fs = File.Open(BasePath + fileName, FileMode.Open);

        try
        {
            var gameState = await _serializer.Deserialize<GameState>(fs);

            return gameState;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null;
    }

    public async Task<Guid> SaveNew(GameState gameState)
    {
        var id = Guid.NewGuid();
        await Save(gameState, id);
        return id;
    }

    public async Task Save(GameState gameState, Guid id)
    {
        var fileName = GenerateFileName(id);
        var stream = _serializer.Serialize(gameState);

        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
        }
        
        await using var fs = File.Open(BasePath + fileName, FileMode.Create);
        await stream.CopyToAsync(fs);
    }

    public IGameStateStoreInstance<GameState> GetInstanceGameStateStore(Guid id)
    {
        return new GameStateStoreInstance(id, this);
    }
}