namespace Mechs_Vs_Minions_Abstractions;

public sealed record AudioHandle(string Filename)
{
    public static AudioHandle StoneSlide => new("Audio/stone_slide.wav");
    public static AudioHandle FireBurst => new("Audio/fire_burst.wav");
    public static AudioHandle Slash => new("Audio/slash.wav");
    public static AudioHandle Swoosh => new("Audio/swoosh.wav");
};