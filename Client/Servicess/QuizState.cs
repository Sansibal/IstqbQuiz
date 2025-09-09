using System.Collections.Generic;

namespace IstqbQuiz.Client.Services
{
    public class QuizState
    {
        // Fragen wie sie aus JSON geladen werden
        public List<QuestionDto> Questions { get; set; } = new();
        public List<int?> Answers { get; set; } = new();
        public int Score { get; set; }

        public void Clear()
        {
            Questions.Clear();
            Answers.Clear();
            Score = 0;
        }
    }

    // DTO passend zur questions.json Struktur
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectIndex { get; set; }
        public string? Explanation { get; set; } // optional
        public string? Diagram { get; set; } // optional
        public List<Dictionary<string,string>>? Table { get; set; } // optional
        public string? PostText { get; set; } // optional
    }
}
