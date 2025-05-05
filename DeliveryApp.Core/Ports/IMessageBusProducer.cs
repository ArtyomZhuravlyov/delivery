using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports;

public interface IMessageBusProducer
{
    Task PublishOrderAssignedDomainEvent(OrderAssignedDomainEvent notification, CancellationToken cancellationToken);

    Task PublishOrderCompletedDomainEvent(OrderCompletedDomainEvent notification, CancellationToken cancellationToken);
}