using IstqbQuiz.Shared.Models;
using IstqbQuiz.Shared.Services;
using Xunit;
using System.Collections.Generic;

namespace ISTQB_Tests
{
    public class QuestionTests
    {
        [Fact]
        public void Can_Set_And_Get_All_Properties()
        {
            // Arrange
            var q = new Question();

            // Act
            q.Id = 42;
            q.Text = "Beispiel-Frage?";
            q.Options = new List<string> { "A", "B", "C" };
            q.CorrectIndexes = new List<int> { 1, 2 };
            q.Diagram = "diagram.png";
            q.Table = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "Key", "Value" } }
            };
            q.Explanation = "Weil das korrekt ist.";
            q.Explanation2 = "Alternative Erklärung.";
            q.PostText = "Nach der Frage.";
            q.PreText = "Vor der Frage.";
            q.KLevel = 2;
            q.Topic = "Grundlagen";

            // Assert
            Assert.Equal(42, q.Id);
            Assert.Equal("Beispiel-Frage?", q.Text);
            Assert.Equal(new List<string> { "A", "B", "C" }, q.Options);
            Assert.Equal(new List<int> { 1, 2 }, q.CorrectIndexes);
            Assert.Equal("diagram.png", q.Diagram);
            Assert.Single(q.Table);
            Assert.Equal("Weil das korrekt ist.", q.Explanation);
            Assert.Equal("Alternative Erklärung.", q.Explanation2);
            Assert.Equal("Nach der Frage.", q.PostText);
            Assert.Equal("Vor der Frage.", q.PreText);
            Assert.Equal(2, q.KLevel);
            Assert.Equal("Grundlagen", q.Topic);
        }
    }
}
