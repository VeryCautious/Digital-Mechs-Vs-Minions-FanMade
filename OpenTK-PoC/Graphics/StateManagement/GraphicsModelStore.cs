using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

internal class GraphicsModelStore : IGraphicsModelProvider
{
    private readonly IDictionary<Type, object> _models;
    private readonly IDictionary<Type, List<object>> _proceduralModels;

    public GraphicsModelStore()
    {
        _models = new Dictionary<Type, object>();
        _proceduralModels = new Dictionary<Type, List<object>>();
    }

    public void AddRenderModel<T, TInstance>(T model) where T : RenderModel<TInstance> where TInstance : Instance
    {
        model.Load();
        _models[typeof(T)] = model;
    }

    public T GetRenderModel<T, TInstance>() where T : RenderModel<TInstance> where TInstance : Instance
        => (T)_models[typeof(T)];

    public bool HasRenderModelFor(Type t)
        => _models.ContainsKey(t);

    public void AddProceduralRenderModel<T, TInstance>(T model) where T : RenderModel<TInstance> where TInstance : Instance
    {
        model.Load();
        if (!_proceduralModels.ContainsKey(typeof(T))) _proceduralModels[typeof(T)] = new List<object>();
        _proceduralModels[typeof(T)].Add(model);
    }

    public List<T> GetProceduralRenderModels<T, TInstance>() where T : RenderModel<TInstance> where TInstance : Instance
        => _proceduralModels[typeof(T)].OfType<T>().ToList();

    public bool HasProceduralRenderModelFor(Type t)
        => _proceduralModels.ContainsKey(t);

    public void Destroy<T>()
    {
        _proceduralModels.Remove(typeof(T));
        _models.Remove(typeof(T));
    }

}