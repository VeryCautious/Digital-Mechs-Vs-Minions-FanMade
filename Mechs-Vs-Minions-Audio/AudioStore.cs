using Mechs_Vs_Minions_Abstractions;

namespace Mechs_Vs_Minions_Audio;

internal class AudioStore : IAudioStore
{
    private readonly IDictionary<AudioHandle, Audio> _audioLookup;

    public AudioStore()
    {
        _audioLookup = new Dictionary<AudioHandle, Audio>();
    }

    public void Load(AudioHandle audioHandle)
    {
        _audioLookup[audioHandle] = new Audio(audioHandle.Filename);
    }

    public IPlayable Get(AudioHandle audioHandle) => _audioLookup[audioHandle];
}