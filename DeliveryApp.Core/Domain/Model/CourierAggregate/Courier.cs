using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Courier : Aggregate<Guid>
{
    private Courier()
    {
    }
    
    private Courier(string name, Transport transport, Location location) : this()
    {
        Id = Guid.NewGuid();
        Name = name;
        Transport = transport;
        Location = location;
        Status = CourierStatus.Free;
    }
    
    public string Name  { get; }
    
    public Transport Transport { get; }
    public Location Location { get; private set; }
    public CourierStatus Status  { get; private set; }


    /// <summary>
    ///     Factory Method
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="transport">Транспорт</param>
    /// <param name="location">Местоположение</param>
    /// <returns>Результат</returns>
    public static Result<Courier, Error> Create(string name, Transport transport, Location location)
    {
        if (string.IsNullOrEmpty(name)) return GeneralErrors.ValueIsRequired(nameof(name));
        if (transport == null) return GeneralErrors.ValueIsRequired(nameof(transport));
        if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));

        return new Courier(name, transport, location);
    }

    
    /// <summary>
    ///     Сделать курьера занятым
    /// </summary>
    /// <returns>Результат</returns>
    public UnitResult<Error> SetBusy()
    {
        if (Status == CourierStatus.Busy) return Errors.CourierHasAlreadyBusy();

        Status = CourierStatus.Busy;
        return UnitResult.Success<Error>();
    }

    /// <summary>
    ///     Сделать курьера свободным
    /// </summary>
    /// <returns>Результат</returns>
    public UnitResult<Error> SetFree()
    {
        Status = CourierStatus.Free;
        return UnitResult.Success<Error>();
    }
    
    public Result<int, Error> GetSteps(Location orderLocation)
    {
        if(orderLocation == null) return GeneralErrors.ValueIsRequired(nameof(orderLocation));
        var steps = Location.CalculateDistance(orderLocation).Value / Transport.Speed;
        return steps;
    }

    public UnitResult<Error> Move(Location targetLocation)
    {
        if (targetLocation == null) return GeneralErrors.ValueIsRequired(nameof(targetLocation));

        var difX = targetLocation.X - Location.X;
        var difY = targetLocation.Y - Location.Y;

        var newX = Location.X;
        var newY = Location.Y;

        var cruisingRange = Transport.Speed;

        if (difX > 0)
        {
            if (difX >= cruisingRange)
            {
                newX += cruisingRange;
                Location = Location.Create(newX, newY).Value;
                return UnitResult.Success<Error>();
            }

            if (difX < cruisingRange)
            {
                newX += difX;
                Location = Location.Create(newX, newY).Value;
                if (Location == targetLocation) 
                    return UnitResult.Success<Error>();
                cruisingRange -= difX;
            }
        }

        if (difX < 0)
        {
            if (Math.Abs(difX) >= cruisingRange)
            {
                newX -= cruisingRange;
                Location = Location.Create(newX, newY).Value;
                return UnitResult.Success<Error>();
            }

            if (Math.Abs(difX) < cruisingRange)
            {
                newX -= Math.Abs(difX);
                Location = Location.Create(newX, newY).Value;
                if (Location == targetLocation)
                    return UnitResult.Success<Error>();
                cruisingRange -= Math.Abs(difX);
            }
        }

        if (difY > 0)
        {
            if (difY >= cruisingRange)
            {
                newY += cruisingRange;
                Location = Location.Create(newX, newY).Value;
                return UnitResult.Success<Error>();
            }

            if (difY < cruisingRange)
            {
                newY += difY;
                Location = Location.Create(newX, newY).Value;
                if (Location == targetLocation) 
                    return UnitResult.Success<Error>();
            }
        }

        if (difY < 0)
        {
            if (Math.Abs(difY) >= cruisingRange)
            {
                newY -= cruisingRange;
                Location = Location.Create(newX, newY).Value;
                return UnitResult.Success<Error>();
            }

            if (Math.Abs(difY) < cruisingRange)
            {
                newY -= Math.Abs(difY);
                Location = Location.Create(newX, newY).Value;
                if (Location == targetLocation) 
                    return UnitResult.Success<Error>();
            }
        }

        Location = Location.Create(newX, newY).Value;
        return UnitResult.Success<Error>();
    }

    
    /// <summary>
    ///     Ошибки, которые может возвращать сущность
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Errors
    {
        public static Error CourierHasAlreadyBusy()
        {
            return new Error($"{nameof(Courier).ToLowerInvariant()}.cant.set.busy.to.courier.already.busy",
                "Нельзя занят курьера, т.к. он уже занят");
        }
    }
}