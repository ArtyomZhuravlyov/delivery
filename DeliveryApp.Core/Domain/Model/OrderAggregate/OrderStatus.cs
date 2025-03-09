using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;


public class OrderStatus : ValueObject
{
    public static readonly OrderStatus Created = new(nameof(Created).ToLowerInvariant());
    public static readonly OrderStatus Assigned = new(nameof(Assigned).ToLowerInvariant());
    public static readonly OrderStatus Completed = new(nameof(Completed).ToLowerInvariant());
        
    /// <summary>
    ///     Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private OrderStatus()
    {
    }

    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="name">Название</param>
    private OrderStatus(string name) : this()
    {
        Name = name;
    }

    /// <summary>
    ///     Название
    /// </summary>
    public string Name { get; private set;}

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
