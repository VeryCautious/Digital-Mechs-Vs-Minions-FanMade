using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class CameraTargetMoveAnimation : CameraBezierAnimation
{
    public CameraTargetMoveAnimation(
        GameCameraManager cameraManager,
        CameraPose startPose,
        CameraPose endPose,
        TimeSpan animationDuration,
        TimeSpan delay,
        string endPoseName
    ) : base(
        cameraManager,
        Vector3BezierCurve.LinearCurve(startPose.Target, endPose.Target),
        FloatBezierCurve.ConstantCurve(startPose.AzimuthAngle),
        FloatBezierCurve.ConstantCurve(startPose.ElevationAngle),
        FloatBezierCurve.ConstantCurve(startPose.Radius),
        animationDuration,
        delay,
        endPoseName
    )
    { }
}
