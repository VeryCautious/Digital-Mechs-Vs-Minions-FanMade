using System.Drawing;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions;
using OpenTK.Mathematics;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using System.Collections.Immutable;
using Mechs_Vs_Minions_Abstractions.Utilities;
using Mechs_Vs_Minions_Graphics.Utilities;
using Utilities;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement.Camera;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

public static class TransitionTranslation
{
    private static TimeSpan DefaultTransitionDuration => TimeSpan.FromSeconds(2.5);

    private static int GetFieldsMovedFrom(MoveTransition moveTransition) => Math.Abs(moveTransition.EndPosition.X - moveTransition.StartPosition.X) +
            Math.Abs(moveTransition.EndPosition.Y - moveTransition.StartPosition.Y);

    private static IAnimation GetPushAnimation(
        GameState gameState,
        TimeSpan originalDuration,
        int originalFieldsMoved,
        Func<Point, Vector3> worldSpaceTransform,
        Func<Guid, IInstanceMutator<MechInstance>> mechMutator,
        Translator<Guid, Handle<MechInstance>> mechHandleTranslator,
        IAudioStore audioStore,
        MoveTransition pushTransition
    ) 
    {
        var fieldsMoved = GetFieldsMovedFrom(pushTransition);
        var duration = TimeSpan.FromMilliseconds((originalDuration.TotalMilliseconds * fieldsMoved) / originalFieldsMoved);
        var delay = originalDuration - duration;
        return new LinearMoveAnimation<MechInstance>(
            duration,
            mechMutator(pushTransition.Id),
            audioStore,
            mechHandleTranslator.Get(pushTransition.Id),
            worldSpaceTransform(pushTransition.StartPosition),
            worldSpaceTransform(pushTransition.EndPosition),
            gameState.MechStates.Single(mech => mech.Id == pushTransition.Id).Orientation,
            delay
        );
    }

    private static IAnimation GetStompedAnimation(
        Func<Point, Vector3> worldSpaceTransform,
        Func<Guid, IInstanceMutator<MinionInstance>> minionMutator,
        Translator<Guid, Handle<MinionInstance>> mechHandleTranslator,
        MinionState stompedMinion,
        TimeSpan delay
    ) 
    {
        return new StompedAnimation<MinionInstance>(
            TimeSpan.FromSeconds(1), 
            minionMutator(stompedMinion.Id),
            mechHandleTranslator.Get(stompedMinion.Id),
            stompedMinion.Orientation,
            worldSpaceTransform(stompedMinion.Position),
            delay
        );
    }

    private static IList<IAnimation> GetCameraMoveAnimations(
        GameState gameState,
        Guid mechId,
        GameCameraManager cameraManager,
        Vector3 diffVector,
        TimeSpan moveAnimationDuration,
        TimeSpan cameraFlyAnimationDelay
    ) {
        var mechIndex = CurrentMechIndex(gameState, mechId);
        var preMoveCamera = cameraManager.GetThirdPersonPose(mechIndex);
        var postMovePose = preMoveCamera.Pose with
        {
            Target = preMoveCamera.Pose.Target + diffVector,
        };
        var postSpeedPoseName = "postMovePose";
        cameraManager.SetTemporaryPose(postSpeedPoseName, postMovePose);

        var cameraPlayAnimation = cameraManager.PoseTransitionAnimation(
            preMoveCamera.Name,
            postSpeedPoseName,
            moveAnimationDuration,
            TimeSpan.Zero,
            PoseTransitionAnimationType.TargetLinear
        );

        var cameraFlyAnimation = CameraFlyAnimation(
            gameState,
            mechId,
            cameraManager,
            cameraFlyAnimationDelay,
            postSpeedPoseName
        );

        var animations = new List<IAnimation>();
        if (cameraPlayAnimation != null)
            animations.Add(cameraPlayAnimation);
        if (cameraFlyAnimation != null)
            animations.Add(cameraFlyAnimation);
        return animations;
    }

