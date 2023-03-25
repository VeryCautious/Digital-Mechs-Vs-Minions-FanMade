using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class FireAnimation : IAnimation
{
    private readonly IInstanceFactory<ParticleInstance> _instanceFactory;
    private readonly IAudioStore _audioStore;
    private bool _soundPlayed;
    private readonly Particle[] _particles;
    private readonly Vector3 _position;
    private readonly Orientation _fireOrientation;
    private readonly TimeSpan _animationDuration;
    private TimeSpan _playedDuration;
    private readonly Random _random = new();

    private class Particle
    {
        public readonly Handle<ParticleInstance> InstanceHandle;
        public float LifeTime;
        public Vector3 Velocity;
        public Vector3 Position;
        public Color4 Color;

        public Particle(Handle<ParticleInstance> instanceHandle, Vector3 position, Color4 color)
        {
            InstanceHandle = instanceHandle;
            Position = position;
            LifeTime = 0;
            Color = color;
        }
    }

    public FireAnimation(TimeSpan animationDuration, IInstanceFactory<ParticleInstance> instanceFactory, IAudioStore audioStore, Vector3 position, Orientation fireOrientation, TimeSpan delay, int particleCount = 500)
    {
        _soundPlayed = false;
        _instanceFactory = instanceFactory;
        _audioStore = audioStore;
        _position = position;
        _playedDuration = TimeSpan.Zero.Subtract(delay);
        _fireOrientation = fireOrientation;
        _animationDuration = animationDuration;
        _particles = Enumerable.
            Range(0, particleCount).
            Select(_ => new Particle(_instanceFactory.CreateInstance(() => new ParticleInstance(Matrix4.CreateTranslation(position), Color4.Red)), position, Color4.Red)).
            ToArray();
    }

    public void Dispose()
    {
        foreach (var particle in _particles)
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

        foreach (var particle in _particles)
        {
            if (particle.LifeTime <= float.Epsilon)
            {
                Respawn(particle);
            }

            Update(particle, passedTime);
            _instanceFactory.Update(
                particle.InstanceHandle,
                _ => new ParticleInstance(Matrix4.CreateTranslation(particle.Position), particle.Color)
            );
        }

    }
    
    private byte RandomColorChannel() => (byte)(255f * _random.NextSingle());

    private void Respawn(Particle particle)
    {
        particle.LifeTime = _random.NextSingle() * 1500.0f;
        
        particle.Color = new Color4(RandomColorChannel(), 0, 0, 255);

        var velocity = new Vector3(1.0f, _random.NextSingle() - 0.5f, _random.NextSingle() - 0.5f);
        var angle = -(int)_fireOrientation * ((float)Math.PI / 2.0f);
        particle.Velocity = Matrix3.CreateRotationY(angle) * velocity;
        particle.Velocity.NormalizeFast();
        var speed = _random.NextSingle();
        particle.Velocity = Vector3.Multiply(particle.Velocity, speed);

        particle.Position = _position;
    }

    private static void Update(Particle particle, TimeSpan dt)
    {
        particle.LifeTime -= (float)dt.TotalMilliseconds;
        particle.Position += Vector3.Multiply(particle.Velocity, (float)dt.TotalSeconds);
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }
}