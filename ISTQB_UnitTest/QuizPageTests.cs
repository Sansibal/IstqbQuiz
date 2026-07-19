// Datei: ISTQB_UnitTest/QuizPageTests.cs
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
    /// bUnit-Komponententests für den Quiz-Ablauf (Quiz.razor): Start, Antwort-Toggle,
    /// Weiter/Zurück-Sperrlogik und Navigation zu "result" nach der letzten Frage.
    /// </summary>
    public class QuizPageTests : BunitContext
    {
        private static List<Question> TwoQuestions() => new()
        {
            new Question { Id = 1, Text = "Frage eins?", Options = new() { "A1", "B1" }, CorrectIndexes = new() { 0 } },
            new Question { Id = 2, Text = "Frage zwei?", Options = new() { "A2", "B2" }, CorrectIndexes = new() { 1 } },
        };

        private QuestionService RegisterQuestionService(List<Question> questions)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(questions) });

            var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
            var service = new QuestionService(httpClient);
            Services.AddSingleton(service);
            return service;
        }

        [Fact]
        public void InitialRender_ShowsStartButton()
        {
            Services.AddSingleton(new QuizState());
            RegisterQuestionService(TwoQuestions());

            var cut = Render<Quiz>();

            Assert.Contains("Start (40 Fragen)", cut.Markup);
        }

        [Fact]
        public void StartQuiz_LoadsQuestions_AndShowsFirstQuestion()
        {
            Services.AddSingleton(new QuizState());
            RegisterQuestionService(TwoQuestions());

            var cut = Render<Quiz>();
            cut.Find("button").Click(); // Start-Button

            Assert.Contains("Frage 1 / 2", cut.Markup);
            Assert.Contains("Frage eins?", cut.Markup);
        }

        [Fact]
        public void WeiterButton_IsDisabled_UntilAnswerSelected()
        {
            Services.AddSingleton(new QuizState());
            RegisterQuestionService(TwoQuestions());

            var cut = Render<Quiz>();
            cut.Find("button").Click(); // Start

            var weiterButton = cut.FindAll("button").Single(b => b.TextContent.Contains("Weiter"));
            Assert.True(weiterButton.HasAttribute("disabled"));

            // Erste Antwortoption anklicken
            cut.FindAll("button").First(b => b.TextContent.Contains("A1")).Click();

            weiterButton = cut.FindAll("button").Single(b => b.TextContent.Contains("Weiter"));
            Assert.False(weiterButton.HasAttribute("disabled"));
        }

        [Fact]
        public void ZurueckButton_IsDisabled_OnFirstQuestion_EnabledAfterAdvancing()
        {
            Services.AddSingleton(new QuizState());
            RegisterQuestionService(TwoQuestions());

            var cut = Render<Quiz>();
            cut.Find("button").Click(); // Start

            var zurueck = cut.FindAll("button").Single(b => b.TextContent.Contains("Zurück"));
            Assert.True(zurueck.HasAttribute("disabled"));

            cut.FindAll("button").First(b => b.TextContent.Contains("A1")).Click();
            cut.FindAll("button").Single(b => b.TextContent.Contains("Weiter")).Click();

            zurueck = cut.FindAll("button").Single(b => b.TextContent.Contains("Zurück"));
            Assert.False(zurueck.HasAttribute("disabled"));
        }

        [Fact]
        public void AnsweringAllQuestions_ComputesScore_AndNavigatesToResult()
        {
            var state = new QuizState();
            Services.AddSingleton(state);
            RegisterQuestionService(TwoQuestions());

            var cut = Render<Quiz>();
            cut.Find("button").Click(); // Start

            // Frage 1: richtige Antwort A1 (Index 0)
            cut.FindAll("button").First(b => b.TextContent.Contains("A1")).Click();
            cut.FindAll("button").Single(b => b.TextContent.Contains("Weiter")).Click();

            // Frage 2: falsche Antwort A2 (Index 0), korrekt wäre B2 (Index 1)
            cut.FindAll("button").First(b => b.TextContent.Contains("A2")).Click();
            cut.FindAll("button").Single(b => b.TextContent.Contains("Weiter")).Click();

            Assert.Equal(1, state.Score);
            Assert.EndsWith("/result", this.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>().Uri, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EmptyQuestionList_ShowsCtflSpecificWarning_NotGenericError()
        {
            Services.AddSingleton(new QuizState());
            RegisterQuestionService(new List<Question>());

            var cut = Render<Quiz>();
            cut.Find("button").Click(); // Start

            Assert.Contains("questions.json", cut.Markup);
            Assert.DoesNotContain("Ein Fehler ist aufgetreten", cut.Markup);
        }
    }
}
