using IstqbQuiz.Shared.Utils;   // Zugriff auf QuizHelper
using IstqbQuiz.Shared.Models;
using Xunit;
using System.Collections.Generic;

namespace ISTQB_Tests
{
    /// <summary>
    /// Tests für die Score-Methode von QuizHelper.
    /// Diese Methode summiert Punkte über alle Fragen hinweg (jede Frage max. 1 Punkt).
    /// </summary>
    public class QuizHelper_Score_Tests
    {
        [Fact]
        public void Score_Returns0_WhenQuestionsListIsEmpty()
        {
            // Arrange
            var questions = new List<Question>();
            var answers = new List<IReadOnlyList<int>>();

            // Act
            var result = QuizHelper.Score(questions, answers);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Score_Returns0_WhenAnswersListIsEmpty()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1 } }
            };
            var answers = new List<IReadOnlyList<int>>();

            var result = QuizHelper.Score(questions, answers);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Score_CountsOnlyMatchingAnswers_WhenCountsDiffer()
        {
            // Fragen = 2, Antworten = 1
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1 } },
                new Question { CorrectIndexes = new List<int> { 2 } }
            };

            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 1 } // nur 1 Antwort für die erste Frage
            };

            var result = QuizHelper.Score(questions, answers);

            // Erwartung: Nur die erste Frage wird bewertet -> 1 Punkt
            Assert.Equal(1, result);
        }

        [Fact]
        public void Score_SumsCorrectly_ForMultipleQuestions()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1 } },
                new Question { CorrectIndexes = new List<int> { 2 } },
                new Question { CorrectIndexes = new List<int> { 3 } }
            };

            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 1 }, // richtig
                new List<int> { 1 }, // falsch
                new List<int> { 3 }  // richtig
            };

            var result = QuizHelper.Score(questions, answers);

            // Erwartung: 2 von 3 Fragen richtig
            Assert.Equal(2, result);
        }

        [Fact]
        public void Score_Returns0_WhenAllAnswersWrong()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1 } },
                new Question { CorrectIndexes = new List<int> { 2 } }
            };

            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 3 },
                new List<int> { 0 }
            };

            var result = QuizHelper.Score(questions, answers);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Score_ReturnsFullPoints_WhenAllCorrect()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1 } },
                new Question { CorrectIndexes = new List<int> { 2 } }
            };

            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 1 },
                new List<int> { 2 }
            };

            var result = QuizHelper.Score(questions, answers);

            Assert.Equal(2, result);
        }
    }
}
