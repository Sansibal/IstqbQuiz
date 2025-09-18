using IstqbQuiz.Shared.Utils;   // Zugriff auf QuizHelper
using Xunit;
using System.Collections.Generic;

namespace ISTQB_Tests
{
    public class QuizHelper_IsCorrect_Tests
    {
        [Fact]
        public void IsCorrect_ReturnsTrue_WhenSingleAnswerMatches()
        {
            var correct = new List<int> { 2 };
            var given = new List<int> { 2 };

            Assert.True(QuizHelper.IsCorrect(correct, given));
        }

        [Fact]
        public void IsCorrect_ReturnsFalse_WhenSingleAnswerDoesNotMatch()
        {
            var correct = new List<int> { 2 };
            var given = new List<int> { 1 };

            Assert.False(QuizHelper.IsCorrect(correct, given));
        }

        [Fact]
        public void IsCorrect_ReturnsTrue_WhenTwoAnswersMatchExactly()
        {
            var correct = new List<int> { 1, 3 };
            var given = new List<int> { 3, 1 }; // Reihenfolge egal

            Assert.True(QuizHelper.IsCorrect(correct, given));
        }

        [Fact]
        public void IsCorrect_ReturnsFalse_WhenOnlyOneOfTwoAnswersGiven()
        {
            var correct = new List<int> { 1, 3 };
            var given = new List<int> { 1 };

            Assert.False(QuizHelper.IsCorrect(correct, given));
        }

        [Fact]
        public void IsCorrect_ReturnsFalse_WhenExtraAnswerIsGiven()
        {
            var correct = new List<int> { 1, 3 };
            var given = new List<int> { 1, 3, 4 }; // enthält mehr als erlaubt

            Assert.False(QuizHelper.IsCorrect(correct, given));
        }

        [Fact]
        public void IsCorrect_ReturnsFalse_WhenGivenIsEmpty()
        {
            var correct = new List<int> { 1 };
            var given = new List<int>();

            Assert.False(QuizHelper.IsCorrect(correct, given));
        }

        [Fact]
        public void IsCorrect_ReturnsFalse_WhenCorrectIsEmpty()
        {
            var correct = new List<int>();
            var given = new List<int> { 1 };

            Assert.False(QuizHelper.IsCorrect(correct, given));
        }

        [Fact]
        public void IsCorrect_ReturnsTrue_WhenBothEmpty()
        {
            var correct = new List<int>();
            var given = new List<int>();

            // beide leer -> gilt als korrekt
            Assert.True(QuizHelper.IsCorrect(correct, given));
        }
    }
}
