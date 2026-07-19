// Datei: IstqbQuiz.Tests/QuizEvaluatorTests.cs
using System.Collections.Generic;
using IstqbQuiz.Shared.Models;
using IstqbQuiz.Shared.Services;
using Xunit;

namespace IstqbQuiz.Tests
{
    public class QuizEvaluatorTests
    {
        // --- IsCorrect -------------------------------------------------------

        [Fact]
        public void IsCorrect_SingleChoice_Correct_ReturnsTrue()
        {
            // Arrange
            var correct = new List<int> { 2 };
            var given = new List<int> { 2 };

            // Act
            var result = QuizEvaluator.IsCorrect(correct, given);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCorrect_SingleChoice_Wrong_ReturnsFalse()
        {
            var correct = new List<int> { 2 };
            var given = new List<int> { 1 };

            var result = QuizEvaluator.IsCorrect(correct, given);

            Assert.False(result);
        }

        [Fact]
        public void IsCorrect_Multi_BothCorrect_OrderIrrelevant_ReturnsTrue()
        {
            var correct = new List<int> { 1, 3 };
            var given = new List<int> { 3, 1 };

            var result = QuizEvaluator.IsCorrect(correct, given);

            Assert.True(result);
        }

        [Fact]
        public void IsCorrect_Multi_MissingOne_ReturnsFalse()
        {
            var correct = new List<int> { 0, 2 };
            var given = new List<int> { 2 };

            var result = QuizEvaluator.IsCorrect(correct, given);

            Assert.False(result);
        }

        [Fact]
        public void IsCorrect_Multi_ExtraWrong_ReturnsFalse()
        {
            var correct = new List<int> { 0, 2 };
            var given = new List<int> { 0, 2, 3 };

            var result = QuizEvaluator.IsCorrect(correct, given);

            Assert.False(result);
        }

        // --- Score -----------------------------------------------------------

        [Fact]
        public void Score_MixedSingleAndMulti_ComputesSumCorrectly()
        {
            // Arrange: 3 Fragen, davon 2 richtig
            var questions = new List<Question>
            {
                new Question { Id = 1, CorrectIndexes = new List<int>{2} },
                new Question { Id = 2, CorrectIndexes = new List<int>{1,3} },
                new Question { Id = 3, CorrectIndexes = new List<int>{0} }
            };

            var answers = new List<IReadOnlyList<int>>
            {
                new List<int>{2},      // richtig
                new List<int>{3,1},    // richtig (Reihenfolge egal)
                new List<int>{1}       // falsch
            };

            // Act
            var score = QuizEvaluator.Score(questions, answers);

            // Assert
            Assert.Equal(2, score);
        }

        [Fact]
        public void Score_Returns0_WhenQuestionsListIsNull()
        {
            var score = QuizEvaluator.Score(null!, new List<IReadOnlyList<int>>());

            Assert.Equal(0, score);
        }

        [Fact]
        public void Score_Returns0_WhenAnswersListIsNull()
        {
            var questions = new List<Question> { new Question { CorrectIndexes = new List<int> { 1 } } };

            var score = QuizEvaluator.Score(questions, null!);

            Assert.Equal(0, score);
        }

        [Fact]
        public void IsCorrect_ReturnsFalse_WhenCorrectIsNull()
        {
            Assert.False(QuizEvaluator.IsCorrect(null!, new List<int> { 1 }));
        }

        [Fact]
        public void IsCorrect_ReturnsFalse_WhenGivenIsNull()
        {
            Assert.False(QuizEvaluator.IsCorrect(new List<int> { 1 }, null!));
        }
    }
}
