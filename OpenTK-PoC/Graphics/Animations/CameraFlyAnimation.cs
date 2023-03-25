using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class CameraFlyAnimation : CameraBezierAnimation
{
    public CameraFlyAnimation(
        GameCameraManager cameraManager,
        CameraPose startPose,
        CameraPose endPose,
        TimeSpan animationDuration,
        TimeSpan delay,
        string endPoseName
    ) : base(
        cameraManager,
        CreatePositionCurve(startPose, endPose),
        FloatBezierCurve.LinearCurve(startPose.AzimuthAngle, endPose.AzimuthAngle),
        FloatBezierCurve.LinearCurve(startPose.ElevationAngle, endPose.ElevationAngle),
        FloatBezierCurve.LinearCurve(startPose.Radius, endPose.Radius),
        animationDuration,
        delay,
        endPoseName
    )
    { }

    private static Vector3BezierCurve CreatePositionCurve(CameraPose startPose, CameraPose endPose)
    {
        return new Vector3BezierCurve(new List<Vector3>
        {
            startPose.Target,
            startPose.Target + startPose.CameraPosition - startPose.Target,
            endPose.Target + endPose.CameraPosition - endPose.Target,
            endPose.Target
        });
    }

    protected override float Smooth(float t) => t * t * t * (t * (t * 6 - 15) + 10);
}
