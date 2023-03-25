using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal class SmokeAnimation : IAnimation
{
    private readonly IInstanceFactory<ParticleInstance> _instanceFactory;
    private readonly IAudioStore _audioStore;
    private bool _soundPlayed;
    private readonly Particle[] _particles;
    private readonly Vector3 _position;
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
        public readonly bool IsSpark;

        public Particle(Handle<ParticleInstance> instanceHandle, Vector3 position, bool isSpark)
        {
            InstanceHandle = instanceHandle;
            Position = position;
            LifeTime = 0;
            IsSpark = isSpark;
        }
    }

    public SmokeAnimation(TimeSpan animationDuration, IInstanceFactory<ParticleInstance> instanceFactory, IAudioStore audioStore, Vector3 position, TimeSpan delay, int particleCount = 500)
    {
        _soundPlayed = false;
        _instanceFactory = instanceFactory;
        _audioStore = audioStore;
        _position = position;
        _playedDuration = TimeSpan.Zero.Subtract(delay);
        _animationDuration = animationDuration;
        _particles = Enumerable.
            Range(0, particleCount).
            Select(_ => new Particle(_instanceFactory.CreateInstance(ParticleInstance.DefaultFactory(Color4.Red)), position, _random.NextSingle() < .2)).
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
            if (particle.LifeTime <= float.Epsilon && _random.NextSingle() < .1f)
            {
                Respawn(particle);
            }

            Update(particle, passedTime);
            _instanceFactory.Update(
                particle.InstanceHandle,
                _ => new ParticleInstance(SizeTransform(particle) * Matrix4.CreateTranslation(particle.Position), particle.Color)
            );
        }

    }

    private static Matrix4 SizeTransform(Particle particle) => particle.IsSpark ? Matrix4.CreateScale(.1f) : Matrix4.CreateScale(.6f);
    
    private byte RandomColorChannel() => (byte)(255f * _random.NextSingle());

    private const float MaxSmokeSpeed = .2f;
    private const float MaxSparkSpeed = 1f;

    private Vector3 GetRandomVelocity(float speed)
    {
        var direction = new Vector3(_random.NextSingle() - 0.5f, -1f, _random.NextSingle() - 0.5f);
        direction.NormalizeFast();
        return Vector3.Multiply(direction, speed);
    }

    private Color4 GetRandomGreyScale()
    {
        var greyScale = RandomColorChannel();
        return new Color4(greyScale, greyScale, greyScale, 255);
    }

    private Color4 GetRandomSparkColor() => new(RandomColorChannel(), 0, 0, 255);


    private void Respawn(Particle particle)
    {
        if (particle.IsSpark)
        {
            particle.LifeTime = 200f + _random.NextSingle() * 200.0f;
            particle.Color = GetRandomSparkColor();
            particle.Velocity = GetRandomVelocity(MaxSparkSpeed);
        }
        else
        {
            particle.LifeTime = 2000f + _random.NextSingle() * 2000.0f;
            particle.Color = GetRandomGreyScale();
            particle.Velocity = GetRandomVelocity(MaxSmokeSpeed);
        }

        particle.Position = _position;
    }

    private void Update(Particle particle, TimeSpan dt)
    {
        if (!particle.IsSpark && _random.NextSingle() < .01f)
        {
            particle.Velocity = GetRandomVelocity(MaxSmokeSpeed);
        }

        particle.LifeTime -= (float)dt.TotalMilliseconds;
        particle.Position += Vector3.Multiply(particle.Velocity, (float)dt.TotalSeconds);
    }

    private void UpdatePassedTime(TimeSpan passedTime)
    {
        _playedDuration = _playedDuration.Add(passedTime);
    }
}