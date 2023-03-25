using Mechs_Vs_Minions_Abstractions.GameStates.Cards;
using System.Collections.Immutable;

namespace Mechs_Vs_Minions_Abstractions.GameStates;

public sealed record CommandLine(IImmutableDictionary<int,IImmutableStack<Card>> Cards) {
    public const int SlotCount = 6;

    public CommandLine PlaceCard(int index, ISlotableCard card) {
        if (index is < 0 or >= SlotCount) throw new ArgumentOutOfRangeException(nameof(index), "Slot index is out of range.");
        return Cards.ContainsKey(index) ? 
            new CommandLine(Cards.SetItem(index, Cards[index].Push((Card)card))) : 
            new CommandLine(Cards.SetItem(index, ImmutableStack<Card>.Empty.Push((Card)card)));
    }

    public Card? TopCardAt(int index) => SlotAt(index)?.Peek();

    public IImmutableStack<Card>? SlotAt(int index) => Cards.GetValueOrDefault(index);

    public static CommandLine Empty => new(ImmutableDictionary.Create<int, IImmutableStack<Card>>());

    public CommandLine SwapSlots(int index1, int index2)
    {
        var hasFirstSlot = Cards.TryGetValue(index1, out var firstSlot);
        var hasSecondSlot = Cards.TryGetValue(index2, out var secondSlot);
        var newCards = Cards;

        newCards = hasFirstSlot ? newCards.SetItem(index2, firstSlot!) : newCards.Remove(index2);
        newCards = hasSecondSlot ? newCards.SetItem(index1, secondSlot!) : newCards.Remove(index1);

        return new CommandLine(newCards);
    }

};