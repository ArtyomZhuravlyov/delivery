using Dapper;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;

public class GetCreatedAndAssignedOrdersHandler : IRequestHandler<GetCreatedAndAssignedOrdersQuery,
    GetCreatedAndAssignedOrdersResponse>
{
    private readonly string _connectionString;

    public GetCreatedAndAssignedOrdersHandler(string connectionString)
    {
        _connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<GetCreatedAndAssignedOrdersResponse> Handle(GetCreatedAndAssignedOrdersQuery message,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var result = (await connection.QueryAsync<dynamic>(
            @"SELECT id, courier_id, location_x, location_y, status FROM public.orders where status!=@status;"
            , new { status = OrderStatus.Completed.Name })).AsList();

        if (result.Count == 0)
            return null;

        var orders = new List<Order>();
        foreach (var item in result) 
            orders.Add(MapToOrder(item));

        return new GetCreatedAndAssignedOrdersResponse(orders);
    }

    private Order MapToOrder(dynamic result)
    {
        var location = new Location { X = result.location_x, Y = result.location_y };
        var order = new Order { Id = result.id, Location = location };
        return order;
    }
}
