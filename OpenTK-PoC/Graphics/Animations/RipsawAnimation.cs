using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class RipsawAnimation : IAnimation
{
    private readonly IInstanceFactory<RipSawInstance>  _instanceFactory;
    private readonly IAudioStore _audioStore;
    private bool _soundPlayed;
    private readonly Handle<RipSawInstance> _instanceHandle;
    private readonly Vector3 _startPosition;
    private readonly Vector3 _directionFromStartToEnd;
    private readonly TimeSpan _animationDuration;
    private TimeSpan _playedDuration = TimeSpan.Zero;
    private const float RotationSpeed = 20.0f;

    /// <summary>
    /// Value between 0 and 1 where 0 means that the animation just started and 1 that it has finished
    /// </summary>
    private float NormalizedAnimationTime => (float)(_playedDuration.TotalMilliseconds / _animationDuration.TotalMilliseconds);

    public RipsawAnimation(TimeSpan animationDuration, IInstanceFactory<RipSawInstance> instanceFactory, IAudioStore audioStore, Vector3 startPosition, Vector3 endPosition)
    {
        _soundPlayed = false;
        _animationDuration = animationDuration;
        _instanceFactory = instanceFactory;
        _audioStore = audioStore;
        _instanceHandle = instanceFactory.CreateInstance(RipSawInstance.DefaultFactory);
        _startPosition = startPosition;
        _directionFromStartToEnd = endPosition - _startPosition;
    }

    public bool HasFinished() => _animationDuration <= _playedDuration;

    public void Update(TimeSpan passedTime)
    {
        UpdatePassedTime(passedTime);
        UpdateInstance();
        if (!_soundPlayed)
        {
            _audioStore.Get(AudioHandle.Swoosh).Play();
            _soundPlayed = true;
        }
    }

    private void UpdateInstance()
    {
        var currentPosition = CalculateCurrentPosition();
        var sawAngle = CalculateCurrentRotation();
        _instanceFactory.Update(_instanceHandle, (inst)=>inst with {ModelTransform = Matrix4.CreateRotationY(sawAngle) * Matrix4.CreateTranslation(currentPosition)});
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }

    private Vector3 CalculateCurrentPosition() => _startPosition + Vector3.Multiply(_directionFromStartToEnd, NormalizedAnimationTime);

    private float CalculateCurrentRotation() => NormalizedAnimationTime * RotationSpeed * ((float)Math.PI * 2.0f);

    public void Dispose()
    {
        _instanceFactory.DestroyInstance(_instanceHandle);
    }
}