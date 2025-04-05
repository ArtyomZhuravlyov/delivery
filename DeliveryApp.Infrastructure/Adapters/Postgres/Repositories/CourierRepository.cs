﻿using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class CourierRepository : ICourierRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CourierRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(Courier courier)
    {
        await _dbContext.Couriers.AddAsync(courier);
    }

    public void Update(Courier courier)
    {
        _dbContext.Couriers.Update(courier);
    }

    public async Task<Maybe<Courier>> GetAsync(Guid courierId)
    {
        var courier = await _dbContext
            .Couriers
            .Include(x => x.Transport)
            .FirstOrDefaultAsync(o => o.Id == courierId);
        return courier;
    }

    public IEnumerable<Courier> GetAllFree()
    {
        var couriers = _dbContext
            .Couriers
            .Include(x => x.Transport)
            .Where(o => o.Status.Name == CourierStatus.Free.Name);
        return couriers;
    }
}