    private static IList<IAnimation> GetCameraRotateAnimations(
        GameState gameState,
        Guid mechId,
        GameCameraManager cameraManager,
        float diffAngle,
        TimeSpan rotateAnimationDuration,
        TimeSpan cameraFlyAnimationDelay
    )
    {
        var mechIndex = CurrentMechIndex(gameState, mechId);
        var preRotateCamera = cameraManager.GetThirdPersonPose(mechIndex);
        var postMovePose = preRotateCamera.Pose with
        {
            AzimuthAngle = preRotateCamera.Pose.AzimuthAngle + diffAngle,
        };
        var postRotatePoseName = "postRotatePose";
        cameraManager.SetTemporaryPose(postRotatePoseName, postMovePose);

        var cameraPlayAnimation = cameraManager.PoseTransitionAnimation(
            preRotateCamera.Name,
            postRotatePoseName,
            rotateAnimationDuration,
            TimeSpan.Zero,
            PoseTransitionAnimationType.AzimuthLinear
        );

        var cameraFlyAnimation = CameraFlyAnimation(
            gameState,
            mechId,
            cameraManager,
            cameraFlyAnimationDelay,
            postRotatePoseName
        );

        var animations = new List<IAnimation>();
        if (cameraPlayAnimation != null)
            animations.Add(cameraPlayAnimation);
        if (cameraFlyAnimation != null)
            animations.Add(cameraFlyAnimation);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimation(this UseOmniStompTransition useOmniStompTransition, 
        GameState gameState, 
        Func<Point,Vector3> worldSpaceTransform, 
        Func<Guid,IInstanceMutator<MechInstance>> mechMutator, 
        Translator<Guid,Handle<MechInstance>> mechHandleTranslator,
        Func<Guid, IInstanceMutator<MinionInstance>> minionMutator,
        Translator<Guid, Handle<MinionInstance>> minionHandleTranslator,
        IAudioStore audioStore,
        GameCameraManager cameraManager
    ) {
        var mech = gameState.MechStates.Single(mech => mech.Id == useOmniStompTransition.MechId);
        var startPosition = worldSpaceTransform(useOmniStompTransition.StartPosition);
        var endPosition = worldSpaceTransform(useOmniStompTransition.EndPosition);

        var moveAnimation = new LinearMoveAnimation<MechInstance>(
            DefaultTransitionDuration,
            mechMutator(mech.Id),
            audioStore,
            mechHandleTranslator.Get(mech.Id),
            startPosition,
            endPosition,
            mech.Orientation
        );

        var pushAnimations = useOmniStompTransition.Pushes
            .Select(pushTransition => GetPushAnimation(
                gameState,
                DefaultTransitionDuration,
                GetFieldsMovedFrom(useOmniStompTransition),
                worldSpaceTransform,
                mechMutator,
                mechHandleTranslator,
                audioStore,
                pushTransition
            )
        );

        var stompMinions = useOmniStompTransition.StompedMinions
            .Select(minion => GetStompedAnimation(
                worldSpaceTransform,
                minionMutator,
                minionHandleTranslator,
                minion,
                DefaultTransitionDuration
            )
        );

        var cameraAnimations = GetCameraMoveAnimations(
            gameState,
            useOmniStompTransition.MechId,
            cameraManager,
            endPosition - startPosition,
            DefaultTransitionDuration,
            DefaultTransitionDuration
        );

        var animations = new List<IAnimation>();
        animations.Add(moveAnimation);
        animations.AddRange(pushAnimations);
        animations.AddRange(stompMinions);
        animations.AddRange(cameraAnimations);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimation(this UseSpeedTransition useSpeedTransition, 
        GameState gameState, 
        Func<Point,Vector3> worldSpaceTransform, 
        Func<Guid,IInstanceMutator<MechInstance>> mechMutator, 
        Translator<Guid,Handle<MechInstance>> mechHandleTranslator, 
        Func<Guid,IInstanceMutator<MinionInstance>> minionMutator, 
        Translator<Guid,Handle<MinionInstance>> minionHandleTranslator, 
        IAudioStore audioStore,
        GameCameraManager cameraManager
    ) {
        var mech = gameState.MechStates.Single(mech => mech.Id == useSpeedTransition.MechId);

        var startPosition = worldSpaceTransform(useSpeedTransition.StartPosition);
        var endPosition = worldSpaceTransform(useSpeedTransition.EndPosition);

        var moveAnimation = new LinearMoveAnimation<MechInstance>(
            DefaultTransitionDuration,
            mechMutator(mech.Id),
            audioStore,
            mechHandleTranslator.Get(mech.Id),
            startPosition,
            endPosition,
            mech.Orientation
        );

        var pushAnimations = useSpeedTransition.Pushes
            .Select(pushTransition => GetPushAnimation(
                gameState,
                DefaultTransitionDuration,
                GetFieldsMovedFrom(useSpeedTransition),
                worldSpaceTransform,
                mechMutator,
                mechHandleTranslator,
                audioStore,
                pushTransition
            )
        );

        var stompMinions = useSpeedTransition.StompedMinions
            .Select(minion => GetStompedAnimation(
                worldSpaceTransform,
                minionMutator,
                minionHandleTranslator,
                minion,
                DefaultTransitionDuration
            )
        );

        var cameraAnimations = GetCameraMoveAnimations(
            gameState,
            useSpeedTransition.MechId,
            cameraManager,
            endPosition - startPosition,
            DefaultTransitionDuration,
            DefaultTransitionDuration
        );

        var animations = new List<IAnimation>();
        animations.Add(moveAnimation);
        animations.AddRange(pushAnimations);
        animations.AddRange(stompMinions);
        animations.AddRange(cameraAnimations);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimation(this UseBlazeTransition useBlazeTransition, 
        GameState gameState, 
        Func<Point,Vector3> worldSpaceTransform, 
        Func<Guid,IInstanceMutator<MechInstance>> mechMutator, 
        Translator<Guid,Handle<MechInstance>> mechHandleTranslator, 
        Func<Guid,IInstanceMutator<MinionInstance>> minionMutator, 
        Translator<Guid,Handle<MinionInstance>> minionHandleTranslator, 
        IInstanceFactory<ParticleInstance> particleFactory, 
        IAudioStore audioStore,
        GameCameraManager cameraManager
    ) {
        var mech = gameState.MechStates.Single(mech => mech.Id == useBlazeTransition.MechId);
        var moveDuration = TimeSpan.FromMilliseconds(0.3f * DefaultTransitionDuration.TotalMilliseconds);
        var fireDuration = DefaultTransitionDuration.Subtract(moveDuration);
        var startPosition = worldSpaceTransform(useBlazeTransition.StartPosition);
        var endPosition = worldSpaceTransform(useBlazeTransition.EndPosition);

        var moveAnimation = new LinearMoveAnimation<MechInstance>(
            moveDuration,
            mechMutator(mech.Id),
            audioStore,
            mechHandleTranslator.Get(mech.Id),
            startPosition,
            endPosition,
            mech.Orientation
        );

        var fireTranslation = -0.5f * Vector3.UnitY;

        var fireAnimationLeft = new FireAnimation(
            fireDuration,
            particleFactory,
            audioStore,
            endPosition + fireTranslation,
            mech.Orientation.OrientationIn(Direction.Left),
            moveDuration
        );

        var fireAnimationRight = new FireAnimation(
            fireDuration,
            particleFactory,
            audioStore,
            endPosition + fireTranslation,
            mech.Orientation.OrientationIn(Direction.Right),
            moveDuration
        );

        var pushAnimations = useBlazeTransition.Pushes
            .Select(pushTransition => GetPushAnimation(
                gameState,
                moveDuration,
                GetFieldsMovedFrom(useBlazeTransition),
                worldSpaceTransform,
                mechMutator,
                mechHandleTranslator,
                audioStore,
                pushTransition
            )
        );

        var stompMinionAnimations = useBlazeTransition.StompedMinions.
            Union(useBlazeTransition.BurnedMinions).
            Select(minion => GetStompedAnimation(
                worldSpaceTransform,
                minionMutator,
                minionHandleTranslator,
                minion,
                DefaultTransitionDuration
            )
        );

        var smokeTranslation = -.4f* Vector3.UnitY;
        var smokeDuration = DefaultTransitionDuration;
        var burnMinionsAnimations = useBlazeTransition.BurnedMinions.
            Select(minion => new SmokeAnimation(
                smokeDuration, 
                particleFactory,
                audioStore,
                worldSpaceTransform(minion.Position) + smokeTranslation,
                DefaultTransitionDuration
            )
        );

        var cameraAnimations = GetCameraMoveAnimations(
            gameState,
            useBlazeTransition.MechId,
            cameraManager,
            endPosition - startPosition,
            moveDuration,
            DefaultTransitionDuration.Add(smokeDuration / 2)
        );

        var animations = new List<IAnimation>();
        animations.Add(moveAnimation);
        animations.Add(fireAnimationLeft);
        animations.Add(fireAnimationRight);
        animations.AddRange(pushAnimations);
        animations.AddRange(stompMinionAnimations);
        animations.AddRange(burnMinionsAnimations);
        animations.AddRange(cameraAnimations);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimation(this UseScytheTransition useScytheTransition, 
        GameState gameState,
        Func<Point,Vector3> worldSpaceTransform,
        IInstanceMutator<MechInstance> mechMutator,
        Handle<MechInstance> mechHandle,
        IInstanceFactory<ParticleInstance> particleFactory,
        Func<Guid,IInstanceMutator<MinionInstance>> minionMutator,
        Translator<Guid,Handle<MinionInstance>> minionHandleTranslator,
        IAudioStore audioStore,
        GameCameraManager cameraManager
    ) {
        var mech = gameState.MechStates.Single(mech => mech.Id == useScytheTransition.MechId);
        var scytheTranslation = -0.5f * Vector3.UnitY;

        var diffAngle = OrientationExtensions.AngleBetweenOrientations(useScytheTransition.EndOrientation, useScytheTransition.StartOrientation);

        var rotateAnimation = new LinearRotationAnimation<MechInstance>(
            DefaultTransitionDuration,
            mechMutator,
            mechHandle,
            useScytheTransition.StartOrientation,
            useScytheTransition.EndOrientation,
            worldSpaceTransform(mech.Position)
        );

        var scytheAnimation = new ScytheAnimation(
            DefaultTransitionDuration,
            particleFactory,
            audioStore,
            worldSpaceTransform(mech.Position) + scytheTranslation,
            Math.Sign(diffAngle)
        );

        var slicedAnimations = useScytheTransition.SlicedMinions.
            Select(minion => GetStompedAnimation(
                worldSpaceTransform,
                minionMutator,
                minionHandleTranslator,
                minion,
                DefaultTransitionDuration
            )
        );

        var cameraAnimations = GetCameraRotateAnimations(
            gameState,
            useScytheTransition.MechId,
            cameraManager,
            diffAngle,
            DefaultTransitionDuration,
            DefaultTransitionDuration
        );

        var animations = new List<IAnimation>();
        animations.Add(rotateAnimation);
        animations.Add(scytheAnimation);
        animations.AddRange(slicedAnimations);
        animations.AddRange(cameraAnimations);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimation(this UseStuckControlsTransition useStuckControlsTransition,
        GameState gameState,
        Func<Point,Vector3> worldSpaceTransform,
        IInstanceMutator<MechInstance> mechMutator,
        Handle<MechInstance> mechHandle,
        GameCameraManager cameraManager
    ) {
        var mech = gameState.MechStates.Single(mech => mech.Id == useStuckControlsTransition.MechId);

        var rotateAnimation = new LinearRotationAnimation<MechInstance>(
            DefaultTransitionDuration,
            mechMutator,
            mechHandle,
            useStuckControlsTransition.StartOrientation,
            useStuckControlsTransition.EndOrientation,
            worldSpaceTransform(mech.Position)
        );

        var cameraAnimations = GetCameraRotateAnimations(
            gameState,
            useStuckControlsTransition.MechId,
            cameraManager,
            OrientationExtensions.AngleBetweenOrientations(useStuckControlsTransition.EndOrientation, useStuckControlsTransition.StartOrientation),
            DefaultTransitionDuration,
            DefaultTransitionDuration
        );

        var animations = new List<IAnimation>();
        animations.Add(rotateAnimation);
        animations.AddRange(cameraAnimations);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimation(this ApplyGlitchDamageToMechTransition applyGlitchTransition, 
        GameState gameState, 
        Func<Point,Vector3> worldSpaceTransform,
        IInstanceFactory<ParticleInstance> particleFactory, 
        IAudioStore audioStore
    ) {
        var mech = gameState.MechStates.Single(mech => mech.Id == applyGlitchTransition.MechId);
        var smokeTranslation = -Vector3.UnitY;
        var smokeDuration = 2f * DefaultTransitionDuration;

        var smokeAnimation = new SmokeAnimation(
            smokeDuration, 
            particleFactory,
            audioStore,
            worldSpaceTransform(mech.Position) + smokeTranslation,
            TimeSpan.Zero
        );

        return new IAnimation[] { smokeAnimation };
    }

    internal static IEnumerable<IAnimation> GetAnimation(this UseAimBotTransition useAimBotTransition,
        GameState gameState,
        Func<Point,Vector3> worldSpaceTransform, 
        IInstanceFactory<ParticleInstance> particleFactory, 
        Func<Guid,IInstanceMutator<MinionInstance>> minionMutator,
        Translator<Guid,Handle<MinionInstance>> minionHandleTranslator,
        IAudioStore audioStore,
        GameCameraManager cameraManager
    ) {
        var elevation = -.5f * Vector3.UnitY;

        var shootAnimation = new AimBotAnimation(
            DefaultTransitionDuration,
            particleFactory,
            audioStore,
            worldSpaceTransform(useAimBotTransition.StartLocation) + elevation,
            worldSpaceTransform(useAimBotTransition.TargetLocation) + elevation
        );

        var shotMinionsAnimation = useAimBotTransition.ShotMinions.
            Select(minion => new LinearMoveAnimation<MinionInstance>(
                TimeSpan.FromSeconds(1), 
                minionMutator(minion.Id),
                minionHandleTranslator.Get(minion.Id),
                worldSpaceTransform(minion.Position), 
                worldSpaceTransform(minion.Position)+ 10f*(worldSpaceTransform(minion.Position)-worldSpaceTransform(useAimBotTransition.StartLocation)).Normalized(),
                minion.Orientation,
                DefaultTransitionDuration-TimeSpan.FromMilliseconds(200)
            )
        );

        var cameraFlyDelay = DefaultTransitionDuration - TimeSpan.FromMilliseconds(200) + TimeSpan.FromSeconds(1);
        var cameraFlyAnimation = CameraFlyAnimation(
            gameState,
            useAimBotTransition.MechId,
            cameraManager,
            cameraFlyDelay,
            null
        );

        var animations = new List<IAnimation>();
        animations.Add(shootAnimation);
        animations.AddRange(shotMinionsAnimation);
        if (cameraFlyAnimation != null)
            animations.Add(cameraFlyAnimation);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimation(
        this ApplyStuckControlsToMechTransition applyStuckControlsTransition,
        GameState gameState, Func<Point,Vector3> worldSpaceTransform,
        IInstanceFactory<ParticleInstance> particleFactory,
        IAudioStore audioStore
    )
    {
        var mech = gameState.MechStates.Single(mech => mech.Id == applyStuckControlsTransition.MechId);
        var smokeTranslation = -Vector3.UnitY;
        var smokeDuration = 2f * DefaultTransitionDuration;

        var smoke = new SmokeAnimation(
            smokeDuration, 
            particleFactory,
            audioStore,
            worldSpaceTransform(mech.Position) + smokeTranslation,
            TimeSpan.Zero
        );

        return new IAnimation[] { smoke };
    }

    internal static IEnumerable<IAnimation> GetAnimation(this UseRipSawTransition useRipsawTransition, 
        GameState gameState,
        Func<Guid, IInstanceMutator<MinionInstance>> minionMutator,
        Translator<Guid, Handle<MinionInstance>> minionHandleTranslator,
        Func<Point,Vector3> worldSpaceTransform, 
        IInstanceFactory<RipSawInstance> ripSawFactory,
        IAudioStore audioStore,
        GameCameraManager cameraManager
    ) {
        var ripSawTranslation = -0.5f * Vector3.UnitY;
        var boardSpaceDistance = useRipsawTransition.StartPosition.DistanceTo(useRipsawTransition.EndPosition);
        var duration = DefaultTransitionDuration / 5f * boardSpaceDistance;

        var ripsawAnimation = new RipsawAnimation(
            duration,
            ripSawFactory,
            audioStore,
            worldSpaceTransform(useRipsawTransition.StartPosition) + ripSawTranslation,
            worldSpaceTransform(useRipsawTransition.EndPosition) + ripSawTranslation
        );

        var slicedAnimations = useRipsawTransition.SlicedMinions.
            Select(minion => GetStompedAnimation(
                worldSpaceTransform,
                minionMutator,
                minionHandleTranslator,
                minion,
                DefaultTransitionDuration
            )
        );

        var cameraFlyAnimation = CameraFlyAnimation(
            gameState,
            useRipsawTransition.MechId,
            cameraManager,
            DefaultTransitionDuration,
            null
        );

        var animations = new List<IAnimation>();
        animations.Add(ripsawAnimation);
        animations.AddRange(slicedAnimations);
        if (cameraFlyAnimation != null)
            animations.Add(cameraFlyAnimation);
        return animations;
    }

    internal static IEnumerable<IAnimation> GetAnimations(
        this MinionMoveTransition minionMoveTransition, 
        GameState gameState, 
        Func<Point, Vector3> worldSpaceTransform,
        Func<Guid, IInstanceMutator<MinionInstance>> getMinionMutator,
        IImmutableList<Handle<MinionInstance>> minionHandleList,
        IAudioStore audioStore
    ) {
        var animations = new List<IAnimation>();
        for (var i = 0; i < minionMoveTransition.MoveTransitions.Count; i++) 
        {
            var minion = gameState.MinionStates.Single(minion => minion.Id == minionMoveTransition.MoveTransitions[i].Id);
            animations.Add(new LinearMoveAnimation<MinionInstance>(
                DefaultTransitionDuration,
                getMinionMutator(minionMoveTransition.MoveTransitions[i].Id),
                audioStore,
                minionHandleList[i],
                worldSpaceTransform(minionMoveTransition.MoveTransitions[i].StartPosition),
                worldSpaceTransform(minionMoveTransition.MoveTransitions[i].EndPosition),
                minion.Orientation
            ));
        }

        return animations.ToArray();
    }

    internal static IEnumerable<IAnimation> GetAnimations(this StartOfPlayActionPhaseTransition _,
        GameCameraManager cameraManager
    )
    {
        var moveToNextMechAnimation = cameraManager.PoseTransitionAnimation(
            cameraManager.CurrentPoseName,
            cameraManager.GetThirdPersonPose(0).Name,
            TimeSpan.FromSeconds(2.5),
            TimeSpan.Zero,
            PoseTransitionAnimationType.TargetBezier
        );

        var animations = new List<IAnimation>();
        if (moveToNextMechAnimation != null)
            animations.Add(moveToNextMechAnimation);

        return animations;
    }

    private static int CurrentMechIndex(GameState gameState, Guid mechId)
    {
        return gameState.MechStates.IndexOf(gameState.MechStates.Single(otherMech => mechId.Equals(otherMech.Id)));
    }

    /// <summary>
    /// Returns the index of the Mech that plays the next card in this round.
    /// If this or the next game phase is not a MechPlayActionPhase -1 is returned.
    /// </summary>
    /// <param name="gameState"></param>
    /// <returns></returns>
    private static int NextMechIndex(GameState gameState)
    {
        if (gameState.GamePhase is not MechPlayActionsPhase playActionsPhase) return -1;

        var mechState = gameState.MechStates[playActionsPhase.MechIndex];
        var lastNonEmptySlotIndex = mechState.CommandLine.Cards.Keys.Max();

        if (playActionsPhase.SlotToPlay <= lastNonEmptySlotIndex) return playActionsPhase.MechIndex;

        if (playActionsPhase.MechIndex == gameState.MechStates.Count - 1) return -1;

        return playActionsPhase.MechIndex + 1;
    }

    private static CameraBezierAnimation? CameraFlyAnimation(
        GameState gameState, 
        Guid mechId,
        GameCameraManager cameraManager, 
        TimeSpan delay, 
        string? startPoseName)
    {
        var nextMechIndex = NextMechIndex(gameState);
        var curMechIndex = CurrentMechIndex(gameState, mechId);
        if (nextMechIndex == curMechIndex) return null;

        var flyStartPoseName = startPoseName ?? CameraPoseNameOf(curMechIndex, gameState, cameraManager);
        var flyEndPoseName = CameraPoseNameOf(nextMechIndex, gameState, cameraManager);

        return cameraManager.PoseTransitionAnimation(
            flyStartPoseName,
            flyEndPoseName,
            TimeSpan.FromSeconds(2.5),
            delay,
            PoseTransitionAnimationType.TargetBezier
        );
    }

    private static string CameraPoseNameOf(int mechIndex, GameState gameState, GameCameraManager cameraManager)
    {
        if (mechIndex < 0 || mechIndex >= gameState.MechStates.Count)
            return cameraManager.GetBirdEyePose().Name;
        return cameraManager.GetThirdPersonPose(mechIndex).Name;
    }

}