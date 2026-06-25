using System.Collections.Generic;
using UnityEngine;

public class PlayfieldController : MonoBehaviour
{
    private const int Rows = 22;
    private const int Cols = 10;

    private int[,] _grid = new int[Rows, Cols];

    public bool IsValidPosition(TetrominoData piece, int rotationIndex, Vector2Int pivot)
    {
        Vector2Int[] cells = piece.GetCells(rotationIndex);
        foreach (var cell in cells)
        {
            int row = pivot.y + cell.y;
            int col = pivot.x + cell.x;
            if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                return false;
            if (_grid[row, col] != 0)
                return false;
        }
        return true;
    }

    public void LockPiece(TetrominoData piece, int rotationIndex, Vector2Int pivot)
    {
        Vector2Int[] cells = piece.GetCells(rotationIndex);
        foreach (var cell in cells)
        {
            int row = pivot.y + cell.y;
            int col = pivot.x + cell.x;
            _grid[row, col] = piece.colorIndex;
        }
    }

    public int[] ClearLines()
    {
        var clearedRows = new List<int>();
        for (int row = 0; row < Rows; row++)
        {
            bool full = true;
            for (int col = 0; col < Cols; col++)
            {
                if (_grid[row, col] == 0) { full = false; break; }
            }
            if (full) clearedRows.Add(row);
        }

        if (clearedRows.Count == 0)
            return clearedRows.ToArray();

        int writeRow = 0;
        for (int readRow = 0; readRow < Rows; readRow++)
        {
            if (clearedRows.Contains(readRow)) continue;
            if (writeRow != readRow)
            {
                for (int col = 0; col < Cols; col++)
                    _grid[writeRow, col] = _grid[readRow, col];
            }
            writeRow++;
        }

        while (writeRow < Rows)
        {
            for (int col = 0; col < Cols; col++)
                _grid[writeRow, col] = 0;
            writeRow++;
        }

        return clearedRows.ToArray();
    }

    public int GetCell(int row, int col)
    {
        return _grid[row, col];
    }

    public void SetCell(int row, int col, int value)
    {
        _grid[row, col] = value;
    }

    public void ClearAll()
    {
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Cols; col++)
                _grid[row, col] = 0;
    }
}
