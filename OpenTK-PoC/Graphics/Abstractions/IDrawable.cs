using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IDrawable : IRenderModelComponent
{
    void Draw();
}