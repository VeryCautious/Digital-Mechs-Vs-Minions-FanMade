using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.StateManagement;

internal class GraphicsState<TModel, TInstance, TUniforms>
    where TModel : RenderModel<TInstance>
    where TInstance : Instance
    where TUniforms : IUniformProvider
{
    private readonly TUniforms _uniformProvider;

    private readonly Dictionary<(Type, int), InstanceStore<TInstance>> _instanceStoreDict;
    private readonly Dictionary<(Type, int), List<RenderModel<TInstance>>> _modelDict;

    public GraphicsState(TUniforms uniformProvider)
    {
        _uniformProvider = uniformProvider;
        _instanceStoreDict = new Dictionary<(Type, int), InstanceStore<TInstance>>();
        _modelDict = new Dictionary<(Type, int), List<RenderModel<TInstance>>>();
    }

    public void AddModel<T>(T model) where T : TModel => AddModel(model, 0);

    public void AddModel<T>(T model, int version) where T : TModel
    {
        _instanceStoreDict.Add((typeof(T), version), new InstanceStore<TInstance>());

        if (!_modelDict.ContainsKey((typeof(T), version))) _modelDict[(typeof(T), version)] = new List<RenderModel<TInstance>>();
        _modelDict[(typeof(T), version)].Add(model);
    }

    public InstanceStore<TInstance> InstanceStore(Type t) => InstanceStore(t, 0);

    public InstanceStore<TInstance> InstanceStore(Type t, int version) => _instanceStoreDict[(t, version)];

    public InstanceStore<TInstance> InstanceStore<T>() where T : TModel => InstanceStore<T>(0);

    public InstanceStore<TInstance> InstanceStore<T>(int version) where T : TModel => _instanceStoreDict[(typeof(T), version)];

    public InstanceStore<TInstance> InstanceStoreOf(Handle<TInstance> handle)
    {
        return _instanceStoreDict.First(dict => dict.Value.Contains(handle)).Value;
    }

    public void DestroyInstance(Handle<TInstance> handle)
        => InstanceStoreOf(handle).DestroyInstance(handle);

    public void DestroyAllVersionsOf<T>()
    {
        foreach (var key in _instanceStoreDict.Keys.Where(key => key.Item1 == typeof(T)))
        {
            _instanceStoreDict.Remove(key);
            _modelDict.Remove(key);
        }
    }

    public void Destroy(ImmutableList<Handle<TInstance>> handles)
    {
        foreach (var handle in handles)
        {
            DestroyInstance(handle);
        }
    }

    public void DrawAllInstances()
    {
        var keys = _instanceStoreDict.Keys.Intersect(_modelDict.Keys);
        foreach (var key in keys)
        {
            var model = _modelDict[key][0];
            SetDepthAndBlendFunction(model);
            using (model.Bind())
            {
                foreach (var instance in _instanceStoreDict[key].GetInstances())
                {
                    model.RenderInstanceWith(instance, _uniformProvider.GetUniforms());
                }
            }
            ResetDepthAndBlendFunction();
        }

    }

    private static void ResetDepthAndBlendFunction()
    {
        GL.Disable(EnableCap.Blend);
        GL.DepthMask(true);
    }

    private void SetDepthAndBlendFunction(RenderModel<TInstance> model)
    {
        GL.DepthMask(!model.HasTransparency);
        if (!model.HasTransparency) return;

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public void ClearInstances()
    {
        foreach (var instanceStore in _instanceStoreDict.Values)
        {
            instanceStore.Clear();
        }
    }
}