using Mechs_Vs_Minions_Graphics.Graphics.Animations;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

internal partial class GameCameraManager
{
    private const string AnimationPoseName = "AnimationPose";
    private ISet<string> _temporaryPoses;

    public void SetTemporaryPose(string name, CameraPose pose)
    {
        SetPose(name, pose);
        _temporaryPoses.Add(name);
    }

    public void BeginAnimation()
    {
        CurrentPoseName = AnimationPoseName;
    }

    public void EndAnimation(string endPoseName)
    {
        DeleteTemporaryPosesExpectFor(endPoseName);
        CurrentPoseName = endPoseName;
    }

    private void DeleteTemporaryPosesExpectFor(string poseName)
    {
        _temporaryPoses = _temporaryPoses.Where(pName => pName != poseName).ToHashSet();
    }

    public CameraBezierAnimation? PoseTransitionAnimation(string startPoseName, string endPoseName, TimeSpan duration, TimeSpan delay, PoseTransitionAnimationType animationKind)
    {
        if (CameraMode != GameCameraMode.Fly) return null;

        if (!TryGetPose(startPoseName, out var startPose)) throw new ArgumentException($"invalid pose name: {startPoseName}");
        if (!TryGetPose(endPoseName, out var endPose)) throw new ArgumentException($"invalid pose name: {endPoseName}");

        return animationKind switch
        {
            PoseTransitionAnimationType.TargetLinear => new CameraTargetMoveAnimation(this, startPose, endPose, duration, delay, endPoseName),
            PoseTransitionAnimationType.TargetBezier => new CameraFlyAnimation(this, startPose, endPose, duration, delay, endPoseName),
            PoseTransitionAnimationType.AzimuthLinear => new CameraRotationAnimation(this, startPose, endPose, duration, delay, endPoseName),
            _ => throw new ArgumentException("kind of animation is not known.")
        };
    }
}

public enum PoseTransitionAnimationType
{
    TargetLinear,
    TargetBezier,
    AzimuthLinear,
}
