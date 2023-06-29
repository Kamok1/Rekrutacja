using Abstractions;
using Models;
using Models.Baselinker;
using Newtonsoft.Json;
using RestSharp;
using Helpers;

namespace Implementations;

public class BaselinkerService : IBaselinkerService
{
    private readonly RestClient _client;
    private readonly BaselinkerSettings _settings;
    public BaselinkerService(IStorageService storage, BaselinkerSettings settings)
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
}