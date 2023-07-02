using Models.Baselinker.Request;
using Models.Baselinker.Response;

namespace Abstractions;

public interface IBaselinkerService
{
    Task AddOrderAsync(NewOrder newOrder);
    /// <summary>
    /// Retrieves a list of orders ordered by ascending date.
    /// </summary>
    /// <param name="dateFrom">The starting date for retrieving orders</param>
    /// <param name="statusId">The status ID to filter the orders</param>
    /// <param name="customSourceId">The custom source ID to filter the orders</param>
    /// <returns></returns>
    Task<List<BaselinkerOrder>> GetOrdersAsync(DateTimeOffset dateFrom, int statusId, int customSourceId);
}