// Datei: ISTQB_UnitTest/QuizCtmatPageTests.cs
using System.Net;
using System.Net.Http.Json;
using Bunit;
using IstqbQuiz.Client.Pages;
using IstqbQuiz.Client.Services;
using IstqbQuiz.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Xunit;

namespace ISTQB_Tests
{
    /// <summary>
    /// Stellt sicher, dass der QuizCtmat-Wrapper die gemeinsame QuizPage-Komponente
    /// korrekt mit der Kategorie "ctmat" parametrisiert (Titel, angefragte API-Kategorie,
    /// State.Category). Ergänzt die ausführlicheren QuizPageTests (die die geteilte
    /// Logik über den CTFL-Wrapper Quiz.razor abdecken).
    /// </summary>
    public class QuizCtmatPageTests : BunitContext
    {
        private static List<Question> OneQuestion() => new()
        {
            new Question { Id = 1, Text = "CT-MAT Frage?", Options = new() { "A", "B" }, CorrectIndexes = new() { 0 } },
        };

        private QuizState RegisterServices()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(OneQuestion()) });

            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
            Services.AddSingleton(new QuestionService(httpClient));

            var state = new QuizState();
            Services.AddSingleton(state);
            return state;
        }

        [Fact]
        public void InitialRender_ShowsCtmatTitleAndStartButton()
        {
            RegisterServices();

            var cut = Render<QuizCtmat>();

            Assert.Contains("ISTQB CT-MAT Quiz", cut.Markup);
            Assert.Contains("Start (40 Fragen)", cut.Markup);
        }

        [Fact]
        public void StartQuiz_SetsCategoryToCtmat_OnState()
        {
            var state = RegisterServices();

            var cut = Render<QuizCtmat>();
            cut.Find("button").Click(); // Start

            Assert.Equal("ctmat", state.Category);
            Assert.Contains("CT-MAT Frage?", cut.Markup);
        }

        [Fact]
        public void EmptyQuestionList_ShowsCtmatSpecificWarning()
        {
            // Leere Fragenliste vom Server -> State.Questions bleibt leer, kein errorMessage
            // -> der spezifische NoQuestionsWarning-Text der QuizCtmat-Seite wird angezeigt.
            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new List<Question>()) });

            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
            Services.AddSingleton(new QuestionService(httpClient));
            Services.AddSingleton(new QuizState());

            var cut = Render<QuizCtmat>();
            cut.Find("button").Click(); // Start

            Assert.Contains("questions_ctmat.json", cut.Markup);
            Assert.DoesNotContain("Ein Fehler ist aufgetreten", cut.Markup);
        }
    }
}
