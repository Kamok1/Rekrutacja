using Newtonsoft.Json;

namespace Models.Baselinker.Response;

public record OrdersResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("orders")]
    public List<BaselinkerOrder> Orders { get; set; }
}