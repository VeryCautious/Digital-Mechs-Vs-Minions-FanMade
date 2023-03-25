using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class LinearMoveAnimation<TInstance> : IAnimation where TInstance : PositionalInstance
{
    private readonly IInstanceMutator<TInstance> _renderableInstanceState;
    private bool _playedAudio;
    private readonly IAudioStore? _audioStore;
    private readonly Handle<TInstance> _instanceHandle;
    private readonly Vector3 _startPosition;
    private readonly Vector3 _directionFromStartToEnd;
    private readonly Orientation _orientation;
    private readonly TimeSpan _animationDuration;
    private TimeSpan _playedDuration;

    /// <summary>
    /// Value between 0 and 1 where 0 means that the animation just started and 1 that it has finished
    /// </summary>
    private float NormalizedAnimationTime => (float)(_playedDuration.TotalMilliseconds / _animationDuration.TotalMilliseconds);

    public LinearMoveAnimation(
        TimeSpan animationDuration, 
        IInstanceMutator<TInstance> renderableInstanceState, 
        IAudioStore? audioStore, 
        Handle<TInstance> instanceHandle, 
        Vector3 startPosition, 
        Vector3 endPosition, 
        Orientation orientation,
        TimeSpan delay)
    {
        _playedAudio = false;
        _playedDuration = TimeSpan.Zero.Subtract(delay);
        _animationDuration = animationDuration;
        _renderableInstanceState = renderableInstanceState;
        _audioStore = audioStore;
        _instanceHandle = instanceHandle ?? throw new ArgumentNullException(nameof(instanceHandle));
        _startPosition = startPosition;
        _orientation = orientation;
        _directionFromStartToEnd = endPosition - _startPosition;
    }

    public LinearMoveAnimation(
        TimeSpan animationDuration,
        IInstanceMutator<TInstance> renderableInstanceState,
        Handle<TInstance> instanceHandle,
        Vector3 startPosition,
        Vector3 endPosition,
        Orientation orientation,
        TimeSpan delay
    ) : this(
        animationDuration,
        renderableInstanceState,
        null,
        instanceHandle,
        startPosition,
        endPosition,
        orientation,
        delay
    )
    { }

    public LinearMoveAnimation(
        TimeSpan animationDuration,
        IInstanceMutator<TInstance> renderableInstanceState,
        IAudioStore audioStore,
        Handle<TInstance> instanceHandle,
        Vector3 startPosition,
        Vector3 endPosition,
        Orientation orientation
    ) : this(
        animationDuration,
        renderableInstanceState,
        audioStore,
        instanceHandle,
        startPosition,
        endPosition,
        orientation,
        TimeSpan.Zero
    )
    { }

    public bool HasFinished() => _animationDuration <= _playedDuration;

    public void Update(TimeSpan passedTime)
    {
        UpdatePassedTime(passedTime);

        if (_playedDuration.TotalMilliseconds < 0) return;

        UpdateInstance();

        if (!_playedAudio)
        {
            _audioStore?.Get(AudioHandle.StoneSlide).Play();
            _playedAudio = true;
        }
    }

    private void UpdateInstance()
    {
        var currentPosition = CalculateCurrentPosition();
        var angle = (int)_orientation * ((float)Math.PI / 2.0f);
        _renderableInstanceState.Update(_instanceHandle, (inst) => inst with {
            ModelTransform = Matrix4.CreateRotationY(angle) * Matrix4.CreateTranslation(currentPosition)
        });
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }

    private Vector3 CalculateCurrentPosition() => _startPosition + Vector3.Multiply(_directionFromStartToEnd, NormalizedAnimationTime);
    public void Dispose()
    {
    }
}