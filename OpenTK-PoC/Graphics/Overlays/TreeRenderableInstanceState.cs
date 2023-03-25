using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.RenderingComponents;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;
internal class TreeRenderableInstanceState
{
    private readonly IGraphicsModelProvider _modelProvider;
    private readonly InstanceHandle _rootHandle;
    private readonly Dictionary<InstanceHandle, OverlaySceneGraphEntry> _treeDict;
    private readonly InstanceStore<OverlayInstance> _instanceState;

    private readonly Dictionary<Type, Action<ImmutableList<OverlayInstance>>> _drawFuncDict;

    public TreeRenderableInstanceState(float rootHwRatio, IGraphicsModelProvider modelProvider)
    {
        _modelProvider = modelProvider;
        _instanceState = new InstanceStore<OverlayInstance>();
        var rootNode = CreateRoot(rootHwRatio);
        _rootHandle = rootNode.InstanceHandle;
        _treeDict = new Dictionary<InstanceHandle, OverlaySceneGraphEntry>
        {
            [rootNode.InstanceHandle] = rootNode
        };

        _drawFuncDict = DrawFuncDict();
    }

    public void UpdateRectData(InstanceHandle handle, Func<OverlaySceneGraphRectangle, OverlaySceneGraphRectangle> rectDataMutator)
    {
        _treeDict[handle].UpdateRectData(rectDataMutator(_treeDict[handle].RectData));
        UpdateInstance(handle);
    }

    public InstanceHandle? GetElementAt(Vector2 normalizedScreenClickPosition)
    {
        var foundInstance = _treeDict[_rootHandle].GetElementAt(normalizedScreenClickPosition.X, normalizedScreenClickPosition.Y)?.InstanceHandle;
        return foundInstance != _rootHandle ? foundInstance : null;
    }

    public InstanceHandle Add(
        OverlaySceneGraphRectangle rectData,
        InstanceHandle parentHandle, 
        Type t
    )
    {
        var instanceHandle = _instanceState.CreateInstance(()=>new OverlayInstance(Matrix4.Identity, t));

        if (!_treeDict.ContainsKey(parentHandle)) throw new ArgumentOutOfRangeException(nameof(parentHandle), "The handle was not registered");

        _treeDict[instanceHandle] = new OverlaySceneGraphEntry(
            _treeDict[parentHandle],
            instanceHandle,
            rectData
        );

        _treeDict[parentHandle].AddChild(_treeDict[instanceHandle]);

        return instanceHandle;
    }

    public InstanceHandle Add<T>(
        OverlaySceneGraphRectangle rectData,
        InstanceHandle parentHandle
    ) => Add(rectData, parentHandle, typeof(T));

    public InstanceHandle AddToRoot(OverlaySceneGraphRectangle rectData, Type t) => Add(rectData, _rootHandle, t);

    public InstanceHandle AddToRoot<T>(OverlaySceneGraphRectangle rectData) => Add(rectData, _rootHandle, typeof(T));

    public void Delete(InstanceHandle handle)
    {
        if (handle == _rootHandle || !_treeDict.ContainsKey(handle)) return;

        var node = _treeDict[handle];
        foreach (var child in node.Children)
        {
            Delete(child.InstanceHandle);
        }
        _treeDict.Remove(handle);

        _instanceState.DestroyInstance((Handle<OverlayInstance>)handle);
    }

    public void SetRootHwRatio(float hwRatio)
    {
        _treeDict[_rootHandle].UpdateRectData(_treeDict[_rootHandle].RectData with { HwRatio = hwRatio });
    }

    public void MoveZIndex(InstanceHandle handle, float zIndex)
    {
        if (!_treeDict.ContainsKey(handle)) throw new ArgumentOutOfRangeException(nameof(handle), "The handle was not registered");

        _treeDict[handle].ZIndex += zIndex;
        UpdateInstance(handle);
    }

    private static OverlaySceneGraphEntry CreateRoot(float hwRatio)
    {
        var rect = OverlaySceneGraphRectangle.Default with { HwRatio = hwRatio };
        return new OverlaySceneGraphEntry(null, new InstanceHandle(Guid.Empty), rect);
    }

    public void UpdateInstance(InstanceHandle handle)
    {
        if (handle == _rootHandle) return;
        _instanceState.Update((Handle<OverlayInstance>)handle, inst => inst with { ModelTransform = _treeDict[handle].GetAccumulatedTransformMatrix()});
    }

    private Dictionary<Type, Action<ImmutableList<OverlayInstance>>> DrawFuncDict() => 
        new()
        {
            {typeof(RipsawCardModel), Draw<RipsawCardModel> },
            {typeof(BlazeCardModel), Draw<BlazeCardModel> },
            {typeof(ScytheCardModel), Draw<ScytheCardModel> },
            {typeof(OmnistompCardModel), Draw<OmnistompCardModel> },
            {typeof(SpeedCardModel), Draw<SpeedCardModel> },
            {typeof(AimBotCardModel), Draw<AimBotCardModel> },
            {typeof(CommandLineModel), Draw<CommandLineModel> },
            {typeof(TransparentOverlayModel), Draw<TransparentOverlayModel> },
            {typeof(LeftArrowModel), Draw<LeftArrowModel> },
            {typeof(TopArrowModel), Draw<TopArrowModel> },
            {typeof(RightArrowModel), Draw<RightArrowModel> },
            {typeof(DownArrowModel), Draw<DownArrowModel> },
            {typeof(VictoryOverlayModel), Draw<VictoryOverlayModel> },
            {typeof(LoadingScreenModel), Draw<LoadingScreenModel> },
            {typeof(DamageCardModel), Draw<DamageCardModel> },
            {typeof(ResetGameMenuModel), Draw<ResetGameMenuModel> }
        };

    public void Draw()
    {
        foreach (var group in _instanceState.GetInstances().GroupBy(instance => instance.ModelType))
        {
            if (!_drawFuncDict.ContainsKey(group.Key)) throw new ArgumentOutOfRangeException(nameof(group.Key));
            _drawFuncDict[group.Key](group.ToImmutableList());
        }
    }

    private void Draw<TModel>(ImmutableList<OverlayInstance> instances) where TModel : RenderModel<OverlayInstance>
    {
        var model = _modelProvider.GetRenderModel<TModel, OverlayInstance>();
        using (model.Bind())
        {
            foreach (var instance in instances)
            {
                model.RenderInstanceWith(instance, Enumerable.Empty<Uniform>());
            }
        }
    }
}