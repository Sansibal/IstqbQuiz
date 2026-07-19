// Datei: ISTQB_UnitTest/QuizStateTests.cs
using IstqbQuiz.Client.Services;
using Xunit;

namespace ISTQB_Tests
{
    public class QuizStateTests
    {
        [Fact]
        public void Clear_ResetsQuestionsAnswersAndScore()
        {
            var state = new QuizState
            {
                Questions = new List<QuestionDto> { new QuestionDto { Id = 1 } },
                Answers = new List<List<int>> { new List<int> { 0 } },
                Score = 5
            };

            state.Clear();

            Assert.Empty(state.Questions);
            Assert.Empty(state.Answers);
            Assert.Equal(0, state.Score);
        }

        [Fact]
        public void Clear_ResetsDebugAndAdvancedFlags()
        {
            var state = new QuizState
            {
                IsDebugQuiz = true,
                IsAdvancedQuiz = true
            };

            state.Clear();

            Assert.False(state.IsDebugQuiz);
            Assert.False(state.IsAdvancedQuiz);
        }

        [Fact]
        public void Clear_ResetsCategoryToCtfl()
        {
            var state = new QuizState { Category = "ctmat" };

            state.Clear();

            Assert.Equal("ctfl", state.Category);
        }

        [Fact]
        public void NewQuizState_HasExpectedDefaults()
        {
            var state = new QuizState();

            Assert.Empty(state.Questions);
            Assert.Empty(state.Answers);
            Assert.Equal(0, state.Score);
            Assert.False(state.IsDebugQuiz);
            Assert.False(state.IsAdvancedQuiz);
            Assert.Equal("ctfl", state.Category);
        }
    }
}
