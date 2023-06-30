using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Models;
public record BaselinkerSettings
{
    public string XBLToken { get; set; }
    public string BaseUrl { get; set; }
    public int DefaultSourceId { get; set; }
    public int DefaultStatusId { get; set; }
}