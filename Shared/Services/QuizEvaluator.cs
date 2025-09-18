// Datei: Shared/Services/QuizEvaluator.cs
using System.Collections.Generic;
using System.Linq;
using IstqbQuiz.Shared.Models;

namespace IstqbQuiz.Shared.Services
{
    /// <summary>
    /// Bewertet Antworten und berechnet Punktzahlen für das Quiz.
    /// </summary>
    public static class QuizEvaluator
    {
        /// <summary>
        /// Eine Frage gilt als richtig, wenn die gewählten Indizes exakt den korrekten entsprechen
        /// (keine zu wenig, keine zu viel; Reihenfolge egal).
        /// </summary>
        public static bool IsCorrect(IReadOnlyList<int> correct, IReadOnlyList<int> given)
        {
            if (correct is null || given is null) return false;
            if (correct.Count != given.Count) return false;

            return correct.OrderBy(x => x).SequenceEqual(given.OrderBy(x => x));
        }

        /// <summary>
        /// Summiert die Punkte über alle Fragen (jede Frage max. 1 Punkt).
        /// </summary>
        public static int Score(IReadOnlyList<Question> questions, IReadOnlyList<IReadOnlyList<int>> answers)
        {
            if (questions is null || answers is null) return 0;

            var count = System.Math.Min(questions.Count, answers.Count);
            var score = 0;

            for (int i = 0; i < count; i++)
            {
                var q = questions[i];
                var a = answers[i] ?? new List<int>();
                if (IsCorrect(q.CorrectIndexes, a.ToList()))
                    score++;
            }

            return score;
        }
    }
}
