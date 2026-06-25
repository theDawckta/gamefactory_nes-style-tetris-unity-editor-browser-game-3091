using System;

public class ScoringSystem
{
    static readonly int[] ScoreTable = { 0, 40, 100, 300, 1200 };

    public int Score { get; private set; }
    public int Level { get; private set; }
    public int TotalLines { get; private set; }

    public event Action<int, int, int> OnStatsChanged;

    public void AddLines(int linesCleared)
    {
        if (linesCleared < 1 || linesCleared > 4)
            return;

        Score += ScoreTable[linesCleared] * (Level + 1);
        TotalLines += linesCleared;
        Level = TotalLines / 10;

        OnStatsChanged?.Invoke(Score, Level, TotalLines);
    }

    public void Reset()
    {
        Score = 0;
        Level = 0;
        TotalLines = 0;
    }
}
