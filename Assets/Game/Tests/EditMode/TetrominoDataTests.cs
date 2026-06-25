using NUnit.Framework;
using UnityEngine;

public class TetrominoDataTests
{
    private TetrominoData CreateInstance(int colorIndex, Vector2Int[][] states)
    {
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        data.colorIndex = colorIndex;
        data.rotationStates = new TetrominoData.RotationState[4];
        for (int i = 0; i < 4; i++)
        {
            data.rotationStates[i] = new TetrominoData.RotationState();
            data.rotationStates[i].cells = states[i];
        }
        return data;
    }

    [Test]
    public void TetrominoData_HasFourRotationStates()
    {
        var data = CreateInstance(1, new Vector2Int[][]
        {
            new Vector2Int[] { new(0,0), new(1,0), new(2,0), new(3,0) },
            new Vector2Int[] { new(0,0), new(0,1), new(0,2), new(0,3) },
            new Vector2Int[] { new(0,0), new(1,0), new(2,0), new(3,0) },
            new Vector2Int[] { new(0,0), new(0,1), new(0,2), new(0,3) }
        });
        Assert.AreEqual(4, data.rotationStates.Length);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void TetrominoData_EachRotationStateHasFourCells()
    {
        var data = CreateInstance(3, new Vector2Int[][]
        {
            new Vector2Int[] { new(0,0), new(1,0), new(2,0), new(1,1) },
            new Vector2Int[] { new(1,0), new(0,1), new(1,1), new(1,2) },
            new Vector2Int[] { new(1,0), new(0,1), new(1,1), new(2,1) },
            new Vector2Int[] { new(0,0), new(0,1), new(1,1), new(0,2) }
        });
        foreach (var state in data.rotationStates)
            Assert.AreEqual(4, state.cells.Length);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void TetrominoData_ColorIndexIsSet()
    {
        var data = CreateInstance(5, new Vector2Int[][]
        {
            new Vector2Int[] { new(0,0), new(1,0), new(1,1), new(2,1) },
            new Vector2Int[] { new(1,0), new(0,1), new(1,1), new(0,2) },
            new Vector2Int[] { new(0,0), new(1,0), new(1,1), new(2,1) },
            new Vector2Int[] { new(1,0), new(0,1), new(1,1), new(0,2) }
        });
        Assert.AreEqual(5, data.colorIndex);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void TetrominoData_GetCells_ReturnsCorrectRotation()
    {
        var expected = new Vector2Int[] { new(0,1), new(1,1), new(2,1), new(3,1) };
        var data = CreateInstance(1, new Vector2Int[][]
        {
            expected,
            new Vector2Int[] { new(2,0), new(2,1), new(2,2), new(2,3) },
            new Vector2Int[] { new(0,2), new(1,2), new(2,2), new(3,2) },
            new Vector2Int[] { new(1,0), new(1,1), new(1,2), new(1,3) }
        });
        CollectionAssert.AreEqual(expected, data.GetCells(0));
        Object.DestroyImmediate(data);
    }

    [Test]
    public void TetrominoDefinitions_AllReturnsSeven()
    {
        var all = TetrominoDefinitions.All;
        Assert.AreEqual(7, all.Length, "Expected 7 tetromino assets in Resources/Tetrominoes/");
    }

    [Test]
    public void TetrominoDefinitions_AllAssetsHaveFourRotationStates()
    {
        foreach (var data in TetrominoDefinitions.All)
        {
            Assert.IsNotNull(data, "Null TetrominoData asset found");
            Assert.AreEqual(4, data.rotationStates.Length,
                $"{data.name} should have 4 rotation states");
            foreach (var state in data.rotationStates)
                Assert.AreEqual(4, state.cells.Length,
                    $"{data.name} rotation state should have 4 cells");
        }
    }

    [Test]
    public void TetrominoDefinitions_AllAssetsHaveValidColorIndex()
    {
        foreach (var data in TetrominoDefinitions.All)
        {
            Assert.IsNotNull(data);
            Assert.Greater(data.colorIndex, 0,
                $"{data.name} colorIndex should be > 0");
            Assert.LessOrEqual(data.colorIndex, 7,
                $"{data.name} colorIndex should be <= 7");
        }
    }

    [Test]
    public void TetrominoDefinitions_RandomReturnsNonNull()
    {
        var piece = TetrominoDefinitions.Random();
        Assert.IsNotNull(piece);
    }
}
