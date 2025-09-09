using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using IstqbQuiz.Client;
using IstqbQuiz.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient so, dass relative URLs (base href) automatisch gewürdigt werden
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Application state (in-memory) für Quiz / Ergebnis
builder.Services.AddSingleton<QuizState>();

await builder.Build().RunAsync();
