using Newtonsoft.Json;

namespace Models.Faire;

public record Item
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public string UpdatedAt { get; set; }

    [JsonProperty("order_id")]
    public string OrderId { get; set; }

    [JsonProperty("product_id")]
    public string ProductId { get; set; }

    [JsonProperty("product_option_id")]
    public string ProductOptionId { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("sku")]
    public string Sku { get; set; }

    [JsonProperty("price_cents")]
    public int PriceCents { get; set; }

    [JsonProperty("product_name")]
    public string ProductName { get; set; }

    [JsonProperty("product_option_name")]
    public string ProductOptionName { get; set; }

    [JsonProperty("includes_tester")]
    public bool IncludesTester { get; set; }
}