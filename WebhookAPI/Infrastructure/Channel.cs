using System.Collections.Concurrent;

namespace WebhookAPI.Infrastructure;

public class Channel<T> : IRead<T>, IWrite<T>
{
    private readonly SemaphoreSlim _flag;

    private readonly ConcurrentQueue<T> _queue;
    private bool _finished;

    public Channel()
    {
        _queue = new ConcurrentQueue<T>();
        _flag = new SemaphoreSlim(0);
    }

    public async Task<T?> Read()
    {
        await _flag.WaitAsync();

        return _queue.TryDequeue(out var webhookInfo) ? webhookInfo : default;
    }

    public async Task<IEnumerable<T>> ReadAll()
    {
        await _flag.WaitAsync();

        return _queue.AsEnumerable();
    }

    public bool IsComplete()
    {
        return _finished && _queue.IsEmpty;
    }

    public void Push(T webhookInfo)
    {
        _queue.Enqueue(webhookInfo);
        _flag.Release();
    }

    public void Complete()
    {
        _finished = true;
    }
}