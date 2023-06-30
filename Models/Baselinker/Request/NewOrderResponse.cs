using Newtonsoft.Json;

namespace Models.Baselinker.Request;

public record NewOrderResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("order_id")]
    public string OrderId { get; set; }
}