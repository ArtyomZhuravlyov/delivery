using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public class DispatchService : IDispatchService
{
   public  Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
   {
      if (order == null) return GeneralErrors.ValueIsRequired(nameof(order));
      if (order.Status != OrderStatus.Created) return GeneralErrors.ValueIsRequired(nameof(order));
      if (couriers == null || couriers.Count == 0) return GeneralErrors.InvalidLength(nameof(couriers));
      
      var freeCouriers = couriers.Where(x => x.Status == CourierStatus.Free);
      var minTimeToLocation = double.MaxValue;
      Courier fastestCourier = null;
      foreach (var courier in freeCouriers)
      {
         var courierCalculateTimeToLocationResult = courier.GetSteps(order.Location);
         if (courierCalculateTimeToLocationResult.IsFailure) return courierCalculateTimeToLocationResult.Error;
         var timeToLocation = courierCalculateTimeToLocationResult.Value;

         if (timeToLocation < minTimeToLocation)
         {
            minTimeToLocation = timeToLocation;
            fastestCourier = courier;
         }
      }
      
      // Если подходящий курьер не был найден, возвращаем ошибку
      if (fastestCourier == null) return Errors.SuitableCourierWasNotFound();
      
      // Если курьер найден - назначаем заказ на курьера
      var orderAssignToCourierResult = order.AssignCourier(fastestCourier.Id);
      if (orderAssignToCourierResult.IsFailure) return orderAssignToCourierResult.Error;


      var courierSetBusyResult = fastestCourier.SetBusy();
      if (courierSetBusyResult.IsFailure) return courierSetBusyResult.Error;

      return fastestCourier;
   }
   
   /// <summary>
   ///     Ошибки, которые может возвращать сущность
   /// </summary>
   [ExcludeFromCodeCoverage]

   public static class Errors
   {
      public static Error CantDispatchNotCreatedOrder()
      {
         return new Error($"{nameof(Order).ToLowerInvariant()}.cant.dispatch.not.created.order", 
            $"Подбор курьеров можно делать только для заказа в статусе {nameof(OrderStatus.Created)}");
      }
      
      public static Error SuitableCourierWasNotFound()
      {
         return new Error("suitable.courier.was.not.found",
            "Подходящий курьер не был найден");
      }


   }
}