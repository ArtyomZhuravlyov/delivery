using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

public sealed record OrderCompletedDomainEvent(Guid OrderId, string Status) : DomainEvent;