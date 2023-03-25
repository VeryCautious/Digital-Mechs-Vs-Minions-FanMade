using System.Collections.Immutable;
using Mechs_Vs_Minions_Abstractions;

namespace Mechs_Vs_Minions_Graphics.Utilities;

public class Translator<TKey, TValue> where TKey : notnull
{
    private readonly IDictionary<TKey, TValue> _translationDict;

    public Translator()
    {
        _translationDict = new Dictionary<TKey, TValue>();
    }

    public IImmutableList<TKey> Keys => _translationDict.Keys.ToImmutableList();

    public TValue Get(TKey key) => _translationDict[key];
    public TValue Get(IIdentifiable<TKey> identifiable)
        => Get(identifiable.Id);

    public bool Contains(TKey key) => _translationDict.ContainsKey(key);
    public bool Contains(IIdentifiable<TKey> identifiable)
        => Contains(identifiable.Id);

    public TValue GetOrCreate(TKey key, Func<TValue> factory)
    {
        if (!_translationDict.ContainsKey(key))
        {
            var newValue = factory();
            _translationDict.Add(key, newValue);
        }

        return Get(key);
    }
    public TValue GetOrCreate(IIdentifiable<TKey> identifiable, Func<TValue> factory)
        => GetOrCreate(identifiable.Id, factory);

    public void Remove(TKey key) => _translationDict.Remove(key);
    public void Remove(IIdentifiable<TKey> identifiable)
        => Remove(identifiable.Id);
}