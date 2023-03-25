namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal interface IAnimation : IDisposable
{
    bool HasFinished();
    void Update(TimeSpan passedTime);
}