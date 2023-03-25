using Mechs_Vs_Minions_Graphics.UserInteractions;
using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics.Graphics.GameWindow;

internal class Clickables
{
    private readonly IDictionary<Guid, int> _idToNumber;
    private readonly IDictionary<int, Guid> _numberToId;
    private readonly IDictionary<Guid, UserInteraction> _userInteractionDict;

    private int _counter;

    public Clickables()
    {
        _counter = 0;
        _idToNumber = new Dictionary<Guid, int>();
        _numberToId = new Dictionary<int, Guid>();
        _userInteractionDict = new Dictionary<Guid, UserInteraction>();
    }

    public void Add(Guid id, UserInteraction userInteraction)
    {
        if (_idToNumber.ContainsKey(id)) return;

        var number = _counter++;
        _idToNumber.Add(id, number);
        _numberToId.Add(number, id);

        _userInteractionDict.Add(id, userInteraction);
    }

    public bool TryGetColorOf(Guid id, out Color4 color)
    {
        if (!_idToNumber.TryGetValue(id, out int number))
        {
            color = default(Color4);
            return false;
        }

        var intColor = IntToColor(number);
        color = new Color4(intColor.R / 255.0f, intColor.G / 255.0f, intColor.B / 255.0f, 1);
        return true;
    }

    public bool TryGetIdOf(Color4 color, out Guid id)
    {
        if (!_numberToId.TryGetValue(ColorToNumber(color), out Guid dictId))
        {
            id = default(Guid);
            return false;
        }

        id = dictId;
        return true;
    }

    public UserInteraction? GetUserInteractionFor(Color4 color)
    {
        if (!TryGetIdOf(color, out Guid id)) return null;
        return _userInteractionDict[id];
    }

    private static Color4 IntToColor(int n) => new Color4(
        n & 0xFF,
        (n >> 8) % 0xFF,
        (n >> 16) % 0xFF,
        0xFF
    );

    private static int ColorToNumber(Color4 color)
    {
        int number = (int) color.B;
        number <<= 8;
        number += (int) color.G;
        number <<= 8;
        number += (int) color.R;
        return number;
    }
}
