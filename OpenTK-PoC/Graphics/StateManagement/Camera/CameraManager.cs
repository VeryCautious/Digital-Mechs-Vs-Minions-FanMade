using OpenTK.Mathematics;
using System.Diagnostics.CodeAnalysis;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

internal class CameraManager
{
    public const string DefaultPoseName = "Default";

    private string _currentPoseName;
    public string CurrentPoseName
    {
        get => _currentPoseName;
        set
        {
            if (!_poses.ContainsKey(value)) return;
            _currentPoseName = value;
            Update();
        }
    }

    private readonly Dictionary<string, CameraPose> _poses;

    public CameraPose Pose => _poses[CurrentPoseName];

    public CameraManager() : this(0, 0, 1, Vector3.Zero)
    {
    }

    public CameraManager(float azimuthAngle, float elevationAngle, float radius, Vector3 cameraTarget)
    {
        _poses = new Dictionary<string, CameraPose>
        {
            [DefaultPoseName] = new(cameraTarget, azimuthAngle, elevationAngle, radius)
        };
        _currentPoseName = DefaultPoseName;
    }

    public bool TryGetPose(string name, [MaybeNullWhen(false)] out CameraPose pose)
    {
        if (!_poses.TryGetValue(name, out var internPose))
        {
            pose = null;
            return false;
        }
        pose = internPose;
        return true;
    }

    public CameraPose GetDefaultPose() => _poses[DefaultPoseName];

    public void SetPose(string name, CameraPose pose) => _poses[name] = pose;

    public void UpdatePose(CameraPose pose)
    {
        _poses[CurrentPoseName] = pose;
        Update();
    }

    public void SetTarget(Vector3 target, PoseChangeReason pcr = PoseChangeReason.UserInput)
    {
        if (_currentPoseName == DefaultPoseName && pcr == PoseChangeReason.Animation) return;
        Pose.Target = target;
        Update();
    }

    public void SetAzimuthAngle(float angle, PoseChangeReason pcr = PoseChangeReason.UserInput)
    {
        if (_currentPoseName == DefaultPoseName && pcr == PoseChangeReason.Animation) return;
        Pose.AzimuthAngle = angle;
        Update();
    }

    public void SetElevationAngle(float angle, PoseChangeReason pcr = PoseChangeReason.UserInput)
    {
        if (_currentPoseName == DefaultPoseName && pcr == PoseChangeReason.Animation) return;
        Pose.ElevationAngle = angle;
        Update();
    }

    public void SetRadius(float radius, PoseChangeReason pcr = PoseChangeReason.UserInput)
    {
        if (_currentPoseName == DefaultPoseName && pcr == PoseChangeReason.Animation) return;
        Pose.Radius = radius;
        Update();
    }

    protected virtual void Update() { }
}
