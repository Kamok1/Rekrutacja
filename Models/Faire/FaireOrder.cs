using System.Globalization;
using Newtonsoft.Json;

namespace Models.Faire;

public record FaireOrder
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAtString { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt => DateTimeOffset.ParseExact(CreatedAtString, "yyyyMMdd'T'HHmmss.fff'Z'", CultureInfo.InvariantCulture);

    [JsonProperty("updated_at")]
    public string UpdatedAtString { get; set; }

    [JsonIgnore]
    public DateTimeOffset UpdatedAt => DateTimeOffset.ParseExact(UpdatedAtString, "yyyyMMdd'T'HHmmss.fff'Z'", CultureInfo.InvariantCulture);


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