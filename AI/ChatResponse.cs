using System.Text.Json.Serialization;

namespace AI
{
    public class ChatResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }
    }
}
