using UnityEngine;
using UnityEngine.UIElements;

public class NextPieceWidget : MonoBehaviour
{
    private const int PreviewSize = 4;
    private const int CenterOffsetX = 1;
    private const int CenterOffsetY = 1;

    [SerializeField] private GameScreen _gameScreen;
    public GameplayController GameplayController;

    private VisualElement[] _previewCells;

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
        BuildUI();
        if (GameplayController != null)
            GameplayController.OnNextPieceChanged += DrawPiece;
    }

    private void OnDestroy()
    {
        if (GameplayController != null)
            GameplayController.OnNextPieceChanged -= DrawPiece;
    }

    private void BuildUI()
    {
        var region = _gameScreen != null ? _gameScreen.NextPieceRegion : null;
        if (region == null) return;
        region.Clear();

        _previewCells = new VisualElement[PreviewSize * PreviewSize];

        var grid = new VisualElement();
        grid.style.flexDirection = FlexDirection.Column;
        grid.style.alignItems = Align.Center;
        grid.style.justifyContent = Justify.Center;
        grid.style.flexGrow = 1;
        region.Add(grid);

        for (int row = PreviewSize - 1; row >= 0; row--)
        {
            var rowEl = new VisualElement();
            rowEl.style.flexDirection = FlexDirection.Row;
            grid.Add(rowEl);

            for (int col = 0; col < PreviewSize; col++)
            {
                var cell = new VisualElement();
                cell.name = $"PreviewCell_{col}_{row}";
                cell.style.width = 20;
                cell.style.height = 20;
                cell.style.backgroundColor = new StyleColor(ColorMap[0]);
                rowEl.Add(cell);
                _previewCells[row * PreviewSize + col] = cell;
            }
        }
    }

    public void DrawPiece(TetrominoData piece)
    {
        if (_previewCells == null) return;

        for (int i = 0; i < _previewCells.Length; i++)
            _previewCells[i].style.backgroundColor = new StyleColor(ColorMap[0]);

        if (piece == null) return;

        Color pieceColor = ColorMap[Mathf.Clamp(piece.colorIndex, 0, ColorMap.Length - 1)];
        Vector2Int[] cells = piece.GetCells(0);

        foreach (var cell in cells)
        {
            int col = cell.x + CenterOffsetX;
            int row = cell.y + CenterOffsetY;
            if (col >= 0 && col < PreviewSize && row >= 0 && row < PreviewSize)
                _previewCells[row * PreviewSize + col].style.backgroundColor = new StyleColor(pieceColor);
        }
    }
}
