using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.Utilities;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class LinearRotationAnimation<TInstance> : IAnimation 
    where TInstance : PositionalInstance
{
    private readonly IInstanceMutator<TInstance> _renderableInstanceState;
    private readonly Handle<TInstance> _instanceHandle;
    private readonly float _startAngle;
    private readonly float _angleFromStartToEnd;
    private readonly Vector3 _mechPosition;
    private readonly TimeSpan _animationDuration;
    private TimeSpan _playedDuration = TimeSpan.Zero;

    /// <summary>
    /// Value between 0 and 1 where 0 means that the animation just started and 1 that it has finished
    /// </summary>
    private float NormalizedAnimationTime => (float)(_playedDuration.TotalMilliseconds / _animationDuration.TotalMilliseconds);

    public LinearRotationAnimation(TimeSpan animationDuration, IInstanceMutator<TInstance> renderableInstanceState, Handle<TInstance> instanceHandle, Orientation startOrientation, Orientation endOrientation, Vector3 mechPosition)
    {
        _animationDuration = animationDuration;
        _renderableInstanceState = renderableInstanceState;
        _instanceHandle = instanceHandle;
        _mechPosition = mechPosition;
        _startAngle = startOrientation.ToFloatAngle();
        _angleFromStartToEnd = OrientationExtensions.AngleBetweenOrientations(startOrientation, endOrientation);
    }

    public bool HasFinished() => _animationDuration <= _playedDuration;

    public void Update(TimeSpan passedTime)
    {
        UpdatePassedTime(passedTime);
        UpdateInstance();
    }

    private void UpdateInstance()
    {
        var currentAngle = _startAngle + NormalizedAnimationTime * _angleFromStartToEnd;
        _renderableInstanceState.Update(
            _instanceHandle,
            (inst) => inst with
            {
                ModelTransform = Matrix4.CreateRotationY(currentAngle) * Matrix4.CreateTranslation(_mechPosition)
            }
        );
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }

    public void Dispose()
    {
    }
}