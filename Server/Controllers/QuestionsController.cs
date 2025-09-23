using IstqbQuiz.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IstqbQuiz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public QuestionsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ============================================
        // Hilfsfunktion: Lädt Fragen aus einer JSON-Datei
        // - category = ctfl, ctmat, ...
        // - Standard: ctfl
        // ============================================
        private List<Question> LoadQuestions(string category = "ctfl")
        {
            var fileName = category.ToLower() switch
            {
                "ctfl" => "questions_ctfl.json",
                "ctmat" => "questions_ctmat.json",
                _ => "questions_ctfl.json" // Fallback
            };

            var filePath = Path.Combine(_env.ContentRootPath, "Data", fileName);

            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException($"Datei {fileName} nicht gefunden! Pfad: {filePath}");

            var json = System.IO.File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Question>>(json) ?? new();
        }

        // ============================================
        // GET api/questions/{category?}
        // -> Lädt ALLE Fragen für die Kategorie
        // -> Standard: ctfl
        // ============================================
        [HttpGet("{category?}")]
        public IActionResult GetAllQuestions(string category = "ctfl")
        {
            try
            {
                var questions = LoadQuestions(category);
                return Ok(questions);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fehler beim Laden der Fragen ({category}): {ex.Message}");
            }
        }

        // ============================================
        // GET api/questions/{category}/random/{count}
        // -> Lädt ZUFÄLLIGE Fragen für die Kategorie
        // ============================================
        [HttpGet("{category}/random/{count}")]
        public IActionResult GetRandomQuestions(string category, int count = 40)
        {
            try
            {
                var questions = LoadQuestions(category);

                var rnd = new Random();
                var randomSet = questions.OrderBy(_ => rnd.Next())
                                         .Take(Math.Min(count, questions.Count))
                                         .ToList();

                return Ok(randomSet);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fehler beim Lesen der Fragen ({category}): {ex.Message}");
            }
        }
    }
}
