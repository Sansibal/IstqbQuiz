using System.Collections.Generic;
using IstqbQuiz.Shared.Models;

namespace IstqbQuiz.Client.Services
{
    public class QuizState
    {
        public List<Question> Questions { get; set; } = new();
        public List<int?> Answers { get; set; } = new();
        public int Score { get; set; }

        // Dictionary: Frage.Id -> gewählter Options-Index
        public Dictionary<int, int> UserAnswers { get; set; } = new Dictionary<int,int>();
    }
}
