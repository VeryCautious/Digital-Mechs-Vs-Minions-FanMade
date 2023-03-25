using System.Collections.Immutable;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.Overlays;

/// <summary>
/// 
/// </summary>
/// <param name="MaxWidthPercentage">completely inside parent for [0;1]</param>
/// <param name="MaxHeightPercentage">completely inside parent for [0;1]</param>
/// <param name="XTranslate">completely inside parent between -1 (at left border) and 1 (at right border)</param>
/// <param name="YTranslate">completely inside parent between -1 (at bottom border) and 1 (at top border)</param>
/// <param name="HwRatio">Height / Width</param>
internal sealed record OverlaySceneGraphRectangle(float MaxWidthPercentage, float MaxHeightPercentage, float XTranslate, float YTranslate, float HwRatio)
{
    public static OverlaySceneGraphRectangle Default => new(1.0f, 1.0f, 0.0f, 0.0f, 1.0f);
}

internal class OverlaySceneGraphEntry
{
    private const float ZDecrease = 0.01f;

    public ImmutableList<OverlaySceneGraphEntry> Children { get; private set; }
    private readonly OverlaySceneGraphEntry? _parent;
    public readonly InstanceHandle InstanceHandle;

    public float ZIndex { get; set; }

    public OverlaySceneGraphRectangle RectData { get; private set; }
    private Matrix4 TransformMatrix { get; set; }

    private Matrix4 InverseTransformMatrix { get; set; }

    public OverlaySceneGraphEntry(
        OverlaySceneGraphEntry? parent,
        InstanceHandle instanceHandle,
        OverlaySceneGraphRectangle rectData)
    {
        _parent = parent;
        InstanceHandle = instanceHandle;
        RectData = rectData;
        Children = ImmutableList<OverlaySceneGraphEntry>.Empty;

        UpdateTransformMatrix();
        ZIndex = IsRoot ? 1.0f : _parent!.ZIndex - ZDecrease;
    }

    public void UpdateRectData(OverlaySceneGraphRectangle newRectData)
    {
        var hwRatioWasUpdated = Math.Abs(RectData.HwRatio - newRectData.HwRatio) > 1e-5;
        RectData = newRectData;
        UpdateTransformMatrix();
        
        if (!hwRatioWasUpdated) return;
        
        foreach (var child in Children)
        {
            child.UpdateTransformMatrix();
        }

    }

    private void UpdateTransformMatrix()
    {
        if (IsRoot)
        {
            TransformMatrix = Matrix4.Identity;
            InverseTransformMatrix = Matrix4.CreateScale(1.0f, RectData.HwRatio, 1.0f);
        }
        else
        {
            var parentRatio = _parent!.RectData.HwRatio;
            var width = 2 * Math.Min(parentRatio * RectData.MaxHeightPercentage / RectData.HwRatio, RectData.MaxWidthPercentage);
            var height = RectData.HwRatio * width;
            var scale = Matrix4.CreateScale(width / 2.0f, width / 2.0f, 1.0f);
            var xt = (1 - (RectData.XTranslate + 1) / 2.0f) * (-1 + width / 2.0f) + (RectData.XTranslate + 1) / 2.0f * (1 - width / 2.0f);
            var yt = (1 - (RectData.YTranslate + 1) / 2.0f) * (-parentRatio + height / 2.0f) + (RectData.YTranslate + 1) / 2.0f * (parentRatio - height / 2.0f);
            var translate = Matrix4.CreateTranslation(xt, yt, 0.0f);
            TransformMatrix = scale * translate;

            var mat = TransformMatrix;
            mat.Transpose();
            mat.Invert();
            InverseTransformMatrix = mat;
        }
    }

    public Matrix4 GetAccumulatedTransformMatrix()
    {
        var node = this;
        var result = node.TransformMatrix;
        while (!node.IsRoot)
        {
            result *= node._parent!.TransformMatrix;
            node = node._parent;
        }
        return result * Matrix4.CreateTranslation(0.0f, 0.0f, ZIndex);
    }

    public bool IsRoot => _parent == null;

    public void AddChild(OverlaySceneGraphEntry childEntry)
    {
        Children = Children.Add(childEntry);
    }


    public OverlaySceneGraphEntry? GetElementAt(float x, float y)
    {
        var v = InverseTransformMatrix * new Vector4(x, y, 1.0f, 1.0f);

        if ((v.X is < -1 or > 1) || (v.Y < -RectData.HwRatio || v.Y > RectData.HwRatio)) return null;

        var potentialElements = Children.
            Select(child => child.GetElementAt(v.X, v.Y)).
            Where(elementHandle => elementHandle != null).ToImmutableList();

        return potentialElements.MinBy(inst => inst?.ZIndex) ?? this;
    }
}