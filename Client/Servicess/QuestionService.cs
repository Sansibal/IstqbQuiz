using IstqbQuiz.Shared.Models;
using System.Net.Http.Json;

namespace IstqbQuiz.Client.Services
{
    public class QuestionService
    {
        private readonly HttpClient _http;
        public QuestionService(HttpClient http) => _http = http;

        // -------------------------------------------
        // Lädt ALLE Fragen einer Kategorie
        // - category: "ctfl" (Default) oder "ctmat" etc.
        // - Server-Route: GET api/questions/{category}
        // -------------------------------------------
        public async Task<List<Question>> GetAllAsync(string category = "ctfl")
        {
            // Hinweis: Der Controller akzeptiert auch api/questions (ohne Kategorie) als Fallback für ctfl,
            // wir rufen aber explizit die Kategorie auf, damit zukünftige Kategorien sauber funktionieren.
            return await _http.GetFromJsonAsync<List<Question>>($"api/questions/{category}") ?? new();
        }

        // -------------------------------------------
        // Lädt ZUFÄLLIGE Fragen einer Kategorie
        // - category: "ctfl" (Default) oder "ctmat" etc.
        // - Server-Route: GET api/questions/{category}/random/{count}
        // -------------------------------------------
        public async Task<List<Question>> GetRandomAsync(int count, string category = "ctfl")
        {
            return await _http.GetFromJsonAsync<List<Question>>($"api/questions/{category}/random/{count}") ?? new();
        }

        // -------------------------------------------
        // Client-seitiges Debug/Advanced-Filtering:
        // - Holt ALLE Fragen einer Kategorie und filtert lokal (ID-Range, K-Level, Topics, Shuffle)
        // -------------------------------------------
        public async Task<List<Question>> GetDebugQuizAsync(
            int count,
            int? minId = null,
            int? maxId = null,
            bool shuffle = false,
            int? kLevel = null,
            List<string>? topics = null,
            string category = "ctfl")   // ✅ NEU: Kategorie-Parameter (Default ctfl)
        {
            // 1) Alles von der gewünschten Kategorie laden
            var all = await GetAllAsync(category);

            // 2) ID-Range
            if (minId.HasValue)
                all = all.Where(q => q.Id >= minId.Value).ToList();
            if (maxId.HasValue)
                all = all.Where(q => q.Id <= maxId.Value).ToList();

            // 3) K-Level
            if (kLevel.HasValue)
                all = all.Where(q => q.KLevel == kLevel.Value).ToList();

            // 4) Topics (mehrere möglich; case-insensitive)
            if (topics != null && topics.Any())
            {
                all = all.Where(q => topics.Contains(q.Topic, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            // 5) Shuffle optional
            if (shuffle)
                all = all.OrderBy(_ => Guid.NewGuid()).ToList();

            // 6) gewünschte Anzahl zurückgeben (sofern vorhanden)
            return all.Take(Math.Max(0, count)).ToList();
        }

        // -------------------------------------------
        // (Optional) Beibehaltener, einfacher Debug-Endpunkt für alte Aufrufe
        // - NICHT empfohlen, aber hilfreich falls bestehender Code noch darauf zeigt.
        // -------------------------------------------
        public async Task<List<Question>> GetDebugQuizAsync(int count, int? minId = null, int? maxId = null, bool shuffle = false)
        {
            return await GetDebugQuizAsync(count, minId, maxId, shuffle, null, null, "ctfl");
        }
    }
}
