using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;
using System.Drawing;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class AimBotAnimation : IAnimation
{
    private class Particle
    {
        public readonly Handle<ParticleInstance> InstanceHandle;
        public Vector3 Position;
        public Vector3 Velocity;
        public Color4 Color;

        public Particle(Handle<ParticleInstance> instanceHandle, Vector3 position, Vector3 velocity, Color4 color)
        {
            InstanceHandle = instanceHandle;
            Velocity = velocity;
            Color = color;
            Position = position;
        }
    }

    /// <summary>
    /// Value between 0 and 1 where 0 means that the animation just started and 1 that it has finished
    /// </summary>
    private float NormalizedAnimationTime => (float)(_playedDuration.TotalMilliseconds / _animationDuration.TotalMilliseconds);


    private readonly IInstanceFactory<ParticleInstance> _instanceFactory;
    private readonly IAudioStore _audioStore;
    private bool _soundPlayed;
    private readonly Particle[] _shotParticles;
    private readonly Particle[] _aimParticles;
    private readonly TimeSpan _animationDuration;
    private TimeSpan _playedDuration;
    private readonly Random _random = new();

    public AimBotAnimation(TimeSpan animationDuration, IInstanceFactory<ParticleInstance> instanceFactory, IAudioStore audioStore, Vector3 startPosition, Vector3 targetPosition, int particleCount = 300)
    {
        _soundPlayed = false;
        _instanceFactory = instanceFactory;
        _audioStore = audioStore;
        var startToEndPosition = targetPosition-startPosition;
        _playedDuration = TimeSpan.Zero;
        _animationDuration = animationDuration;
        _shotParticles = Enumerable.
            Range(0, particleCount).
            Select(_ => new Particle(
                _instanceFactory.CreateInstance(ParticleInstance.DefaultFactory(Color.Green)),
                startPosition, 
                Vector3.Divide(startToEndPosition, (float)animationDuration.TotalSeconds * .6f) + GetRandomOffset(),
                new Color4(0, RandomColorChannel(), 0, 255)
            )).
            ToArray();

        _aimParticles = Enumerable.
            Range(0, particleCount).Select(i => new Particle(
                _instanceFactory.CreateInstance(ParticleInstance.DefaultFactory(Color.Red)),
                startPosition + (startToEndPosition * (i / (float)particleCount)),
                Vector3.Zero,
                new Color4(255, 0, 0, 255)
            )).
            ToArray();
    }

    private byte RandomColorChannel() => (byte)(255f * _random.NextSingle());
    private Vector3 GetRandomOffset() => new Vector3(_random.NextSingle(), _random.NextSingle(), _random.NextSingle()) * 0.2f;

    public void Dispose()
    {
        foreach (var particle in _shotParticles.Union(_aimParticles))
        {
            _instanceFactory.DestroyInstance(particle.InstanceHandle);
        }
    }

    public bool HasFinished() => _animationDuration <= _playedDuration;

    public void Update(TimeSpan passedTime)
    {
        UpdatePassedTime(passedTime);

        if (_playedDuration.TotalMilliseconds < 0) return;
        
        if (!_soundPlayed)
        {
            _audioStore.Get(AudioHandle.FireBurst).Play();
            _soundPlayed = true;
        }
        
        foreach (var particle in _aimParticles)
        {
            Update(particle, passedTime);
            _instanceFactory.Update(
                particle.InstanceHandle, 
                inst => inst with
                {
                    ModelTransform = Matrix4.CreateScale(.3f) * Matrix4.CreateTranslation(particle.Position),
                    Color = particle.Color
                }
            );
        }

        if (NormalizedAnimationTime <= 0.3)
        {
            return;
        }

        foreach (var particle in _shotParticles)
        {
            Update(particle, passedTime);
            _instanceFactory.Update(
                particle.InstanceHandle, 
                inst => inst with
                {
                    ModelTransform = Matrix4.CreateTranslation(particle.Position),
                    Color = particle.Color
                }
            );
        }
    }

    private static void Update(Particle particle, TimeSpan dt)
    {
        particle.Position += Vector3.Multiply(particle.Velocity, (float)dt.TotalSeconds);
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }
}