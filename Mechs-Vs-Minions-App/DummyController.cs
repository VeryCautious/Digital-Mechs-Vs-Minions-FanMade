using System.Collections.Immutable;
using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Abstractions.GameStates.Cards;

namespace Mechs_Vs_Minions_App;

internal class DummyController : IController
{
    
    private CancellationTokenSource _cancellationTokenSource;

    public DummyController(CancellationTokenSource cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;
    }

    public async Task<FieldSelectionResult> Prompt(SelectFieldFrom userPrompt)
    {
        Console.WriteLine("\nSelect from");
        Console.WriteLine(string.Join("\n", userPrompt.Fields.Select((field,index) => $"{index}: ({field.X},{field.Y})")));
        var str = await ReadLineAsync();

        return new FieldSelectionResult(userPrompt.Fields[int.Parse(str ?? "0")]);
    }

    public async Task<CardSelectionResult> Prompt(SelectCardFrom userPrompt)
    {
        Console.WriteLine("\nSelect PlayableCard from");
        Console.WriteLine(string.Join("\n", userPrompt.Cards.Select((card,index) => $"{index}: {card.GetType().Name} ({card.Color})")));
        var str = await ReadLineAsync();

        return new CardSelectionResult(userPrompt.Cards[int.Parse(str ?? "0")]);
    }

    public async Task<SelectCommandSlotResult> Prompt(SelectCommandSlotFrom userPrompt)
    {
        Console.WriteLine("Your current Commandline:");
        
        for (var i = 0; i < CommandLine.SlotCount; i++)
        {
            PrintSlot(userPrompt.CommandLine.SlotAt(i), i);
        }

        Console.Write("Select Slot from: ");
        Console.WriteLine(string.Join(", ", userPrompt.SlotIndices.Select(slot => $"{slot}")));

        int index;

        do
        {
            var str = await ReadLineAsync();
            index = int.Parse(str ?? "0");

            if (!userPrompt.SlotIndices.Contains(index))
            {
                Console.WriteLine("This slot can not be selected. Choose again...");
            }

        } while (!userPrompt.SlotIndices.Contains(index));

        return new SelectCommandSlotResult(index);
    }

    private static void PrintSlot(IImmutableStack<Card>? slot, int index)
    {
        if (slot == null)
        {
            Console.WriteLine($"{index}: Empty");
            return;
        }

        Console.WriteLine($"{index}: {string.Join(",", slot.Reverse().ToArray().Select(card => card.GetType().Name))}");
    }

    public async Task<SelectRotationResult> Prompt(SelectRotationFrom userPrompt)
    {
        Console.WriteLine("\nSelect from");
        Console.WriteLine(string.Join("\n", userPrompt.Rotations.Select((rot,index) => $"{index}: {rot}")));
        var str = await ReadLineAsync();

        return new SelectRotationResult(userPrompt.Rotations[int.Parse(str ?? "0")]);
    }

    public async Task<ConfirmResult> Prompt(Confirm userPrompt)
    {
        Console.WriteLine($"Confirm Action '{userPrompt.Description}': (enter)");
        await ReadLineAsync();
        return new ConfirmResult();
    }

    public async Task<MetaGameRequest> DequeueMetaGameInteraction(CancellationToken cancellation)
    {
        await Task.Delay(TimeSpan.MaxValue, cancellation);

        return null!;
    }

    private async Task<string?> ReadLineAsync()
    {
        var ct = _cancellationTokenSource.Token;
        var readLineTask = Task.Run(Console.ReadLine, ct);
        
        Task.WaitAll(new Task[]{readLineTask}, ct);

        return await readLineTask;
    }

    public void SetCancellationTokenSource(CancellationTokenSource tokenSource)
    {
        _cancellationTokenSource = tokenSource;
    }
}
