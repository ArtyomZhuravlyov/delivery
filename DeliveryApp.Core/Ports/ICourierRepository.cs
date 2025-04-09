using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface ICourierRepository : IRepository<Courier>
{
    /// <summary>
    ///     Добавить
    /// </summary>
    /// <param name="courier">Курьер</param>
    /// <returns>Курьер</returns>
    Task AddAsync(Courier courier);

    /// <summary>
    ///     Обновить
    /// </summary>
    /// <param name="courier">Курьер</param>
    void Update(Courier courier);

    /// <summary>
    ///     Получить
    /// </summary>
    /// <param name="courierId">Идентификатор</param>
    /// <returns>Курьер</returns>
    Task<Maybe<Courier>> GetAsync(Guid courierId);

    /// <summary>
    ///     Получить всех свободных курьеров
    /// </summary>
    /// <returns>Курьеры</returns>
    Task<List<Courier>> GetAllFreeAsync();
}
