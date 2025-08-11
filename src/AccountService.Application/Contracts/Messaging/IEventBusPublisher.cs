namespace AccountService.Application.Contracts.Messaging;

public interface IEventBusPublisher
{
    Task PublishAsync<T>(T @event, string topic, CancellationToken cancellationToken = default);
}