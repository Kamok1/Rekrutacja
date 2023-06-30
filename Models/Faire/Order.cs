using Newtonsoft.Json;

namespace Models.Faire;

public record Order
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public string UpdatedAt { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("items")]
    public List<Item> Items { get; set; }

    [JsonProperty("address")]
    public Address Address { get; set; }

    [JsonProperty("retailer_id")]
    public string RetailerId { get; set; }

    [JsonProperty("payout_costs")]
    public PayoutCosts PayoutCosts { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }
}