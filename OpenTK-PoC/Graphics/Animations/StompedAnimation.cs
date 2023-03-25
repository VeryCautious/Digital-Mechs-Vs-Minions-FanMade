using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class StompedAnimation<TInstance> : IAnimation where TInstance : PositionalInstance
{
    private readonly IInstanceMutator<TInstance> _renderableInstanceState;
    private readonly Handle<TInstance> _instanceHandle;
    private readonly Orientation _orientation;
    private readonly Vector3 _position;
    private readonly TimeSpan _animationDuration;
    private TimeSpan _playedDuration;

    /// <summary>
    /// Value between 0 and 1 where 0 means that the animation just started and 1 that it has finished
    /// </summary>
    private float NormalizedAnimationTime => (float)(_playedDuration.TotalMilliseconds / _animationDuration.TotalMilliseconds);

    public StompedAnimation(
        TimeSpan animationDuration, 
        IInstanceMutator<TInstance> renderableInstanceState,
        Handle<TInstance> instanceHandle,
        Orientation orientation,
        Vector3 position,
        TimeSpan delay)
    {
        _playedDuration = TimeSpan.Zero.Subtract(delay);
        _animationDuration = animationDuration;
        _renderableInstanceState = renderableInstanceState;
        _instanceHandle = instanceHandle ?? throw new ArgumentNullException(nameof(instanceHandle));
        _orientation = orientation;
        _position = position;
    }

    public StompedAnimation(
        TimeSpan animationDuration,
        IInstanceMutator<TInstance> renderableInstanceState,
        Handle<TInstance> instanceHandle,
        Orientation orientation,
        Vector3 position
    ) : this(
        animationDuration,
        renderableInstanceState,
        instanceHandle,
        orientation,
        position,
        TimeSpan.Zero
    )
    { }

    public bool HasFinished() => _animationDuration <= _playedDuration;

    public void Update(TimeSpan passedTime)
    {
        UpdatePassedTime(passedTime);

        if (_playedDuration.TotalMilliseconds < 0) return;

        UpdateInstance();
    }

    private void UpdateInstance()
    {
        var angle = (int)_orientation * ((float)Math.PI / 2.0f);
        _renderableInstanceState.Update(_instanceHandle, (inst) => inst with {
            ModelTransform = CalculateScale() * Matrix4.CreateRotationY(angle) * Matrix4.CreateTranslation(_position)
        });
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }

    private Matrix4 CalculateScale() => Matrix4.CreateScale(1f, Math.Max(.7f - NormalizedAnimationTime, 0f) + .3f, 1f);

    public void Dispose()
    {
    }
}