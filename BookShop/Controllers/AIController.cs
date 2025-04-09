using BookShop.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using System.ComponentModel;


namespace BookShop.Controllers
{
    public class AIController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AIController(AppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Chat(IChatClient client, string message)
        {
            [Description("Gets the weather")]
            string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";

            [Description("Gets the location")]
            string GetLocation() => "Dong Nai, Viet Nam";

            [Description("Gets the weather and location")]
            string GetWeatherAndLocation() => $"{GetWeather()} in {GetLocation()}";

            var chatOptions = new ChatOptions
            {
                Tools =
                [
                    AIFunctionFactory.Create(GetWeather),
                    AIFunctionFactory.Create(GetLocation),
                    AIFunctionFactory.Create(GetWeatherAndLocation)
                ]
            };

            var response = await client.GetResponseAsync(message, chatOptions, cancellationToken: default);

            return Ok(new { response });
        }
    }
}
