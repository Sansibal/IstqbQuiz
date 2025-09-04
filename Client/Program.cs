using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using IstqbQuiz.Client;
using IstqbQuiz.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// App in #app einfügen
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient registrieren
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Eigene Services
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<QuizState>();

await builder.Build().RunAsync();
