using IstqbQuiz.Shared.Utils;   // Zugriff auf QuizHelper
using IstqbQuiz.Shared.Models; // Zugriff auf Question
using Xunit;
using System.Collections.Generic;

namespace ISTQB_Tests
{
    public class QuizHelperTests
    {
        [Fact]
        public void Score_Returns1_WhenSingleAnswerIsCorrect()
        {
            // Arrange
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1 } }
            };
            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 1 }
            };

            // Act
            var score = QuizHelper.Score(questions, answers);

            // Assert
            Assert.Equal(1, score);
        }

        [Fact]
        public void Score_Returns0_WhenAnswerIsWrong()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 2 } }
            };
            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 1 }
            };

            var score = QuizHelper.Score(questions, answers);

            Assert.Equal(0, score);
        }

        [Fact]
        public void Score_Returns1_WhenTwoAnswersMatchExactly()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1, 3 } }
            };
            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 1, 3 }
            };

            var score = QuizHelper.Score(questions, answers);

            Assert.Equal(1, score);
        }

        [Fact]
        public void Score_Returns0_WhenTwoAnswersArePartial()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1, 3 } }
            };
            var answers = new List<IReadOnlyList<int>>
            {
                new List<int> { 1 }
            };

            var score = QuizHelper.Score(questions, answers);

            Assert.Equal(0, score);
        }

        [Fact]
        public void Score_Returns0_WhenAnswersListIsEmpty()
        {
            var questions = new List<Question>
            {
                new Question { CorrectIndexes = new List<int> { 1 } }
            };
            var answers = new List<IReadOnlyList<int>>
            {
                new List<int>() // keine Antwort
            };

            var score = QuizHelper.Score(questions, answers);

            Assert.Equal(0, score);
        }

        [Fact]
        public void Score_Returns0_WhenQuestionsListIsEmpty()
        {
            var questions = new List<Question>();
            var answers = new List<IReadOnlyList<int>>();

            var score = QuizHelper.Score(questions, answers);

            Assert.Equal(0, score);
        }
    }
}
