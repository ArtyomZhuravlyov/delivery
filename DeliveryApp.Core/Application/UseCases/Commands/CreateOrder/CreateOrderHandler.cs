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

    /// <summary>
    ///     Ctr
    /// </summary>
    public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<UnitResult<Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        // На случай если такой заказ уже создавали
        var getOrderResult = await _orderRepository.GetAsync(message.BasketId);
        if (getOrderResult.HasValue) return UnitResult.Success<Error>();

        // Получаем геопозицию из Geo (пока ставим фэйковое значение)
        var location = Location.Random;

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