// Datei: ISTQB_UnitTest/QuestionsControllerTests.cs
using IstqbQuiz.Server.Controllers;
using IstqbQuiz.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ISTQB_Tests
{
    /// <summary>
    /// Testet QuestionsController: Kategorie-Fallback, 404 bei fehlender Datei,
    /// 500 bei kaputtem JSON, und die Math.Min-Begrenzung bei GetRandomQuestions.
    /// Der Controller liest Dateien direkt vom Dateisystem, daher legen die Tests
    /// echte JSON-Dateien in einem temporären ContentRoot an.
    /// </summary>
    public class QuestionsControllerTests : IDisposable
    {
        private readonly string _tempRoot;

        public QuestionsControllerTests()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), "IstqbQuizTests_" + Guid.NewGuid());
            Directory.CreateDirectory(Path.Combine(_tempRoot, "Data"));
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempRoot))
                Directory.Delete(_tempRoot, recursive: true);
        }

        private QuestionsController CreateController()
        {
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);
            return new QuestionsController(envMock.Object);
        }

        private void WriteQuestionsFile(string fileName, List<Question> questions)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(questions);
            File.WriteAllText(Path.Combine(_tempRoot, "Data", fileName), json);
        }

        private static List<Question> SampleQuestions(int count) =>
            Enumerable.Range(1, count)
                .Select(i => new Question { Id = i, Text = $"Q{i}", Options = new() { "A", "B" }, CorrectIndexes = new() { 0 } })
                .ToList();

        // --- GetAllQuestions ---------------------------------------------------

        [Fact]
        public void GetAllQuestions_Ctfl_ReturnsOkWithQuestionsFromCtflFile()
        {
            WriteQuestionsFile("questions_ctfl.json", SampleQuestions(3));
            var controller = CreateController();

            var result = controller.GetAllQuestions("ctfl");

            var ok = Assert.IsType<OkObjectResult>(result);
            var questions = Assert.IsAssignableFrom<List<Question>>(ok.Value);
            Assert.Equal(3, questions.Count);
        }

        [Fact]
        public void GetAllQuestions_Ctmat_ReturnsQuestionsFromCtmatFile()
        {
            WriteQuestionsFile("questions_ctmat.json", SampleQuestions(2));
            var controller = CreateController();

            var result = controller.GetAllQuestions("ctmat");

            var ok = Assert.IsType<OkObjectResult>(result);
            var questions = Assert.IsAssignableFrom<List<Question>>(ok.Value);
            Assert.Equal(2, questions.Count);
        }

        [Fact]
        public void GetAllQuestions_UnknownCategory_FallsBackToCtflFile()
        {
            WriteQuestionsFile("questions_ctfl.json", SampleQuestions(4));
            var controller = CreateController();

            var result = controller.GetAllQuestions("does-not-exist");

            var ok = Assert.IsType<OkObjectResult>(result);
            var questions = Assert.IsAssignableFrom<List<Question>>(ok.Value);
            Assert.Equal(4, questions.Count);
        }

        [Fact]
        public void GetAllQuestions_CategoryIsCaseInsensitive()
        {
            WriteQuestionsFile("questions_ctmat.json", SampleQuestions(1));
            var controller = CreateController();

            var result = controller.GetAllQuestions("CTMAT");

            var ok = Assert.IsType<OkObjectResult>(result);
            var questions = Assert.IsAssignableFrom<List<Question>>(ok.Value);
            Assert.Single(questions);
        }

        [Fact]
        public void GetAllQuestions_MissingFile_ReturnsNotFound()
        {
            // Keine Datei angelegt
            var controller = CreateController();

            var result = controller.GetAllQuestions("ctfl");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetAllQuestions_MalformedJson_ReturnsStatus500()
        {
            File.WriteAllText(Path.Combine(_tempRoot, "Data", "questions_ctfl.json"), "{ not valid json ]");
            var controller = CreateController();

            var result = controller.GetAllQuestions("ctfl");

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        // --- GetRandomQuestions --------------------------------------------------

        [Fact]
        public void GetRandomQuestions_CountLessThanAvailable_ReturnsExactlyCount()
        {
            WriteQuestionsFile("questions_ctfl.json", SampleQuestions(10));
            var controller = CreateController();

            var result = controller.GetRandomQuestions("ctfl", 4);

            var ok = Assert.IsType<OkObjectResult>(result);
            var questions = Assert.IsAssignableFrom<List<Question>>(ok.Value);
            Assert.Equal(4, questions.Count);
        }

        [Fact]
        public void GetRandomQuestions_CountGreaterThanAvailable_ReturnsAllAvailable()
        {
            WriteQuestionsFile("questions_ctfl.json", SampleQuestions(3));
            var controller = CreateController();

            var result = controller.GetRandomQuestions("ctfl", 40);

            var ok = Assert.IsType<OkObjectResult>(result);
            var questions = Assert.IsAssignableFrom<List<Question>>(ok.Value);
            Assert.Equal(3, questions.Count); // Math.Min(40, 3) == 3, kein Überlauf/Duplikate
        }

        [Fact]
        public void GetRandomQuestions_ReturnsOnlyIdsThatExistInSource_NoDuplicates()
        {
            WriteQuestionsFile("questions_ctfl.json", SampleQuestions(10));
            var controller = CreateController();

            var result = controller.GetRandomQuestions("ctfl", 10);

            var ok = Assert.IsType<OkObjectResult>(result);
            var questions = Assert.IsAssignableFrom<List<Question>>(ok.Value);
            Assert.Equal(Enumerable.Range(1, 10), questions.Select(q => q.Id).OrderBy(x => x));
        }

        [Fact]
        public void GetRandomQuestions_MissingFile_ReturnsNotFound()
        {
            var controller = CreateController();

            var result = controller.GetRandomQuestions("ctfl", 5);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
