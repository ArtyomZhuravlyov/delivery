using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Order : Aggregate<Guid>
{
    private Order()
    {
    }
    
    private Order(Guid basketId, Location location) : this()
    {
        Id = basketId;
        Location = location;
        Status = OrderStatus.Created;
    }
    
    public Location Location { get; }
    
    public OrderStatus Status  { get; private set; }

    public Guid? CourierId  { get; private set; }

    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="basketId">Идентификатор корзины</param>
    /// <param name="location"></param>
    /// <returns>Результат</returns>
    public static Result<Order, Error> Create(Guid basketId, Location location)
    {
        if (basketId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(basketId));
        if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
        return new Order(basketId, location);
    }
    
    public UnitResult<Error> AssignCourier(Guid courierId)
    {
        if (courierId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(courierId));
        if (Status == OrderStatus.Completed) return Errors.CantAssignOrderToCompleted();
        if (Status == OrderStatus.Assigned) return Errors.CantAssignOrderToBusyCourier(courierId);

            
        Status = OrderStatus.Assigned;
        CourierId = courierId;
        return UnitResult.Success<Error>();
    }
    
    public UnitResult<Error> Complete()
    {
        if(Status != OrderStatus.Assigned)return Errors.CantCompletedNotAssignedOrder();
            
        Status = OrderStatus.Completed;
        return UnitResult.Success<Error>();
    }
    
    /// <summary>
    ///     Ошибки, которые может возвращать сущность
    /// </summary>
    [ExcludeFromCodeCoverage]

    public static class Errors
    {
        public static Error CantCompletedNotAssignedOrder()
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.cant.completed.not.assigned.order",
                "Нельзя завершить заказ, который не был назначен");
        }

        public static Error CantAssignOrderToBusyCourier(Guid courierId)
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.cant.assign.order.to.busy.courier",
                $"Нельзя назначить заказ на курьера, заказ занят. Id курьера = {courierId}");
        }
        
        public static Error CantAssignOrderToCompleted()
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.cant.assign.order.to.completed",
                $"Нельзя назначить заказ на курьера, который выполнен.");
        }

    }

}