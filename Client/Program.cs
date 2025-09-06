using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using IstqbQuiz.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

// Service Worker (PWA) nur in Production registrieren
if (builder.HostEnvironment.IsProduction())
{
    try
    {
        var js = host.Services.GetRequiredService<IJSRuntime>();
        // Pfad mit führendem Slash, damit es auch auf unterschiedlichen Basispfaden funktioniert
        await js.InvokeVoidAsync("navigator.serviceWorker.register", "/service-worker.js");
        Console.WriteLine("✅ Service Worker erfolgreich registriert.");
    }
    catch (JSException jsex)
    {
        Console.WriteLine($"⚠ Fehler bei Service Worker-Registrierung (JS): {jsex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Unbekannter Fehler bei Service Worker-Registrierung: {ex.Message}");
    }
}

await host.RunAsync();
