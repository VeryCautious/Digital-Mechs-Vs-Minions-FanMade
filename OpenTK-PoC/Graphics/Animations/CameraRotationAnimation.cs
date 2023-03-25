using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class CameraRotationAnimation : CameraBezierAnimation
{
    public CameraRotationAnimation(
        GameCameraManager cameraManager,
        CameraPose startPose,
        CameraPose endPose,
        TimeSpan animationDuration,
        TimeSpan delay,
        string endPoseName
    ) : base(
        cameraManager,
        Vector3BezierCurve.ConstantCurve(startPose.Target),
        GetAzimuthCurve(startPose, endPose),
        FloatBezierCurve.ConstantCurve(startPose.ElevationAngle),
        FloatBezierCurve.ConstantCurve(startPose.Radius),
        animationDuration,
        delay,
        endPoseName
    )
    { }

    private static FloatBezierCurve GetAzimuthCurve(CameraPose startPose, CameraPose endPose)
    {
        var startAngle = startPose.AzimuthAngle;
        var endAngle = endPose.AzimuthAngle;
        var diff = endAngle - startAngle;
        if (diff < -MathF.PI)
        {
            diff += MathF.Tau;
        }
        if (diff > MathF.PI)
        {
            diff -= MathF.Tau;
        }
        return FloatBezierCurve.LinearCurve(startAngle, startAngle + diff);
    }
}

