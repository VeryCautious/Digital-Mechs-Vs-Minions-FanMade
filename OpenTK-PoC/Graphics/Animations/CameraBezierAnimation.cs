using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class CameraBezierAnimation : IAnimation
{
    private readonly GameCameraManager _cameraManager;

    private readonly Vector3BezierCurve _positionCurve;
    private readonly FloatBezierCurve _azimuthAngleCurve;
    private readonly FloatBezierCurve _elevationCurve;
    private readonly FloatBezierCurve _radiusCurve;

    private readonly TimeSpan _animationDuration;

    private TimeSpan _playedDuration;

    private bool _wasFirstTickPlayed;

    private string _endPoseName;

    /// <summary>
    /// Value between 0 and 1 where 0 means that the animation just started and 1 that it has finished
    /// </summary>
    private float NormalizedAnimationTime => (float)(_playedDuration.TotalMilliseconds / _animationDuration.TotalMilliseconds);

    public CameraBezierAnimation(
        GameCameraManager cameraManager,
        Vector3BezierCurve positionCurve,
        FloatBezierCurve azimuthAngleCurve,
        FloatBezierCurve elevationCurve,
        FloatBezierCurve radiusCurve,
        TimeSpan animationDuration,
        TimeSpan delay,
        string endPoseName
    )
    {
        _cameraManager = cameraManager;

        _animationDuration = animationDuration;
        _playedDuration = TimeSpan.Zero.Subtract(delay);

        _positionCurve = positionCurve;
        _azimuthAngleCurve = azimuthAngleCurve;
        _elevationCurve = elevationCurve;
        _radiusCurve = radiusCurve;

        _wasFirstTickPlayed = false;

        _endPoseName = endPoseName;
    }

    public bool HasFinished() => _animationDuration <= _playedDuration;

    public void Update(TimeSpan passedTime)
    {
        UpdatePassedTime(passedTime);

        if (_playedDuration.TotalMilliseconds < 0) return;

        if (!_wasFirstTickPlayed)
        {
            BeforePlay();
            _wasFirstTickPlayed = true;
        }

        Update();

        if (HasFinished()) AfterPlay();
    }

    protected virtual void BeforePlay() 
    {
        _cameraManager.BeginAnimation();
    }

    protected virtual void AfterPlay()
    {
        _cameraManager.EndAnimation(_endPoseName);

    }
    

    private void Update()
    {
        var curNormalizedTime = Smooth(NormalizedAnimationTime);
        _cameraManager.SetTarget(_positionCurve.Eval(curNormalizedTime));
        _cameraManager.SetAzimuthAngle(_azimuthAngleCurve.Eval(curNormalizedTime));
        _cameraManager.SetElevationAngle(_elevationCurve.Eval(curNormalizedTime));
        _cameraManager.SetRadius(_radiusCurve.Eval(curNormalizedTime));
    }

    protected virtual float Smooth(float t) => t;

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }

    public void Dispose()
    {
    }

    public CameraPose GetEndPose()
    {
        return new CameraPose(
            _positionCurve.Knots[^1],
            _azimuthAngleCurve.Knots[^1],
            _elevationCurve.Knots[^1],
            _radiusCurve.Knots[^1]
        );
    }
}
