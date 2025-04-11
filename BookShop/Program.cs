using Microsoft.EntityFrameworkCore;
using BookShop.Data.Contexts;
using Microsoft.Extensions.AI;
using BookShop.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
    //options.ListenAnyIP(5000);
//});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["DbContextSetting:ConnectionString"]);
});
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Configuration setup Ollama AI
var ollamaEndpoint = builder.Configuration["AIOllamaEndpoint"] ?? "http://localhost:11434";
var chatModel = builder.Configuration["AIOllamaChatModel"] ?? "llama3.2";

// Create and configure Ollama client
IChatClient client = new OllamaChatClient(ollamaEndpoint, modelId: chatModel)
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

builder.Services.AddChatClient(client);
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Cache()
            .Expire(TimeSpan.FromMinutes(5)));
});

builder.Services.AddTransient<IVnpay, Vnpay>();

var app = builder.Build();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
   //ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapOpenApi().CacheOutput();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.Run();
