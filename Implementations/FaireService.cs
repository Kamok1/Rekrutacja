using Abstractions;
using Models;
using Models.Faire;
using Newtonsoft.Json;
using RestSharp;

namespace Implementations;

public class FaireService : IFaireService
{
    private readonly RestClient _client;
    public FaireService(FaireSettings settings)
    {
        var options = new RestClientOptions(settings.BaseUrl);
        _client = new RestClient(options);
        _client.AddDefaultHeader("X-FAIRE-ACCESS-TOKEN", settings.XFaireAccessToken);
    }

    public async Task<List<FaireOrder>> GetOrdersAsync(int limit = 50, int page = 1, DateTimeOffset? lastUpdateTime = new DateTimeOffset?())
    {
        var request = new RestRequest("orders", Method.Get)
            .AddParameter("limit", limit > 50 ? 50 : limit)
            .AddParameter("page", page)
            .AddParameter("updated_at_min", (lastUpdateTime ?? DateTime.MinValue).ToString("s"));

        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful == false || response.Content == null)
            throw new Exception("Failed to retrieve orders from faire!");

        var orders = JsonConvert.DeserializeObject<OrdersResponse>(response.Content)?.Orders;
        if (orders == null || orders.Any() == false)
            throw new Exception("Failed to retrieve orders from faire!");

        return orders!;
    }
}