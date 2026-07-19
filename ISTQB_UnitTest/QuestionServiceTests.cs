// Datei: ISTQB_UnitTest/QuestionServiceTests.cs
using System.Net;
using System.Net.Http.Json;
using IstqbQuiz.Client.Services;
using IstqbQuiz.Shared.Models;
using Moq;
using Moq.Protected;
using Xunit;

namespace ISTQB_Tests
{
    /// <summary>
    /// Testet QuestionService, insbesondere die lokale Filterlogik von GetDebugQuizAsync,
    /// die bisher komplett ungetestet war. HttpClient wird über einen gemockten
    /// HttpMessageHandler bedient, damit kein echter Server nötig ist.
    /// </summary>
    public class QuestionServiceTests
    {
        private static List<Question> SampleQuestions() => new()
        {
            new Question { Id = 1, Text = "Q1", Options = new() { "A", "B" }, CorrectIndexes = new() { 0 }, KLevel = 1, Topic = "Kapitel 1" },
            new Question { Id = 2, Text = "Q2", Options = new() { "A", "B" }, CorrectIndexes = new() { 1 }, KLevel = 2, Topic = "Kapitel 2" },
            new Question { Id = 3, Text = "Q3", Options = new() { "A", "B" }, CorrectIndexes = new() { 0 }, KLevel = 2, Topic = "kapitel 3" },
            new Question { Id = 4, Text = "Q4", Options = new() { "A", "B" }, CorrectIndexes = new() { 1 }, KLevel = 3, Topic = "Kapitel 3" },
            new Question { Id = 5, Text = "Q5", Options = new() { "A", "B" }, CorrectIndexes = new() { 0 }, KLevel = 1, Topic = "Kapitel 1" },
        };

        private static QuestionService CreateService(List<Question> questions, out List<string?> requestedUris)
        {
            var capturedUris = new List<string?>();
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
                {
                    capturedUris.Add(req.RequestUri?.ToString());
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(questions)
                    };
                });

            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
            requestedUris = capturedUris;
            return new QuestionService(httpClient);
        }

        [Fact]
        public async Task GetAllAsync_RequestsCorrectCategoryUrl_AndReturnsAllQuestions()
        {
            var service = CreateService(SampleQuestions(), out var uris);

            var result = await service.GetAllAsync("ctmat");

            Assert.Equal(5, result.Count);
            Assert.Equal("http://localhost/api/questions/ctmat", uris.Single());
        }

        [Fact]
        public async Task GetRandomAsync_RequestsCorrectCategoryAndCountUrl()
        {
            var service = CreateService(SampleQuestions(), out var uris);

            var result = await service.GetRandomAsync(3, "ctfl");

            Assert.Equal(5, result.Count); // Mock liefert immer den vollen Datensatz zurück
            Assert.Equal("http://localhost/api/questions/ctfl/random/3", uris.Single());
        }

        [Fact]
        public async Task GetDebugQuizAsync_FiltersByMinId()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 10, minId: 3, category: "ctfl");

            Assert.Equal(new[] { 3, 4, 5 }, result.Select(q => q.Id));
        }

        [Fact]
        public async Task GetDebugQuizAsync_FiltersByMaxId()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 10, maxId: 2, category: "ctfl");

            Assert.Equal(new[] { 1, 2 }, result.Select(q => q.Id));
        }

        [Fact]
        public async Task GetDebugQuizAsync_FiltersByIdRange_MinAndMaxCombined()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 10, minId: 2, maxId: 4, category: "ctfl");

            Assert.Equal(new[] { 2, 3, 4 }, result.Select(q => q.Id));
        }

        [Fact]
        public async Task GetDebugQuizAsync_FiltersByKLevel()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 10, kLevel: 2);

            Assert.Equal(new[] { 2, 3 }, result.Select(q => q.Id));
        }

        [Fact]
        public async Task GetDebugQuizAsync_FiltersByTopics_CaseInsensitive()
        {
            var service = CreateService(SampleQuestions(), out _);

            // "kapitel 3" (klein) muss auch "Kapitel 3" (Frage 4) matchen
            var result = await service.GetDebugQuizAsync(count: 10, topics: new List<string> { "kapitel 3" });

            Assert.Equal(new[] { 3, 4 }, result.Select(q => q.Id).OrderBy(x => x));
        }

        [Fact]
        public async Task GetDebugQuizAsync_NoTopicsFilter_WhenTopicsListIsEmpty()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 10, topics: new List<string>());

            Assert.Equal(5, result.Count);
        }

        [Fact]
        public async Task GetDebugQuizAsync_LimitsResultToRequestedCount()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 2, category: "ctfl");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetDebugQuizAsync_ZeroCount_ReturnsEmptyList()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 0, category: "ctfl");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDebugQuizAsync_NegativeCount_ReturnsEmptyList_DoesNotThrow()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: -5, category: "ctfl");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDebugQuizAsync_Shuffle_ReturnsSameSetOfIds_DifferentOrderAllowed()
        {
            var service = CreateService(SampleQuestions(), out _);

            var result = await service.GetDebugQuizAsync(count: 5, shuffle: true, category: "ctfl");

            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result.Select(q => q.Id).OrderBy(x => x));
        }

        [Fact]
        public async Task GetDebugQuizAsync_CombinesAllFilters()
        {
            var service = CreateService(SampleQuestions(), out _);

            // KLevel 2 UND Kapitel 3 -> nur Frage 3 (KLevel 2, "kapitel 3")
            var result = await service.GetDebugQuizAsync(
                count: 10,
                minId: 1,
                maxId: 5,
                kLevel: 2,
                topics: new List<string> { "Kapitel 3" });

            Assert.Equal(new[] { 3 }, result.Select(q => q.Id));
        }

        [Fact]
        public async Task GetDebugQuizAsync_LegacyOverload_DefaultsToCtflCategory_NoKLevelOrTopicFilter()
        {
            var service = CreateService(SampleQuestions(), out var uris);

            // Alte 4-Parameter-Überladung ohne kLevel/topics/category.
            // Per Delegate-Cast auf die exakte Signatur, da beide Overloads sonst mehrdeutig wären.
            var legacyOverload = (Func<int, int?, int?, bool, Task<List<Question>>>)service.GetDebugQuizAsync;
            var result = await legacyOverload(10, 2, 4, false);

            Assert.Equal(new[] { 2, 3, 4 }, result.Select(q => q.Id));
            Assert.Equal("http://localhost/api/questions/ctfl", uris.Single());
        }
    }
}
