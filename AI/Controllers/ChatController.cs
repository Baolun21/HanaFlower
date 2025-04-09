using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace AI.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [AllowAnonymous]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ChatController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] ChatRequest request)
        {
            var websiteInfo = new WebsiteInfo();
            string additionalInfo = string.Empty;

            if (request.Question.Contains("tên", StringComparison.OrdinalIgnoreCase) && request.Question.Contains("web", StringComparison.OrdinalIgnoreCase))
            {
                additionalInfo = $"Nếu tôi hỏi tên web, bạn hãy trả lời tên website là {websiteInfo.Name}.";
            }
            else if (request.Question.Contains("địa chỉ", StringComparison.OrdinalIgnoreCase))
            {
                additionalInfo = $"Nếu tôi hỏi địa chỉ, Địa chỉ của trang web tôi đang hỏi là {websiteInfo.Address}.";
            }
            else if (request.Question.Contains("điện thoại", StringComparison.OrdinalIgnoreCase) || request.Question.Contains("dt", StringComparison.OrdinalIgnoreCase) || request.Question.Contains("đt", StringComparison.OrdinalIgnoreCase) || request.Question.Contains("sđt", StringComparison.OrdinalIgnoreCase))
            {
                additionalInfo = $"Nếu tôi hỏi số điện thoại, Số điện thoại liên hệ tôi đang hỏi là {websiteInfo.Phone}.";
            }
            else if (request.Question.Contains("mail", StringComparison.OrdinalIgnoreCase))
            {
                additionalInfo = $"Nếu tôi hỏi email, Email liên hệ tôi đang hỏi là {websiteInfo.Email}.";
            }

            var fullPrompt = $"{additionalInfo} {request.Question}".Trim();

            var payload = new
            {
                model = "llama3.2:latest",
                prompt = fullPrompt,
                stream = true
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://100.80.241.22:11434/api/generate")
            {
                Content = jsonContent
            };

            var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error communicating with LLM");
            }

            var responseStream = await response.Content.ReadAsStreamAsync();

            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            using var reader = new StreamReader(responseStream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    await Response.WriteAsync($"data: {line}\n\n");
                    await Response.Body.FlushAsync();
                }
            }

            return new EmptyResult();
        }
    }
}
