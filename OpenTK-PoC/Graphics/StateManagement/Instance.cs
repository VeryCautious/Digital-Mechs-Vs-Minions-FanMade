using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

internal record Instance;

internal record PositionalInstance(Matrix4 ModelTransform) : Instance;

internal record ClickableInstance(Matrix4 ModelTransform, Color4 Color) : PositionalInstance(ModelTransform);

internal record BoardTileInstance(Matrix4 ModelTransform, bool IsOutlined) : PositionalInstance(ModelTransform);

internal sealed record MechInstance(Matrix4 ModelTransform, bool IsOutlined) : PositionalInstance(ModelTransform)
{
    public static Func<MechInstance> DefaultFactory => () => new MechInstance(Matrix4.Identity, false);
}

internal sealed record MinionInstance(Matrix4 ModelTransform) : PositionalInstance(ModelTransform)
{
    public static Func<MinionInstance> DefaultFactory => () => new MinionInstance(Matrix4.Identity);
}

internal sealed record ParticleInstance(Matrix4 ModelTransform, Color4 Color) : PositionalInstance(ModelTransform)
{
    public static Func<ParticleInstance> DefaultFactory(Color4 color) => () => new ParticleInstance(Matrix4.Identity, color);
}

internal sealed record OverlayInstance(Matrix4 ModelTransform, Type ModelType) : PositionalInstance(ModelTransform);

internal sealed record RipSawInstance(Matrix4 ModelTransform) : PositionalInstance(ModelTransform)
{
    public static Func<RipSawInstance> DefaultFactory => () => new RipSawInstance(Matrix4.Identity);
}