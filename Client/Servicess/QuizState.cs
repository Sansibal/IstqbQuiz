using System.Collections.Generic;

namespace IstqbQuiz.Client.Services
{
    public class QuizState
    {
        // Liste der Fragen, wie sie aus der JSON geladen werden
        public List<QuestionDto> Questions { get; set; } = new();

        // Statt nur eine Antwort pro Frage (int?), speichern wir mehrere (Liste von Indexen)
        public List<List<int>> Answers { get; set; } = new();

        // Gesamtergebnis / Punkte
        public int Score { get; set; }

        public bool IsDebugQuiz { get; set; } = false;


        // Setzt den gesamten Zustand zurück
        public void Clear()
        {
            Questions.Clear();
            Answers.Clear();
            Score = 0;
        }
    }

    // DTO passend zur Struktur in questions.json
    public class QuestionDto
    {
        public int Id { get; set; }

        // Der eigentliche Fragetext
        public string Text { get; set; } = string.Empty;

        // Liste der Antwortoptionen (z. B. A, B, C, D)
        public List<string> Options { get; set; } = new();

        // ✅ Eine oder mehrere richtige Antworten (maximal 2 beim ISTQB)
        public List<int> CorrectIndexes { get; set; } = new();

        // ✅ Erklärungen für richtige Antworten (maximal 2)
        public string? Explanation { get; set; }
        public string? Explanation2 { get; set; }

        // Optional: Diagrammbild zu einer Frage
        public string? Diagram { get; set; }

        // Optional: Tabelle für bestimmte Fragen
        public List<Dictionary<string, string>>? Table { get; set; }

        // Optional: Zusätzlicher Text nach Bild oder Tabelle
        public string? PostText { get; set; }

        // Optional: Zusätzlicher Text vor Bild oder Tabelle
        public string? PreText { get; set; }
    }
}
