namespace IstqbQuiz.Shared.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();

        // ✅ Mehrere richtige Antworten
        public List<int> CorrectIndexes { get; set; } = new();

        public string? Diagram { get; set; }
        public List<Dictionary<string, string>>? Table { get; set; }

        // Erklärung
        public string? Explanation { get; set; }

        // Zusätzlicher Text nach Bild/Tabelle
        public string? PostText { get; set; }

        // Optional: Zusätzlicher Text vor Bild/Tabelle
        public string? PreText { get; set; }
    }
}
