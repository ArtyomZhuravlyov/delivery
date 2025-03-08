using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Transport : Entity<int>
{
    
    /// <summary>
    ///     Пешеход
    /// </summary>
    public static readonly Transport Pedestrian  = new(1, "пешеход", 1);

    /// <summary>
    ///     Велосипедист
    /// </summary>
    public static readonly Transport Bicycle  = new(2, "велосипедист", 2);

    /// <summary>
    ///     Автомобиль
    /// </summary>
    public static readonly Transport Car  = new(3, "автомобиль", 3);

    
    /// <summary>
    ///     Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private Transport()
    {
    }

    /// <summary>
    ///     Ctr
    /// </summary>
    private Transport(int id, string name, int speed) : this()
    {
        Id = id;
        Name = name;
        Speed = speed;
    }
    
    
    /// <summary>
    ///     Название
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Скорость
    /// </summary>
    public int Speed { get; }
    
    
    /// <summary>
    ///     Список всех значений списка
    /// </summary>
    /// <returns>Значения списка</returns>
    public static IEnumerable<Transport> List()
    {
        yield return Pedestrian;
        yield return Bicycle;
        yield return Car;
    }

    /// <summary>
    ///     Получить транспорт по названию
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Transport</returns>
    public static Result<Transport, Error> FromName(string name)
    {
        var state = List()
            .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (state == null) return Errors.TransportIsWrong();
        return state;
    }


    public static Result<Transport, Error> FromId(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);
        if (state == null) return Errors.TransportIsWrong();
        return state;
    }

    /// <summary>
    ///     Ошибки, которые может возвращать сущность
    /// </summary>
    private static class Errors
    {
        public static Error TransportIsWrong()
        {
            return new Error($"{nameof(Transport).ToLowerInvariant()}.is.wrong",
                $"Неверное значение. Допустимые значения: {nameof(Transport).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
        }
    }

    


}