using UnityEngine;

[CreateAssetMenu(menuName = "Tetris/TetrominoData")]
public class TetrominoData : ScriptableObject
{
    [System.Serializable]
    public class RotationState
    {
        public Vector2Int[] cells;
    }

    public int colorIndex;
    public RotationState[] rotationStates = new RotationState[4];

    public Vector2Int[] GetCells(int rotationIndex)
    {
        return rotationStates[rotationIndex % rotationStates.Length].cells;
    }
}
