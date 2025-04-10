using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCouriers;

public class GetCouriersHandler : IRequestHandler<GetCouriersQuery, GetCouriersResponse>
{
    private readonly string _connectionString;

    public GetCouriersHandler(string connectionString)
    {
        _connectionString = !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<GetCouriersResponse> Handle(GetCouriersQuery message,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var result = (await connection.QueryAsync<dynamic>(
            @"SELECT id, name, location_x, location_y FROM public.couriers"
            , new { })).AsList();

        if (result.Count == 0)
            return null;

        var couriers = new List<Courier>();
        foreach (var item in result)
            couriers.Add(MapToCourier(item));

        return new GetCouriersResponse(couriers);
    }

    private Courier MapToCourier(dynamic result)
    {
        var location = new Location { X = result.location_x, Y = result.location_y };
        var courier = new Courier
            { Id = result.id, Name = result.name, Location = location, TransportId = result.transport_id };
        return courier;
    }
}
