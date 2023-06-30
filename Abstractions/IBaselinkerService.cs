using Models.Baselinker.Request;
using Models.Baselinker.Response;

namespace Abstractions;

public interface IBaselinkerService
{
    Task AddOrderAsync(NewOrder newOrder);
    Task<List<Order>> GetOrdersAsync(DateTimeOffset dateFrom);
}