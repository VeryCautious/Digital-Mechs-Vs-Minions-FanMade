using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.UserInteractions;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

internal abstract class GamePhaseOverlayManager
{
    protected readonly TreeRenderableInstanceState OverlaySceneGraph;
    protected readonly IGraphicsModelProvider GraphicsModelProvider;
    protected readonly ITexturesStore TexturesStore;
    protected readonly IUserInteractionLookup UserInteractionLookup;
    
    private float _screenHwRatio;
    public bool IsVisible { get; private set; } = true;

    public void SetVisibility(bool visibility) => IsVisible = visibility;

    protected GamePhaseOverlayManager(
        float screenHwRatio,
        IGraphicsModelProvider graphicsModelProvider,
        ITexturesStore texturesStore,
        IUserInteractionLookup userInteractionLookup
    ) {
        _screenHwRatio = screenHwRatio;
        OverlaySceneGraph = new TreeRenderableInstanceState(_screenHwRatio, graphicsModelProvider);
        GraphicsModelProvider = graphicsModelProvider;
        TexturesStore = texturesStore;
        UserInteractionLookup = userInteractionLookup;
    }

    public abstract void Init(GameState? gameState);
    public abstract void Destroy();
    protected abstract void Update();

    public void OnResize(float screenHwRatio)
    {
        _screenHwRatio = screenHwRatio;
        OnParentResize();
    }

    protected void OnParentResize()
    {
        OverlaySceneGraph.SetRootHwRatio(_screenHwRatio);
        Update();
    }

    protected void ChildDraw()
        => OverlaySceneGraph.Draw();

    public void Draw()
    {
        if (!IsVisible)
        {
            return;
        }
        
        ChildDraw();
    }
    
    public void ToggleVisibility(){
        IsVisible = !IsVisible;
        Update();
    }

    public UserInteraction? GetUserInteractionFromClick(Vector2 normalizedScreenClickPosition)
    {
        var elementHandle = OverlaySceneGraph.GetElementAt(normalizedScreenClickPosition);
        return elementHandle is null ? null : UserInteractionLookup.GetInteractionFrom(elementHandle);
    }

    public virtual void Notify(UserInteraction interaction) { }

}