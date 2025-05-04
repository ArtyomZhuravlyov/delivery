using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

public sealed record OrderAssignedDomainEvent(Guid OrderId, string Status) : DomainEvent;