using Abstractions;
using Models;
using Models.Baselinker.Request;
using Models.Baselinker.Response;
using Newtonsoft.Json;
using RestSharp;

namespace Implementations;

public class BaselinkerService : IBaselinkerService
{
    private readonly RestClient _client;
    public BaselinkerService(BaselinkerSettings settings)
    {
        var options = new RestClientOptions(settings.BaseUrl);
        _client = new RestClient(options);
        _client.AddDefaultHeader("X-BLToken", settings.XBLToken);
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

        var response = await _client.ExecuteAsync<NewOrderResponse>(request);
        if (response.IsSuccessful == false || response.Content == null)
            throw new Exception("Error occurred while adding orders!");

        var status  = JsonConvert.DeserializeObject<NewOrderResponse>(response.Content)?.Status;
        if(status == "ERROR")
            throw new Exception("Error occurred while adding orders!");
    }
    public async Task<List<BaselinkerOrder>> GetOrdersAsync(DateTimeOffset dateFrom, int statusId, int customSourceId)
    {
        List<BaselinkerOrder> orders = new();
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

        var response = await _client.ExecuteAsync<List<OrdersResponse>>(request);
        if (response.IsSuccessful == false || response.Content == null)
            throw new Exception("Failed to retrieve orders from baselinker!");

        var collection = JsonConvert.DeserializeObject<OrdersResponse>(response.Content)?.Orders;
        if (collection != null)
            orders.AddRange(collection);

        return orders;
    }
}