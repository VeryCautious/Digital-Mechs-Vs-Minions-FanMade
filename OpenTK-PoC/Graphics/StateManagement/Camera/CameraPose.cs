using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

internal sealed record CameraPose
{
    private Vector3 _target;
    public Vector3 Target
    {
        get { return _target; }
        set
        {
            _target = value;
            Update();
        }
    }

    private float _azimuthAngle;
    public float AzimuthAngle
    {
        get { return _azimuthAngle; }
        set
        {
            _azimuthAngle = value;
            if (_azimuthAngle < 0) _azimuthAngle += MathF.Tau;
            if (_azimuthAngle > MathF.Tau) _azimuthAngle -= MathF.Tau;
            Update();
        }
    }

    private float _elevationAngle;
    public float ElevationAngle
    {
        get { return _elevationAngle; }
        set
        {
            _elevationAngle = Math.Max(Math.Min(value, MathF.PI / 2 - 1e-5f), -MathF.PI / 2 + 1e-5f);
            Update();
        }
    }


    private float _radius;
    public float Radius
    {
        get { return _radius; }
        set
        {
            _radius = Math.Max(0, value);
            Update();
        }
    }

    public Vector3 CameraPosition { get; private set; }
    public Matrix4 ViewMatrix { get; private set; }

    public CameraPose(Vector3 target, float azimuthAngle, float elevationAngle, float radius)
    {
        _target = target;
        _azimuthAngle = azimuthAngle;
        _elevationAngle = elevationAngle;
        _radius = radius;

        Update();
    }

    private void Update()
    {
        CameraPosition = Target + new Vector3(
            Radius * (float)Math.Cos(ElevationAngle) * (float)Math.Cos(AzimuthAngle),
            Radius * (float)Math.Sin(ElevationAngle),
            Radius * (float)Math.Cos(ElevationAngle) * (float)Math.Sin(AzimuthAngle)
        );

        var cameraDirection = Vector3.Normalize(CameraPosition - Target);
        var cameraRight = Vector3.Normalize(Vector3.Cross(-Vector3.UnitY, cameraDirection));
        var cameraUp = Vector3.Cross(cameraDirection, cameraRight);
        ViewMatrix = Matrix4.LookAt(CameraPosition, Target, cameraUp);
    }
}
