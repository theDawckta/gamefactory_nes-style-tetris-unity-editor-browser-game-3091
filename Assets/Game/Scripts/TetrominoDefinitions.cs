using UnityEngine;

public static class TetrominoDefinitions
{
    private static TetrominoData[] _all;

    public static TetrominoData[] All
    {
        get
        {
            if (_all == null || _all.Length == 0)
                _all = Resources.LoadAll<TetrominoData>("Tetrominoes");
            return _all;
        }
    }

    public static TetrominoData Random()
    {
        return All[UnityEngine.Random.Range(0, All.Length)];
    }
}
