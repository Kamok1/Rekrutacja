using System.Text;
using Abstractions;
using Models;
using Newtonsoft.Json;
using RestSharp;
using Helpers;
using Models.Baselinker.Request;
using Models.Baselinker.Response;
using System.Collections.Generic; // Add this namespace for Dictionary and List

namespace Implementations;

public class BaselinkerService : IBaselinkerService
{
    private readonly RestClient _client;
    private readonly BaselinkerSettings _settings;
    public BaselinkerService(BaselinkerSettings settings)
    {
        _settings = settings;
        var options = new RestClientOptions(_settings.BaseUrl);
        _client = new RestClient(options);
        _client.AddDefaultHeader("X-BLToken", _settings.XBLToken);
    }
    public async Task AddOrderAsync(NewOrder newOrder)
    {
        var apiParams = new Dictionary<string, object>
        {
            { "method", "addOrder" },
            { "parameters",  JsonConvert.SerializeObject(newOrder) }
        };

        var request = new RestRequest("", Method.Post);
        request.AddParameter("application/x-www-form-urlencoded", Helpers.Helpers.QueryString(apiParams), ParameterType.RequestBody);

        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful == false)
            throw new Exception("Error occurred while adding orders!");
    }

    public async Task<List<Order>?> GetOrdersAsync(DateTimeOffset dateFrom)
    {
        return await GetOrdersAsync(dateFrom, _settings.DefaultStatusId, _settings.DefaultSourceId);
    }
    private async Task<List<Order>?> GetOrdersAsync(DateTimeOffset dateFrom, int statusId, int customSourceId)
    {
        List<Order> orders = new();
        var parametersDict = new Dictionary<string, object>
        {
            { "filter_order_source_id", customSourceId },
            { "status_id", statusId },
            { "date_from", dateFrom.ToUnixTimeSeconds().ToString() },
            { "get_unconfirmed_orders", true }
        };
        var apiParams = new Dictionary<string, object>
        {
            { "method", "getOrders" },
            { "parameters",  Helpers.Helpers.QueryString(parametersDict)}
        };

        var request = new RestRequest("", Method.Post);
        request.AddParameter("application/x-www-form-urlencoded", Helpers.Helpers.QueryString(apiParams), ParameterType.RequestBody);

        var response = await _client.ExecuteAsync<List<OrdersResponse>>(request); // Specify the response type as List<Order>
        if (response.IsSuccessful == false || response.Content == null)
            throw new Exception("Failed to retrieve orders from baselinker!");

        var collection = JsonConvert.DeserializeObject<OrdersResponse>(response.Content)?.Orders;
        if (collection != null)
            orders.AddRange(collection);

        return orders; // Return the list of orders
    }
}