using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;

/// <summary>
/// Implementation from: https://adrianb.io/2014/08/09/perlinnoise.html
/// </summary>
internal class Noise3D
{
    private readonly int[] _p;

    private readonly float _scale;

    public Noise3D(float scale) : this(scale, new Random().Next()) { }

    private Noise3D(float scale, int seed)
    {
        _scale = scale;
        _p = GeneratePermutationArray(seed);
    }

    public float Generate(Vector3 v) => Generate(v.X, v.Y, v.Z);


    private float Generate(float x, float y, float z)
    {
        var coo = (new Vector3(x, y, z) + new Vector3(1.0f, 1.0f, 1.0f)) * _scale;

        var flx = MathF.Floor(coo.X);
        var fly = MathF.Floor(coo.Y);
        var flz = MathF.Floor(coo.Z);

        var gridX = (int)flx & 255;
        var gridY = (int)fly & 255;
        var gridZ = (int)flz & 255;

        var ix = coo.X - flx;
        var iy = coo.Y - fly;
        var iz = coo.Z - flz;

        var u = Fade(ix);
        var v = Fade(iy);
        var w = Fade(iz);

        var aaa = _p[_p[_p[gridX] + gridY] + gridZ];
        var aba = _p[_p[_p[gridX] + gridY + 1] + gridZ];
        var aab = _p[_p[_p[gridX] + gridY] + gridZ + 1];
        var abb = _p[_p[_p[gridX] + gridY + 1] + gridZ + 1];
        var baa = _p[_p[_p[gridX + 1] + gridY] + gridZ];
        var bba = _p[_p[_p[gridX + 1] + gridY + 1] + gridZ];
        var bab = _p[_p[_p[gridX + 1] + gridY] + gridZ + 1];
        var bbb = _p[_p[_p[gridX + 1] + gridY + 1] + gridZ + 1];

        var x1 = Mix(
            Grad(aaa, ix, iy, iz),
            Grad(baa, ix - 1, iy, iz),
            u);
        var x2 = Mix(
            Grad(aba, ix, iy - 1, iz),
            Grad(bba, ix - 1, iy - 1, iz),
            u);
        var y1 = Mix(x1, x2, v);

        x1 = Mix(
            Grad(aab, ix, iy, iz - 1),
            Grad(bab, ix - 1, iy, iz - 1),
            u);
        x2 = Mix(
            Grad(abb, ix, iy - 1, iz - 1),
            Grad(bbb, ix - 1, iy - 1, iz - 1),
            u);
        var y2 = Mix(x1, x2, v);

        return (Mix(y1, y2, w) + 1.0f) / 2.0f;
    }


    private static float Grad(int h, float x, float y, float z)
    {
        return (h & 15) switch
        {
            0 => x + y,
            1 => -x + y,
            2 => x - y,
            3 => -x - y,
            4 => x + z,
            5 => -x + z,
            6 => x - z,
            7 => -x - z,
            8 => y + z,
            9 => -y + z,
            10 => y - z,
            11 => -y - z,
            12 => y + x,
            13 => -y + z,
            14 => y - x,
            15 => -y - z,
            _ => 0
        };
    }

    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Mix(float a, float b, float factor) => a + factor * (b - a);

    private int[] GeneratePermutationArray(int seed)
    {
        var rnd = new Random(seed);

        var arr = Enumerable.Range(0,256).OrderBy(_ => rnd.Next()).ToList();

        var perm = arr;
        arr.AddRange(arr);
        arr.Add(arr[0]);

        return perm.ToArray();
    }
}
