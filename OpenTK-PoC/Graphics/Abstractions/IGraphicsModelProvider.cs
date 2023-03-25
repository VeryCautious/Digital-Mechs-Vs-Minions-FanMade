using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

namespace Mechs_Vs_Minions_Graphics.Graphics.Abstractions;

internal interface IGraphicsModelProvider
{
    T GetRenderModel<T, TInstance>() where T : RenderModel<TInstance> where TInstance : Instance;
    bool HasRenderModelFor(Type t);

    public List<T> GetProceduralRenderModels<T, TInstance>() where T : RenderModel<TInstance> where TInstance : Instance;

    public bool HasProceduralRenderModelFor(Type t);
}