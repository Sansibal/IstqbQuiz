using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using IstqbQuiz.Client;
using IstqbQuiz.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient für API-Aufrufe (gleiche Domain)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// State + Service
builder.Services.AddSingleton<QuizState>();
builder.Services.AddScoped<QuestionService>();

await builder.Build().RunAsync();
