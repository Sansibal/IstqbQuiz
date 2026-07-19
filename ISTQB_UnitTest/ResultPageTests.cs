// Datei: ISTQB_UnitTest/ResultPageTests.cs
using Bunit;
using IstqbQuiz.Client.Pages;
using IstqbQuiz.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ISTQB_Tests
{
    /// <summary>
    /// bUnit-Komponententests für Result.razor: Bestehens-Logik je Kategorie,
    /// Advanced/CTMAT-Hinweise und Navigation über "Neues Quiz" / "Details".
    /// </summary>
    public class ResultPageTests : BunitContext
    {
        private QuizState SetUpState(int score, int totalQuestions, string category = "ctfl", bool advanced = false)
        {
            var state = new QuizState
            {
                Score = score,
                Questions = Enumerable.Range(1, totalQuestions).Select(i => new QuestionDto { Id = i }).ToList(),
                Category = category,
                IsAdvancedQuiz = advanced
            };
            Services.AddSingleton(state);
            return state;
        }

        [Fact]
        public void NoQuestions_ShowsNoDataMessage()
        {
            Services.AddSingleton(new QuizState());

            var cut = Render<Result>();

            Assert.Contains("Keine Daten gefunden", cut.Markup);
        }

        [Fact]
        public void Ctfl_ScoreAtPassThreshold_ShowsPassed()
        {
            SetUpState(score: 26, totalQuestions: 40, category: "ctfl");

            var cut = Render<Result>();

            Assert.Contains("bestanden", cut.Markup);
            Assert.DoesNotContain("Nicht bestanden", cut.Markup);
        }

        [Fact]
        public void Ctfl_ScoreBelowPassThreshold_ShowsFailed()
        {
            SetUpState(score: 25, totalQuestions: 40, category: "ctfl");

            var cut = Render<Result>();

            Assert.Contains("Nicht bestanden", cut.Markup);
        }

        [Fact]
        public void Ctmat_DoesNotShowPassFailBadge()
        {
            SetUpState(score: 10, totalQuestions: 40, category: "ctmat");

            var cut = Render<Result>();

            Assert.Contains("CT-MAT Quiz", cut.Markup);
            Assert.DoesNotContain("Nicht bestanden", cut.Markup);
            Assert.DoesNotContain("bestanden.", cut.Markup);
        }

        [Fact]
        public void AdvancedQuiz_ShowsAdvancedHint_RegardlessOfCategory()
        {
            SetUpState(score: 5, totalQuestions: 10, category: "ctfl", advanced: true);

            var cut = Render<Result>();

            Assert.Contains("Advanced-Quiz", cut.Markup);
        }

        [Fact]
        public void StartNew_NavigatesToQuiz_ForNormalCtflQuiz()
        {
            SetUpState(score: 1, totalQuestions: 1, category: "ctfl", advanced: false);
            var cut = Render<Result>();

            cut.Find("button.btn-primary").Click();

            var nav = Services.GetRequiredService<NavigationManager>();
            Assert.EndsWith("/quiz", nav.Uri);
        }

        [Fact]
        public void StartNew_NavigatesToQuizCtmat_ForCtmatCategory()
        {
            SetUpState(score: 1, totalQuestions: 1, category: "ctmat", advanced: false);
            var cut = Render<Result>();

            cut.Find("button.btn-primary").Click();

            var nav = Services.GetRequiredService<NavigationManager>();
            Assert.EndsWith("/quizctmat", nav.Uri);
        }

        [Fact]
        public void StartNew_NavigatesToQuizAdvanced_ForAdvancedQuiz()
        {
            SetUpState(score: 1, totalQuestions: 1, category: "ctfl", advanced: true);
            var cut = Render<Result>();

            cut.Find("button.btn-primary").Click();

            var nav = Services.GetRequiredService<NavigationManager>();
            Assert.EndsWith("/quizadvanced", nav.Uri);
        }

        [Fact]
        public void ShowDetails_NavigatesToDetailsPage()
        {
            SetUpState(score: 1, totalQuestions: 1, category: "ctfl");
            var cut = Render<Result>();

            cut.Find("button.btn-secondary").Click();

            var nav = Services.GetRequiredService<NavigationManager>();
            Assert.EndsWith("/details", nav.Uri);
        }
    }
}
