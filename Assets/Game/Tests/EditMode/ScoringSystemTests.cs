using NUnit.Framework;
using System.Collections.Generic;

public class ScoringSystemTests
{
    ScoringSystem scoring;

    [SetUp]
    public void SetUp()
    {
        scoring = new ScoringSystem();
    }

    [Test]
    public void AddLines_Single_AtLevel0_Adds40Points()
    {
        scoring.AddLines(1);
        Assert.AreEqual(40, scoring.Score);
    }

    [Test]
    public void AddLines_Tetris_AtLevel0_Adds1200Points()
    {
        scoring.AddLines(4);
        Assert.AreEqual(1200, scoring.Score);
    }

    [Test]
    public void AddLines_Tetris_AtLevel1_Adds2400Points()
    {
        // Reach level 1 by clearing 10 lines first
        for (int i = 0; i < 10; i++)
            scoring.AddLines(1);
        int scoreBeforeTetris = scoring.Score;
        scoring.AddLines(4);
        Assert.AreEqual(scoreBeforeTetris + 2400, scoring.Score);
    }

    [Test]
    public void AddLines_Double_AtLevel0_Adds100Points()
    {
        scoring.AddLines(2);
        Assert.AreEqual(100, scoring.Score);
    }

    [Test]
    public void AddLines_Triple_AtLevel0_Adds300Points()
    {
        scoring.AddLines(3);
        Assert.AreEqual(300, scoring.Score);
    }

    [Test]
    public void LevelIncrements_After10LinesCleared()
    {
        Assert.AreEqual(0, scoring.Level);
        for (int i = 0; i < 10; i++)
            scoring.AddLines(1);
        Assert.AreEqual(1, scoring.Level);
    }

    [Test]
    public void TotalLines_TracksCorrectly()
    {
        scoring.AddLines(2);
        scoring.AddLines(3);
        Assert.AreEqual(5, scoring.TotalLines);
    }

    [Test]
    public void OnStatsChanged_FiresAfterAddLines()
    {
        int fireCount = 0;
        scoring.OnStatsChanged += (score, level, lines) => fireCount++;
        scoring.AddLines(1);
        scoring.AddLines(4);
        Assert.AreEqual(2, fireCount);
    }

    [Test]
    public void OnStatsChanged_ReceivesUpdatedValues()
    {
        int lastScore = -1, lastLevel = -1, lastLines = -1;
        scoring.OnStatsChanged += (score, level, lines) =>
        {
            lastScore = score;
            lastLevel = level;
            lastLines = lines;
        };
        scoring.AddLines(1);
        Assert.AreEqual(40, lastScore);
        Assert.AreEqual(0, lastLevel);
        Assert.AreEqual(1, lastLines);
    }

    [Test]
    public void Reset_SetsAllValuesToZero()
    {
        scoring.AddLines(4);
        scoring.Reset();
        Assert.AreEqual(0, scoring.Score);
        Assert.AreEqual(0, scoring.Level);
        Assert.AreEqual(0, scoring.TotalLines);
    }
}
