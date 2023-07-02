using Models.Baselinker.Request;
using Models.Baselinker.Response;

namespace Abstractions;

public interface IBaselinkerService
{
    Task AddOrderAsync(NewOrder newOrder);
    Task<List<BaselinkerOrder>> GetOrdersAsync(DateTimeOffset dateFrom, int statusId, int customSourceId);
}