using IstqbQuiz.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IstqbQuiz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetQuestions()
        {
            // Use AppContext.BaseDirectory to get the correct path to the output folder.
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "questions.json");
            
            if (!System.IO.File.Exists(filePath))
                return NotFound("questions.json not found! Path: " + filePath);

            try
            {
                var json = System.IO.File.ReadAllText(filePath);
                var questions = JsonSerializer.Deserialize<List<Question>>(json);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                // Return an error with details to help debug the JSON deserialization.
                return StatusCode(500, $"An error occurred while deserializing the questions.json file: {ex.Message}");
            }
        }
    }
}
