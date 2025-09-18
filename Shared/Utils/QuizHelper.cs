// Datei: IstqbQuiz.Shared/Utils/QuizHelper.cs
using System;
using System.Collections.Generic;
using System.Linq;
using IstqbQuiz.Shared.Models;

namespace IstqbQuiz.Shared.Utils
{
    /// <summary>
    /// Zentrale, testbare Bewertungslogik für das Quiz.
    /// Keine UI-/Blazor-Abhängigkeiten.
    /// </summary>
    public static class QuizHelper
    {
        /// <summary>
        /// Summiert die Punkte über alle Fragen (jede Frage max. 1 Punkt).
        /// Eine Frage gilt als richtig, wenn die gegebenen Indizes exakt den korrekten entsprechen
        /// (Reihenfolge egal, gleiche Menge).
        /// </summary>
        public static int Score(IReadOnlyList<Question> questions, IReadOnlyList<IReadOnlyList<int>> answers)
        {
            if (questions is null || answers is null) return 0;

            var count = Math.Min(questions.Count, answers.Count);
            var score = 0;

            for (int i = 0; i < count; i++)
            {
                var q = questions[i];
                var a = answers[i] ?? Array.Empty<int>();

                if (IsCorrect(q.CorrectIndexes, a))
                    score++;
            }

            return score;
        }

        /// <summary>
        /// Prüft, ob die gegebene Antwort exakt korrekt ist (gleiche Menge, Reihenfolge egal).
        /// Öffentlich, damit wir sie separat unit-testen können.
        /// </summary>
        public static bool IsCorrect(IReadOnlyList<int> correctIndexes, IReadOnlyList<int> given)
        {
            if (correctIndexes == null || given == null) return false;
            if (correctIndexes.Count != given.Count) return false;

            return correctIndexes.OrderBy(x => x).SequenceEqual(given.OrderBy(x => x));
        }
    }
}
