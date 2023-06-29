using Abstractions;
using Models;
using Models.Faire;
using Newtonsoft.Json;
using RestSharp;
using System.Xml.Serialization;
using Helpers;

namespace Implementations;

public class FaireService : IFaireService
{
    private readonly RestClient _client;
    private readonly IStorageService _storage;
    public FaireService(FaireSettings settings, IStorageService storage)
    {
        var options = new RestClientOptions(settings.BaseUrl);
        _client = new RestClient(options);
        _client.AddDefaultHeader("X-FAIRE-ACCESS-TOKEN", settings.XFaireAccessToken);
        _storage = storage;
    }

    public async Task<List<Order>> GetOrdersAsync(int limit = 50, int page = 1, DateTime? lastUpdateTime = new DateTime?())
    {
        EnsureThatLastUpdateDateExists();
        var request = new RestRequest("orders", Method.Get)
            .AddParameter("limit", limit)
            .AddParameter("page", page)
            .AddParameter("updated_at_min", lastUpdateTime?.ToString("s") ?? _storage.Get<string>("lastUpdatedDate"));

        var response = await _client.ExecuteAsync(request);
        if (response.Content == null)
            throw new Exception("Failed to retrieve orders from faire!");

        var orders = JsonConvert.DeserializeObject<OrdersResponse>(response.Content)?.Orders;
        if(orders != null && orders.Any())
            throw new Exception("Failed to retrieve orders from faire!");

        //From documentation: "This endpoint retrieves a list of orders, ordered ascending by updated_at."
        //So it is possible that some orders may appear more than once, but only if they have been updated
        if (lastUpdateTime .HasValue == false) 
            _storage.Set("lastUpdatedDate", orders.Last().UpdatedAt);

        return orders;
    }

    private void EnsureThatLastUpdateDateExists()
    {
        if(_storage.Exists("lastUpdatedDate") == false)
            _storage.Set("lastUpdatedDate", DateTime.MinValue.ToString("s"));
    }
}