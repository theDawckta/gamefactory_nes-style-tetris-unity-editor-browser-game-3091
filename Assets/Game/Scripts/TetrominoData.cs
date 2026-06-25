using UnityEngine;

[CreateAssetMenu(fileName = "TetrominoData", menuName = "Tetris/TetrominoData")]
public class TetrominoData : ScriptableObject
{
    [System.Serializable]
    public class RotationState
    {
        public Vector2Int[] cells = new Vector2Int[4];
    }

    public int colorIndex;
    public RotationState[] rotationStates = new RotationState[4];

    public Vector2Int[] GetCells(int rotationIndex)
    {
        return rotationStates[rotationIndex].cells;
    }
}
