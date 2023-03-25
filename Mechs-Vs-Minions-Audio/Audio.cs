using Mechs_Vs_Minions_Abstractions;
using OpenTK.Audio.OpenAL;

namespace Mechs_Vs_Minions_Audio;

public class Audio : IDisposable, IPlayable
{
    private readonly int _sourceHandle;
    private readonly int _bufferHandle;

    private sealed record AudioData(byte[] Data, int SampleRate);

    public Audio(string wavFile) : this(ParseWavFile(wavFile))
    {
    }

    private Audio(AudioData audioData)
    {
        _sourceHandle = AL.GenSource();
        _bufferHandle = AL.GenBuffer();

        AL.BufferData(_bufferHandle, ALFormat.Stereo16, audioData.Data, audioData.SampleRate);
        AL.BindBufferToSource(_sourceHandle, _bufferHandle);
    }

    private static AudioData ParseWavFile(string wavFile)
    {
        var (format, dataInfo) = WAVFileReader.ReadFile(File.OpenRead(wavFile));

        var fs = File.OpenRead(wavFile);
        fs.Seek(dataInfo.Begin, SeekOrigin.Begin);
        var data = new byte[dataInfo.Size];
        var redBytes = fs.Read(data, 0, (int)dataInfo.Size);
        
        if (redBytes < dataInfo.Size) throw new Exception();
        
        return new AudioData(data, (int)format.SampleRate);
    }

    public void Play() => AL.SourcePlay(_sourceHandle);

    public void Dispose()
    {
        AL.DeleteBuffer(_bufferHandle);
        AL.DeleteSource(_sourceHandle);
    }
}