using IstqbQuiz.Shared.Models;
using System.Net.Http.Json;

namespace IstqbQuiz.Client.Services
{
    public class QuestionService
    {
        private readonly HttpClient _http;
        public QuestionService(HttpClient http) => _http = http;

        public async Task<List<Question>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<Question>>("api/questions") ?? new();
        }

        public async Task<List<Question>> GetRandomAsync(int count)
        {
            return await _http.GetFromJsonAsync<List<Question>>($"api/questions/random/{count}") ?? new();
        }

        public async Task<List<Question>> GetDebugQuizAsync(int count, int? minId = null, int? maxId = null, bool shuffle = false)
        {
            var all = await GetAllAsync();

            // ID-Range filtern
            if (minId.HasValue)
                all = all.Where(q => q.Id >= minId.Value).ToList();
            if (maxId.HasValue)
                all = all.Where(q => q.Id <= maxId.Value).ToList();

            // optional mischen
            if (shuffle)
                all = all.OrderBy(_ => Guid.NewGuid()).ToList();

            return all.Take(count).ToList();
        }
    }
}
