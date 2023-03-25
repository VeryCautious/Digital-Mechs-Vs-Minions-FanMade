using System.Drawing;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class ScytheAnimation : IAnimation
{
    private readonly IInstanceFactory<ParticleInstance> _instanceFactory;
    private readonly IAudioStore _audioStore;
    private bool _soundPlayed;
    private readonly Particle[] _particles;
    private readonly Vector3 _position;
    private readonly float _startAngle;
    private readonly float _startToEndAngle;
    private readonly TimeSpan _animationDuration;
    private TimeSpan _playedDuration;
    private readonly Random _random = new();
    private const float Radius = 1.0f;

    /// <summary>
    /// Value between 0 and 1 where 0 means that the animation just started and 1 that it has finished
    /// </summary>
    private float NormalizedAnimationTime => (float)(_playedDuration.TotalMilliseconds / _animationDuration.TotalMilliseconds);

    private class Particle
    {
        public readonly Handle<ParticleInstance> InstanceHandle;
        public float LifeTime;
        public Vector3 Position;
        public Color4 Color;

        public Particle(Handle<ParticleInstance> instanceHandle, Vector3 position)
        {
            InstanceHandle = instanceHandle;
            Position = position;
            Color = Color4.Blue;
            LifeTime = 0;
        }
    }

    public ScytheAnimation(TimeSpan animationDuration, IInstanceFactory<ParticleInstance> instanceFactory, IAudioStore audioStore, Vector3 position, int sign, int particleCount = 500)
    {
        _soundPlayed = false;
        _instanceFactory = instanceFactory;
        _audioStore = audioStore;
        _position = position;
        _startAngle = 0.0f;
        _startToEndAngle = sign * (float)Math.PI * 2.0f;
        _playedDuration = TimeSpan.Zero;
        _animationDuration = animationDuration;
        _particles = Enumerable.
            Range(0, particleCount).
            Select(_ => new Particle(_instanceFactory.CreateInstance(ParticleInstance.DefaultFactory(Color.Blue)), position)).
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
            _audioStore.Get(AudioHandle.Slash).Play();
            _soundPlayed = true;
        }

        foreach (var particle in _particles)
        {
            if (particle.LifeTime <= 0.0f)
            {
                Respawn(particle);
            }

            Update(particle, passedTime);
            _instanceFactory.Update(
                particle.InstanceHandle,
                _ => new ParticleInstance(Matrix4.CreateScale(.3f) * Matrix4.CreateTranslation(particle.Position), particle.Color)
            );
        }

    }

    private byte RandomColorChannel() => (byte)(255f * _random.NextSingle());

    private void Respawn(Particle particle)
    {
        particle.Color = new Color4(0, 0, RandomColorChannel(), 255);
        var x = _random.NextSingle();
        particle.LifeTime = x * x * x * 500.0f;
        var angle = _startAngle + NormalizedAnimationTime * _startToEndAngle;
        var v = Vector3.Multiply(new Vector3(0.9f + 0.05f*_random.NextSingle(), 0.1f*_random.NextSingle(), 1.0f + 0.1f*_random.NextSingle()), Radius) * Matrix3.CreateRotationY(-angle);
        particle.Position = _position + v;
    }

    private static void Update(Particle particle, TimeSpan dt)
    {
        particle.LifeTime -= (float)dt.TotalMilliseconds;
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }
}