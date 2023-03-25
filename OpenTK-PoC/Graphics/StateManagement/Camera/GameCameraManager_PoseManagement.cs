using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

internal partial class GameCameraManager
{
    private const string BirdEyePoseName = "BirdEyePose";
    private static string MechCameraPoseName(int i) => $"MechCameraPose{i}";

    private readonly CameraUniformSet _cameraUniformSet;

    private GameCameraMode _cameraMode;

    public GameCameraMode CameraMode => _cameraMode;

    public GameCameraManager(CameraUniformSet cameraUniformSet)
    {
        _cameraUniformSet = cameraUniformSet;
        Update();

        SetPose(BirdEyePoseName, new CameraPose(Vector3.Zero, 3 * MathF.PI / 2.0f, -MathF.PI / 3.0f, 8.0f));
        CurrentPoseName = BirdEyePoseName;

        SetPose(AnimationPoseName, new CameraPose(Vector3.Zero, 0, 0, 1));

        _temporaryPoses = new HashSet<string>();

        _cameraMode = GameCameraMode.BirdEye;
    }

    protected override void Update()
    {
        _cameraUniformSet.ViewUniform.Matrix = Pose.ViewMatrix;
        _cameraUniformSet.CameraPositionUniform.Vector = Pose.CameraPosition;
    }

    public void UpdatePerspective(float widthPerHeightRatio)
    {
        _cameraUniformSet.ProjectionUniform.Matrix =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, widthPerHeightRatio, 0.1f, 100f);
    }


    public (string Name, CameraPose Pose) GetThirdPersonPose(int mechIndex)
    {
        var name = MechCameraPoseName(mechIndex);
        if (!TryGetPose(name, out var pose)) throw new Exception($"{mechIndex} is out of range");
        return (name, pose);
    }

    public (string Name, CameraPose Pose) GetBirdEyePose()
    {
        if (!TryGetPose(BirdEyePoseName, out var pose)) throw new Exception($"{BirdEyePoseName} is not a valid camera pose name");
        return (BirdEyePoseName, pose);
    }

    public void ToggleCameraPose(WorldGraphicsState worldGraphicsState, GameState? gameState)
    {
        if (gameState is null) return;

        switch(_cameraMode)
        {
            case GameCameraMode.Fly:
                SetBirdEyeMode();
                return;
            case GameCameraMode.BirdEye:
                SetFlyMode(worldGraphicsState, gameState);
                return;
        }
    }

    public void SetBirdEyeMode()
    {
        _cameraMode = GameCameraMode.BirdEye;
        CurrentPoseName = BirdEyePoseName;
    }

    public void SetFlyMode(WorldGraphicsState worldGraphicsState, GameState gameState)
    {
        _cameraMode = GameCameraMode.Fly;
        UpdateCameraPoses(worldGraphicsState, gameState);
        if (gameState is not { GamePhase: MechPlayActionsPhase gamePhase }) return;
        CurrentPoseName = MechCameraPoseName(gamePhase.MechIndex);
    }

    public void UpdateCameraPoses(WorldGraphicsState worldGraphicsState, GameState gameState)
    {
        if (gameState is null) return;
        for (var i = 0; i < gameState.MechStates.Count; i++)
        {
            var (pos, azimuthAngle) = worldGraphicsState.GetMechWorldPose(gameState.MechStates[i]);
            pos -= Vector3.UnitY;
            SetPose(MechCameraPoseName(i), GetThirdPersonPose(pos, azimuthAngle));
        }
    }
    public static CameraPose GetThirdPersonPose(Vector3 pos, float azimuthAngle) => new CameraPose(
        pos,
        azimuthAngle,
        -MathF.PI / 4,
        1
    );
}

internal enum GameCameraMode
{
    Fly,
    BirdEye,
}
