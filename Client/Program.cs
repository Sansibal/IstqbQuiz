using IstqbQuiz.Shared.Models;
using IstqbQuiz.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP Client für API oder statische Daten
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Services registrieren
builder.Services.AddScoped<QuestionService>();
builder.Services.AddSingleton<QuizState>();

var host = builder.Build();

// Environment prüfen
var env = host.Services.GetRequiredService<IWebAssemblyHostEnvironment>();

if (env.IsProduction())
{
    var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
    await jsRuntime.InvokeVoidAsync("navigator.serviceWorker.register", "service-worker.js");
}

await host.RunAsync();

