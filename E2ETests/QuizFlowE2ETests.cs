// Datei: E2ETests/QuizFlowE2ETests.cs
using Microsoft.Playwright;
using Xunit;

namespace IstqbQuiz.E2ETests
{
    /// <summary>
    /// Echte Browser-E2E-Tests gegen die laufende App (Client + Server + API zusammen).
    /// Basis-URL über Umgebungsvariable E2E_BASE_URL konfigurierbar, Default = Live-Deployment.
    /// Lokal gegen "dotnet run --project Server" testen: E2E_BASE_URL=http://localhost:5000
    /// </summary>
    public class QuizFlowE2ETests : IAsyncLifetime
    {
        private static readonly string BaseUrl =
            Environment.GetEnvironmentVariable("E2E_BASE_URL")?.TrimEnd('/') ?? "https://istqbquiz.onrender.com";

        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }

        public async Task DisposeAsync()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Fact]
        public async Task CtflQuiz_CompleteFlow_StartToResult_ShowsScore()
        {
            var page = await _browser.NewPageAsync();

            await page.GotoAsync($"{BaseUrl}/quiz");
            await page.GetByRole(AriaRole.Button, new() { Name = "Start (40 Fragen)" }).ClickAsync();

            // Bis zu 40 Fragen durchklicken: jeweils erste Option wählen und "Weiter"
            for (var i = 0; i < 40; i++)
            {
                var resultVisible = await page.GetByText("Dein Ergebnis").IsVisibleAsync();
                if (resultVisible) break;

                var optionButtons = page.Locator("div.d-grid > button");
                await optionButtons.First.ClickAsync();

                var weiter = page.GetByRole(AriaRole.Button, new() { Name = "Weiter", Exact = true });
                await weiter.ClickAsync();
            }

            await page.WaitForURLAsync($"{BaseUrl}/result");
            await Assertions.Expect(page.GetByText("Punkte:")).ToBeVisibleAsync();

            await page.CloseAsync();
        }

        [Fact]
        public async Task DetailsPage_WithoutPriorQuiz_ShowsNoDataMessage()
        {
            var page = await _browser.NewPageAsync();

            await page.GotoAsync($"{BaseUrl}/details");

            await Assertions.Expect(page.GetByText("Keine Daten vorhanden")).ToBeVisibleAsync();

            await page.CloseAsync();
        }

        [Fact]
        public async Task HomePage_NavigationLinks_PointToExpectedRoutes()
        {
            var page = await _browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);

            var expectedRoutes = new (string Text, string Href)[]
            {
                ("CTFL Quiz", "quiz"),
                ("CT-MAT Quiz", "quizctmat"),
                ("Quiz (mit Filter)", "quizadvanced"),
                ("Ergebnis", "result"),
                ("Details", "details"),
                ("Danksagung", "danksagung"),
            };

            foreach (var (text, href) in expectedRoutes)
            {
                // Manche Links tauchen zusätzlich im Footer/Hinweistext auf, daher .First
                var link = page.GetByRole(AriaRole.Link, new() { Name = text }).First;
                await Assertions.Expect(link).ToHaveAttributeAsync("href", href);
            }

            await page.CloseAsync();
        }
    }
}
