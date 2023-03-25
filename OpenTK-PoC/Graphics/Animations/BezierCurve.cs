using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Graphics.Graphics.Animations;

internal abstract class BezierCurve<TKnot>
{
    private IList<TKnot> _knots;

    public BezierCurve(IList<TKnot> knots)
    {
        _knots = knots;
    }

    public IImmutableList<TKnot> Knots => _knots.ToImmutableList();

    protected abstract TKnot Add(TKnot a, TKnot b);

    protected abstract TKnot Scale(TKnot a, float t);

    protected TKnot LinearInterpolation(TKnot a, TKnot b, float t) => Add(Scale(a, 1-t), Scale(b, t));

    public IList<TKnot> Step(IList<TKnot> knots, float t) => 
        Enumerable.Range(0, knots.Count - 1)
            .Select(i => LinearInterpolation(knots[i], knots[i + 1], t))
            .ToList();

    public TKnot Eval(float t)
    {
        var knots = _knots;
        while (knots.Count > 1) knots = Step(knots, t);
        return knots[0];
    }

    public IList<TKnot> Subdivide(int resolution = 0)
    {
        if (resolution == 0) return _knots;

        var knots = _knots;
        for (int i = 0; i < resolution; i++) knots = Subdivide(knots);

        return knots;
    }

    private List<TKnot> Subdivide(IList<TKnot> knots)
    {
        var calcPyramide = new List<IList<TKnot>> { knots };
        while (calcPyramide.Last().Count > 1) calcPyramide.Add(Step(calcPyramide.Last(), 0.5f));

        var nextKnots = new List<TKnot>();
        for (int i = 0; i < calcPyramide.Count; i++) nextKnots.Add(calcPyramide[i].First());
        for (int i = calcPyramide.Count - 2; i >= 0; i--) nextKnots.Add(calcPyramide[i].Last());
        return nextKnots;
    }
}

internal class Vector3BezierCurve : BezierCurve<Vector3>
{
    public Vector3BezierCurve(IList<Vector3> knots) : base(knots)
    {
    }

    protected override Vector3 Add(Vector3 a, Vector3 b) => a + b;

    protected override Vector3 Scale(Vector3 a, float t) => t * a;

    public static Vector3BezierCurve LinearCurve(Vector3 start, Vector3 end) => new Vector3BezierCurve(new List<Vector3> { start, end });

    public static Vector3BezierCurve ConstantCurve(Vector3 constant) => new Vector3BezierCurve(new List<Vector3> { constant });
}

internal class FloatBezierCurve : BezierCurve<float>
{
    public FloatBezierCurve(IList<float> knots) : base(knots)
    {
    }

    protected override float Add(float a, float b) => a + b;

    protected override float Scale(float a, float t) => t * a;

    public static FloatBezierCurve LinearCurve(float start, float end) => new FloatBezierCurve(new List<float> { start, end });

    public static FloatBezierCurve ConstantCurve(float constant) => new FloatBezierCurve(new List<float> { constant });
}
