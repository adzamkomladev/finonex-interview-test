namespace WebhookAPI.Infrastructure;

public interface IRead<T>
{
    Task<T?> Read();
    Task<IEnumerable<T>> ReadAll();
    bool IsComplete();
}