using NUnit.Framework;
using UnityEngine;

public class TetrominoDataTests
{
    TetrominoData CreateTestData(int colorIndex = 1)
    {
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        data.colorIndex = colorIndex;
        data.rotationStates = new TetrominoData.RotationState[4];
        for (int i = 0; i < 4; i++)
        {
            data.rotationStates[i] = new TetrominoData.RotationState
            {
                cells = new Vector2Int[]
                {
                    new Vector2Int(-1, 0), new Vector2Int(0, 0),
                    new Vector2Int(1, 0),  new Vector2Int(2, 0)
                }
            };
        }
        return data;
    }

    [Test]
    public void TetrominoData_HasFourRotationStates()
    {
        var data = CreateTestData();
        Assert.AreEqual(4, data.rotationStates.Length);
    }

    [Test]
    public void TetrominoData_EachRotationStateHasFourCells()
    {
        var data = CreateTestData();
        foreach (var state in data.rotationStates)
            Assert.AreEqual(4, state.cells.Length);
    }

    [Test]
    public void TetrominoData_ColorIndexIsSet()
    {
        var data = CreateTestData(colorIndex: 3);
        Assert.AreEqual(3, data.colorIndex);
    }

    [Test]
    public void TetrominoData_GetCells_ReturnsCorrectRotation()
    {
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        data.rotationStates = new TetrominoData.RotationState[4];
        for (int i = 0; i < 4; i++)
        {
            data.rotationStates[i] = new TetrominoData.RotationState
            {
                cells = new Vector2Int[] { new(i, 0), new(i, 1), new(i, 2), new(i, 3) }
            };
        }
        Assert.AreEqual(new Vector2Int(2, 0), data.GetCells(2)[0]);
    }

    [Test]
    public void TetrominoDefinitions_AllReturnsSeven()
    {
        Assert.AreEqual(7, TetrominoDefinitions.All.Length);
    }

    [Test]
    public void TetrominoDefinitions_AllAssetsHaveFourRotationStates()
    {
        foreach (var data in TetrominoDefinitions.All)
            Assert.AreEqual(4, data.rotationStates.Length);
    }

    [Test]
    public void TetrominoDefinitions_AllAssetsHaveValidColorIndex()
    {
        foreach (var data in TetrominoDefinitions.All)
        {
            Assert.GreaterOrEqual(data.colorIndex, 1);
            Assert.LessOrEqual(data.colorIndex, 7);
        }
    }

    [Test]
    public void TetrominoDefinitions_RandomReturnsNonNull()
    {
        Assert.IsNotNull(TetrominoDefinitions.Random());
    }
}
