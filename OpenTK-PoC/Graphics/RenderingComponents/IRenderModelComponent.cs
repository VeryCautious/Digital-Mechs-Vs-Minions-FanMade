using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

namespace Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

internal interface IRenderModelComponent : IBindable, IDisposable
{
    void Load();
}
