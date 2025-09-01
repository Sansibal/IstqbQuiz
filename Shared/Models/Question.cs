namespace IstqbQuiz.Shared.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectIndex { get; set; }
        public string? Explanation { get; set; }

        // Bild (Diagramm) zur Frage
        public string? Diagram { get; set; }

        // Tabelle zur Frage (Schlüssel = Spaltenname, Wert = Zelleninhalt)
        public List<Dictionary<string, string>> Table { get; set; } = new();

        // Zusätzlicher Text nach Bild/Tabelle
        public string? PostText { get; set; }

        // Optional: Zusätzlicher Text vor Bild/Tabelle
        public string? PreText { get; set; }
    }
}