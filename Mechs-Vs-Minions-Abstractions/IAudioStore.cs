namespace Mechs_Vs_Minions_Abstractions;

public interface IAudioStore
{
    void Load(AudioHandle audioHandle);
    IPlayable Get(AudioHandle audioHandle);
}