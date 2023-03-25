using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

internal partial class GameCameraManager : CameraManager
{
    private const float RadiusSpeed = 0.1f;
    private const float AngleSpeed = MathF.PI / 60f;
    private const float TargetSpeed = 0.5f;

    public void MoveCounterClockwise(PoseChangeReason pcr = PoseChangeReason.UserInput)
        => SetAzimuthAngle(Pose.AzimuthAngle + AngleSpeed, pcr);

    public void MoveClockwise(PoseChangeReason pcr = PoseChangeReason.UserInput)
        => SetAzimuthAngle(Pose.AzimuthAngle - AngleSpeed, pcr);

    public void MoveDownOnSphere(PoseChangeReason pcr = PoseChangeReason.UserInput)
        => SetElevationAngle(Pose.ElevationAngle + AngleSpeed, pcr);

    public void MoveUpOnSphere(PoseChangeReason pcr = PoseChangeReason.UserInput)
        => SetElevationAngle(Pose.ElevationAngle - AngleSpeed, pcr);

    public void MoveOut(PoseChangeReason pcr = PoseChangeReason.UserInput)
        => SetRadius(Pose.Radius + RadiusSpeed, pcr);

    public void MoveIn(PoseChangeReason pcr = PoseChangeReason.UserInput)
        => SetRadius(Pose.Radius - RadiusSpeed, pcr);

    public void MoveTarget(Vector3 diff, PoseChangeReason pcr = PoseChangeReason.UserInput)
        => SetTarget(Pose.Target + TargetSpeed * diff, pcr);
}
