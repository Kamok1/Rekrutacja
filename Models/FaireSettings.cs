using Newtonsoft.Json;

namespace Models;

public record FaireSettings
{
    public string BaseUrl { get; set; }
    public string XFaireAccessToken { get; set; }
}