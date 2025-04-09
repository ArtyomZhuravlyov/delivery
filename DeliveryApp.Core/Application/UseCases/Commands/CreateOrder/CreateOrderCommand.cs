using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<UnitResult<Error>>
{
    /// <summary>
    ///     Ctr
    /// </summary>
    /// <param name="basketId">Идентификатор корзины</param>
    /// <param name="street">Улица</param>
    /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
    public CreateOrderCommand(Guid basketId, string street)
    {
        if (basketId == Guid.Empty) throw new ArgumentException(nameof(basketId));
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException(nameof(street));

        BasketId = basketId;
        Street = street;
    }

    /// <summary>
    ///     Идентификатор корзины
    /// </summary>
    /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
    public Guid BasketId { get; }

    /// <summary>
    ///     Улица
    /// </summary>
    /// <remarks>Корзина содержала полный Address, но для упрощения мы будем использовать только Street из Address</remarks>
    public string Street { get; }

}