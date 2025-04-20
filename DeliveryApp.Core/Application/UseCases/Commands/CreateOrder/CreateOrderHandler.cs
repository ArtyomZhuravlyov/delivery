using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeoClient _geoClient;

    /// <summary>
    ///     Ctr
    /// </summary>
    public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IGeoClient geoClient)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _geoClient = geoClient ?? throw new ArgumentNullException(nameof(geoClient));
    }

    public async Task<UnitResult<Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        // На случай если такой заказ уже создавали
        var getOrderResult = await _orderRepository.GetAsync(message.BasketId);
        if (getOrderResult.HasValue) return UnitResult.Success<Error>();

        
        var locationResult = await _geoClient.GetGeolocationAsync(message.Street, cancellationToken);
        if (locationResult.IsFailure) return locationResult;

        var location = locationResult.Value;


        // Создаем заказ
        var orderCreateResult = Order.Create(message.BasketId, location);
        if (orderCreateResult.IsFailure) return orderCreateResult;
        var order = orderCreateResult.Value;

        // Сохраняем
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

}