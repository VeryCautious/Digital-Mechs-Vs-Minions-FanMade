using OpenTK.Audio.OpenAL;

namespace Mechs_Vs_Minions_Audio;

internal static class AudioPlayer
{
    public static void Load()
    {
        ALBase.RegisterOpenALResolver();
        
        var device = ALC.OpenDevice(null);
        var context = ALC.CreateContext(device, Array.Empty<int>());

        ALC.MakeContextCurrent(context);
    }
}