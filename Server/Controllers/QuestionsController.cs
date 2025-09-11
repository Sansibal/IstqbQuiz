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

        [HttpGet]
        public IActionResult GetAllQuestions()
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Data", "questions.json");

            if (!System.IO.File.Exists(filePath))
                return NotFound($"questions.json not found! Path: {filePath}");

            try
            {
                var json = System.IO.File.ReadAllText(filePath);
                var questions = JsonSerializer.Deserialize<List<Question>>(json);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deserializing questions.json: {ex.Message}");
            }
        }

        [HttpGet("random/{count}")]
        public IActionResult GetRandomQuestions(int count = 40)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Data", "questions.json");

            if (!System.IO.File.Exists(filePath))
                return NotFound($"questions.json not found! Path: {filePath}");

            try
            {
                var json = System.IO.File.ReadAllText(filePath);
                var questions = JsonSerializer.Deserialize<List<Question>>(json) ?? new();

                var rnd = new Random();
                var randomSet = questions.OrderBy(_ => rnd.Next()).Take(Math.Min(count, questions.Count)).ToList();

                return Ok(randomSet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading questions.json: {ex.Message}");
            }
        }
    }
}
