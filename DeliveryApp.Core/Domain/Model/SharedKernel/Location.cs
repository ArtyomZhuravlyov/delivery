using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.SharedKernel;

public class Location : ValueObject
{
    private const int MinCoordinate = 1;
    private const int MaxCoordinate = 10;

    public static Location Random => new Location(new Random().Next(MinCoordinate, MaxCoordinate),
        new Random().Next(MinCoordinate, MaxCoordinate));
    
    [ExcludeFromCodeCoverage]
    private Location()
    {
    }
    
    private Location(int x, int y) : this()
    {
        X = x;
        Y = y;
    }
    
    public int X { get; } 
    public int Y { get; }

    public static Result<Location, Error> Create(int x, int y)
    {
        if (x < MinCoordinate || x > MaxCoordinate)
            return GeneralErrors.ValueIsInvalid(nameof(x));
        if (y < MinCoordinate || y > MaxCoordinate)
            return GeneralErrors.ValueIsInvalid(nameof(y));

        return new Location(x, y);
    }

    public Result<int, Error> CalculateDistance(Location otherLocation)
    {
        if(otherLocation is null)
            return GeneralErrors.ValueIsInvalid(nameof(otherLocation));
        
        return Math.Abs(X - otherLocation.X) + Math.Abs(Y - otherLocation.Y);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}