using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Mechs_Vs_Minions_Graphics.Graphics.GameWindow;
internal class KeyBindings
{
    private readonly Dictionary<Keys, Action> _keyBindings;

    public KeyBindings()
    {
        _keyBindings = new Dictionary<Keys, Action>();
    }

    public void Add(Keys key, Action action)
    {
        if (_keyBindings.ContainsKey(key)) return;
        _keyBindings[key] = action;
    }

    public void Add(IEnumerable<(Keys, Action)> bindings)
    {
        foreach (var (key, action) in bindings) Add(key, action);
    }

    public void Delete(Keys key) => _keyBindings.Remove(key);

    public void Delete(IEnumerable<Keys> list)
    {
        foreach (var key in list) Delete(key);
    }

    public bool OnKeyDown(Keys key)
    {
        _keyBindings.TryGetValue(key, out var action);
        if (action == null) return false;
        action();
        return true;
    }
}
