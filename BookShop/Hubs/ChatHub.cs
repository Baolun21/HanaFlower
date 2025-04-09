using BookShop.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Text.Json;

namespace BookShop.Hubs
{
    public class ChatHub : Hub
    {
        private readonly HttpClient _httpClient;

        public ChatHub(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendQuestion(string question)
        {
            var chatRequest = new ChatRequest { Question = question };
            var jsonContent = new StringContent(JsonSerializer.Serialize(chatRequest), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://hanaflower.click:5066/api/chat/ask")
            {
                Content = jsonContent
            };

            var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);


            if (!response.IsSuccessStatusCode)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Error communicating with API.");
                return;
            }

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", line);
                }
            }
        }
    }
}
