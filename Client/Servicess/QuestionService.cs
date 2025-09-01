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
            var all = await GetAllAsync();
            return all.OrderBy(_ => Guid.NewGuid()).Take(count).ToList();
        }
    }
}
