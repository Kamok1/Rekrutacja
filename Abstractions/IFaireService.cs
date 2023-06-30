using Models.Faire;
namespace Abstractions;

public interface IFaireService
{
    /// <summary>
    /// Retrieves a list of orders ordered by ascending update date asynchronously.
    /// </summary>
    /// <param name="limit">The maximum number of orders to retrieve.</param>
    /// <param name="page">The page number of the results.</param>
    /// <param name="lastUpdateTime">Returns only orders updated after this date.</param>
    /// <returns>A list of orders.</returns>
    Task<List<FaireOrder>> GetOrdersAsync(int limit = 50, int page = 1, DateTimeOffset? lastUpdateTime = new DateTimeOffset?());
}