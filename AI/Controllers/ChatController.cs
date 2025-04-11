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
            string additionalInfo = "Đây là thông tin website của tôi. Tên là HanaFlower, địa chỉ website là hanaflower.click. Mở cửa mỗi ngày từ 9 giờ sáng tới 8 giờ tối. Website chuyên bán các loại hoa, số điện thoại: 0979357611. Sau đây là câu hỏi của tôi: ";

            
            var fullPrompt2 = $"{additionalInfo} {request.Question}".Trim();
            var fullPrompt = request.Question.Trim();

            var payload = new
            {
                model = "llama3.2:latest",
                prompt = fullPrompt,
                stream = true
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://100.113.57.83:11434/api/generate")
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
