using System.Collections.Concurrent;

namespace Mechs_Vs_Minions_Graphics.UserInteractions;

public class AsyncQueue<T>
{
    private readonly SemaphoreSlim _enumerationSemaphore = new(1);
    private readonly ConcurrentQueue<T> _bufferBlock = new();

    public void Clear() => _bufferBlock.Clear();

    public Task Enqueue(T item, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        _bufferBlock.Enqueue(item);
        return Task.CompletedTask;
    }


    public async Task<T> Dequeue(CancellationToken token = default)
    {
        await _enumerationSemaphore.WaitAsync(token);
        try {
            T result;
            while (!_bufferBlock.TryDequeue(out result!))
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(5, token);
            }
            return result;
        } finally {
            _enumerationSemaphore.Release();
        }

    }
}