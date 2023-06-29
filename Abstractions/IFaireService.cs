using Models.Faire;
namespace Abstractions;

public interface IFaireService
{
    /// <summary>
    /// Retrieves a list of orders ordered by ascending update date asynchronously.
    /// If date filter is not provided, it will return only orders that haven't been retrieved yet.
    /// </summary>
    /// <param name="limit">The maximum number of orders to retrieve (max 50).</param>
    /// <param name="page">The page number of the results.</param>
    /// <param name="lastUpdateTime">Returns only orders updated latter than this date.</param>
    /// <returns>A list of orders.</returns>
    Task<List<Order>> GetOrdersAsync(int limit = 50, int page = 1, DateTime? lastUpdateTime = new DateTime?());
}