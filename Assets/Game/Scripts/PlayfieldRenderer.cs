using UnityEngine;

public class PlayfieldRenderer : MonoBehaviour
{
    private const int Cols = 10;
    private const int VisibleRows = 20;
    private const float CellSize = 24f;
    private const float CellVisualScale = 0.9f;

    public PlayfieldController PlayfieldController;
    public PieceController PieceController;
    public Sprite BlockSprite;

    private SpriteRenderer[] _gridRenderers;
    private SpriteRenderer[] _pieceRenderers;

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
        CreateGridRenderers();
        CreatePieceRenderers();
        CreateBorder();
    }

    private Vector3 GridOrigin()
    {
        float x = -(Cols * CellSize) / 2f + CellSize / 2f;
        float y = -(VisibleRows * CellSize) / 2f + CellSize / 2f;
        return new Vector3(x, y, 0f);
    }

    private void CreateGridRenderers()
    {
        _gridRenderers = new SpriteRenderer[Cols * VisibleRows];
        Vector3 origin = GridOrigin();
        float visualSize = CellSize * CellVisualScale;

        for (int row = 0; row < VisibleRows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                var go = new GameObject($"Cell_{col}_{row}");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = origin + new Vector3(col * CellSize, row * CellSize, 0f);
                go.transform.localScale = new Vector3(visualSize, visualSize, 1f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = BlockSprite;
                sr.color = ColorMap[0];
                _gridRenderers[row * Cols + col] = sr;
            }
        }
    }

    private void CreatePieceRenderers()
    {
        _pieceRenderers = new SpriteRenderer[4];
        float visualSize = CellSize * CellVisualScale;

        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject($"PieceCell_{i}");
            go.transform.SetParent(transform, false);
            go.transform.localScale = new Vector3(visualSize, visualSize, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = BlockSprite;
            sr.color = ColorMap[0];
            sr.sortingOrder = 1;
            _pieceRenderers[i] = sr;
        }
    }

    private void CreateBorder()
    {
        float halfW = (Cols * CellSize) / 2f;
        float halfH = (VisibleRows * CellSize) / 2f;
        float gridW = Cols * CellSize;
        float gridH = VisibleRows * CellSize;

        CreateBorderRenderer("Border_Left",   new Vector3(-halfW - 0.5f, 0f, 0f), new Vector3(1f, gridH + 2f, 1f));
        CreateBorderRenderer("Border_Right",  new Vector3(halfW + 0.5f,  0f, 0f), new Vector3(1f, gridH + 2f, 1f));
        CreateBorderRenderer("Border_Top",    new Vector3(0f,  halfH + 0.5f, 0f), new Vector3(gridW, 1f, 1f));
        CreateBorderRenderer("Border_Bottom", new Vector3(0f, -halfH - 0.5f, 0f), new Vector3(gridW, 1f, 1f));
    }

    private void CreateBorderRenderer(string goName, Vector3 localPos, Vector3 localScale)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = localScale;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = BlockSprite;
        sr.color = Color.white;
    }

    private void LateUpdate()
    {
        RefreshGrid();
        RefreshActivePiece();
    }

    private void RefreshGrid()
    {
        for (int row = 0; row < VisibleRows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                int colorIndex = PlayfieldController.GetCell(row, col);
                _gridRenderers[row * Cols + col].color =
                    ColorMap[Mathf.Clamp(colorIndex, 0, ColorMap.Length - 1)];
            }
        }
    }

    private void RefreshActivePiece()
    {
        TetrominoData piece = PieceController.ActivePiece;
        if (piece == null)
        {
            HidePieceRenderers();
            return;
        }

        Color pieceColor = ColorMap[Mathf.Clamp(piece.colorIndex, 0, ColorMap.Length - 1)];
        Vector2Int[] cells = piece.GetCells(PieceController.CurrentRotation);
        Vector3 origin = GridOrigin();

        for (int i = 0; i < 4; i++)
        {
            if (i < cells.Length)
            {
                int col = PieceController.CurrentPivot.x + cells[i].x;
                int row = PieceController.CurrentPivot.y + cells[i].y;
                _pieceRenderers[i].transform.localPosition =
                    origin + new Vector3(col * CellSize, row * CellSize, -0.1f);
                _pieceRenderers[i].color = pieceColor;
            }
            else
            {
                _pieceRenderers[i].color = ColorMap[0];
            }
        }
    }

    private void HidePieceRenderers()
    {
        for (int i = 0; i < 4; i++)
            _pieceRenderers[i].color = ColorMap[0];
    }
}
