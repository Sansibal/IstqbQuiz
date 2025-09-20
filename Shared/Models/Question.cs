namespace IstqbQuiz.Shared.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();

        // ✅ Mehrere richtige Antworten (max 2 in deinem Fall)
        public List<int> CorrectIndexes { get; set; } = new();

        public string? Diagram { get; set; }
        public List<Dictionary<string, string>>? Table { get; set; }

        // Erklärung für die erste und zweite richtige Antwort
        public string? Explanation { get; set; }
        public string? Explanation2 { get; set; }

        public string? PostText { get; set; }
        public string? PreText { get; set; }

        // Neue Felder für Filter
        public int KLevel { get; set; }                      // K1, K2 oder K3
        public string Topic { get; set; } = "";              // Optionell: Thema / Kapitel
    }
}
