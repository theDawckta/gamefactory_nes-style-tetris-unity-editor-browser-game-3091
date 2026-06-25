using UnityEngine;

public class NextPieceWidget : MonoBehaviour
{
    private const int PreviewSize = 4;
    private const float CellSize = 24f;
    private const float CellVisualScale = 0.9f;
    // Offset to center piece cells (which are in [-1..2] range) inside the 4x4 preview grid.
    private const int CenterOffsetX = 1;
    private const int CenterOffsetY = 1;

    public GameplayController GameplayController;
    public Sprite BlockSprite;
    public Vector3 PreviewWorldPosition = new Vector3(168f, 120f, 0f);

    private SpriteRenderer[] _previewCells;

    private static readonly Color[] ColorMap =
    {
        new Color(0f, 0f, 0f, 0f),
        new Color(0f, 240f / 255f, 240f / 255f),
        new Color(240f / 255f, 240f / 255f, 0f),
        new Color(160f / 255f, 0f, 240f / 255f),
        new Color(0f, 240f / 255f, 0f),
        new Color(240f / 255f, 0f, 0f),
        new Color(0f, 0f, 240f / 255f),
        new Color(240f / 255f, 160f / 255f, 0f),
    };

    private void Start()
    {
        CreatePreviewCells();

        if (GameplayController != null)
            GameplayController.OnNextPieceChanged += DrawPiece;
    }

    private void OnDestroy()
    {
        if (GameplayController != null)
            GameplayController.OnNextPieceChanged -= DrawPiece;
    }

    private void CreatePreviewCells()
    {
        _previewCells = new SpriteRenderer[PreviewSize * PreviewSize];
        float visualSize = CellSize * CellVisualScale;

        // Grid origin: bottom-left corner of the 4x4 grid, centered on PreviewWorldPosition.
        float halfGrid = (PreviewSize * CellSize) / 2f;
        Vector3 origin = PreviewWorldPosition + new Vector3(-halfGrid + CellSize / 2f, -halfGrid + CellSize / 2f, 0f);

        for (int row = 0; row < PreviewSize; row++)
        {
            for (int col = 0; col < PreviewSize; col++)
            {
                var go = new GameObject($"PreviewCell_{col}_{row}");
                go.transform.SetParent(transform, false);
                go.transform.position = origin + new Vector3(col * CellSize, row * CellSize, 0f);
                go.transform.localScale = new Vector3(visualSize, visualSize, 1f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = BlockSprite;
                sr.color = ColorMap[0];
                _previewCells[row * PreviewSize + col] = sr;
            }
        }
    }

    public void DrawPiece(TetrominoData piece)
    {
        for (int i = 0; i < _previewCells.Length; i++)
            _previewCells[i].color = ColorMap[0];

        if (piece == null)
            return;

        Color pieceColor = ColorMap[Mathf.Clamp(piece.colorIndex, 0, ColorMap.Length - 1)];
        Vector2Int[] cells = piece.GetCells(0);

        foreach (var cell in cells)
        {
            int col = cell.x + CenterOffsetX;
            int row = cell.y + CenterOffsetY;
            if (col >= 0 && col < PreviewSize && row >= 0 && row < PreviewSize)
                _previewCells[row * PreviewSize + col].color = pieceColor;
        }
    }
}
