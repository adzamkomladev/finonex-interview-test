namespace WebhookAPI.Infrastructure;

public interface IWrite<T>
{
    void Push(T webhookInfo);
    void Complete();
}