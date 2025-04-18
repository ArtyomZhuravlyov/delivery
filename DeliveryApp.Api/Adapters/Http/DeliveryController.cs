using Api.Controllers;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Location = DeliveryApp.Core.Application.UseCases.Queries.GetCouriers.Location;

namespace DeliveryApp.Api.Adapters.Http;

public class DeliveryController : DefaultApiController
{
    private readonly IMediator _mediator;

    public DeliveryController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    
    public override async  Task<IActionResult> CreateOrder()
    {
        var orderId = Guid.NewGuid();
        var street = "Несуществующая";
        var createOrderCommand =
            new CreateOrderCommand(orderId, street);
        var response = await _mediator.Send(createOrderCommand);
        if (response.IsSuccess) return Ok();
        return Conflict(response.Error.Message); // не делайте так в проде!

    }

    public override async  Task<IActionResult> GetCouriers()
    {
        // Вызываем Query
        var getAllCouriersQuery = new GetCouriersQuery();
        var response = await _mediator.Send(getAllCouriersQuery);

        // Мапим результат Query на Model
        if (response == null) return NotFound();
        var model = response.Couriers.Select(c => new Courier
        {
            Id = c.Id,
            Name = c.Name,
            Location = new Location { X = c.Location.X, Y = c.Location.Y },
            TransportId = c.TransportId,
        });
        return Ok(model);

    }

    public override async  Task<IActionResult> GetOrders()
    {
        // Вызываем Query
        var getActiveOrdersQuery = new GetCreatedAndAssignedOrdersQuery();
        var response = await _mediator.Send(getActiveOrdersQuery);

        // Мапим результат Query на Model
        if (response == null) return NotFound();
        var model = response.Orders.Select(o => new Order
        {
            Id = o.Id,
            Location = new Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders.Location() { X = o.Location.X, Y = o.Location.Y }
        });
        return Ok(model);

    }
}