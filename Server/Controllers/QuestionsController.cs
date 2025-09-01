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
        public IActionResult GetQuestions()
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Data", "questions.json");
            if (!System.IO.File.Exists(filePath))
                return NotFound("questions.json not found!");

            var json = System.IO.File.ReadAllText(filePath);
            var questions = JsonSerializer.Deserialize<List<Question>>(json);
            return Ok(questions);
        }
    }
}