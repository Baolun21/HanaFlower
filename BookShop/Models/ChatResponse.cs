using System.Text.Json.Serialization;

namespace BookShop.Models
{
    public class ChatResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }
    }
}
