namespace AccountService.Application.EventBus;

public interface IEventBusPublisher
{
    Task PublishAsync<T>(T @event, string topic, CancellationToken cancellationToken = default);
}