// Datei: ISTQB_UnitTest/DetailsPageTests.cs
using Bunit;
using IstqbQuiz.Client.Pages;
using IstqbQuiz.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ISTQB_Tests
{
    /// <summary>
    /// bUnit-Komponententests für Details.razor: Richtig/Falsch-Badges,
    /// Auf-/Zuklappen und den "nur falsche Fragen"-Filter.
    /// </summary>
    public class DetailsPageTests : BunitContext
    {
        private QuizState SetUpState()
        {
            var state = new QuizState
            {
                Questions = new List<QuestionDto>
                {
                    new QuestionDto { Id = 1, Text = "Frage 1", Options = new() { "A", "B" }, CorrectIndexes = new() { 0 } },
                    new QuestionDto { Id = 2, Text = "Frage 2", Options = new() { "A", "B" }, CorrectIndexes = new() { 1 } },
                },
                Answers = new List<List<int>>
                {
                    new() { 0 }, // Frage 1: richtig beantwortet
                    new() { 0 }, // Frage 2: falsch beantwortet (korrekt wäre 1)
                }
            };
            Services.AddSingleton(state);
            return state;
        }

        [Fact]
        public void NoQuestions_ShowsNoDataMessage()
        {
            Services.AddSingleton(new QuizState());

            var cut = Render<Details>();

            Assert.Contains("Keine Daten vorhanden", cut.Markup);
        }

        [Fact]
        public void ShowsCorrectAndWrongBadges_ForEachQuestion()
        {
            SetUpState();

            var cut = Render<Details>();

            Assert.Contains("Frage 1 / 2", cut.Markup);
            Assert.Contains("Frage 2 / 2", cut.Markup);
            // Eine Karte "Richtig", eine "Falsch"
            Assert.Contains("Richtig", cut.Markup);
            Assert.Contains("Falsch", cut.Markup);
        }

        [Fact]
        public void ExpandAll_ShowsQuestionTextForAllCards()
        {
            SetUpState();
            var cut = Render<Details>();

            cut.FindAll("button").Single(b => b.TextContent.Contains("Alles aufklappen")).Click();

            Assert.Contains("Frage 1", cut.Markup);
            Assert.Contains("Frage 2", cut.Markup);
        }

        [Fact]
        public void CollapseAll_HidesQuestionBody()
        {
            SetUpState();
            var cut = Render<Details>();

            cut.FindAll("button").Single(b => b.TextContent.Contains("Alles aufklappen")).Click();
            cut.FindAll("button").Single(b => b.TextContent.Contains("Alles zuklappen")).Click();

            // Kopfzeilen bleiben sichtbar, aber Fragetext (card-body Inhalt) verschwindet
            Assert.DoesNotContain("Deine Antwort(en)", cut.Markup);
        }

        [Fact]
        public void ToggleWrongOnly_HidesCorrectlyAnsweredQuestions()
        {
            SetUpState();
            var cut = Render<Details>();

            cut.FindAll("button").Single(b => b.TextContent.Contains("Nur falsche Fragen anzeigen")).Click();

            Assert.DoesNotContain("Frage 1 / 2", cut.Markup); // richtig beantwortet -> ausgeblendet
            Assert.Contains("Frage 2 / 2", cut.Markup);       // falsch beantwortet -> sichtbar
        }

        [Fact]
        public void ToggleWrongOnly_TwiceRestoresAllQuestions()
        {
            SetUpState();
            var cut = Render<Details>();

            var toggle = () => cut.FindAll("button").Single(b =>
                b.TextContent.Contains("Nur falsche Fragen anzeigen") || b.TextContent.Contains("Alle Fragen anzeigen"));

            toggle().Click();
            toggle().Click();

            Assert.Contains("Frage 1 / 2", cut.Markup);
            Assert.Contains("Frage 2 / 2", cut.Markup);
        }
    }
}
