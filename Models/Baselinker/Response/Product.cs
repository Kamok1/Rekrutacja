using Newtonsoft.Json;

namespace Models.Baselinker.Response;

public record Product
{
    [JsonProperty("storage")]
    public string Storage { get; set; }

    [JsonProperty("storage_id")]
    public int StorageId { get; set; }

    [JsonProperty("order_product_id")]
    public string OrderProductId { get; set; }

    [JsonProperty("product_id")]
    public string ProductId { get; set; }

    [JsonProperty("variant_id")]
    public int VariantId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("attributes")]
    public string Attributes { get; set; }

    [JsonProperty("sku")]
    public string Sku { get; set; }

    [JsonProperty("ean")]
    public string Ean { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }

    [JsonProperty("auction_id")]
    public string AuctionId { get; set; }

    [JsonProperty("price_brutto")]
    public double PriceBrutto { get; set; }

    [JsonProperty("tax_rate")]
    public int TaxRate { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("weight")]
    public int Weight { get; set; }

    [JsonProperty("bundle_id")]
    public int BundleId { get; set; }
}