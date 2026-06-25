using UnityEngine.UIElements;

public class GameScreen : BaseScreen
{
    private VisualElement _scoreRegion;
    private VisualElement _levelRegion;
    private VisualElement _linesRegion;
    private VisualElement _nextPieceRegion;

    public VisualElement ScoreRegion => _scoreRegion ??= Document?.rootVisualElement?.Q("scoreRegion");
    public VisualElement LevelRegion => _levelRegion ??= Document?.rootVisualElement?.Q("levelRegion");
    public VisualElement LinesRegion => _linesRegion ??= Document?.rootVisualElement?.Q("linesRegion");
    public VisualElement NextPieceRegion => _nextPieceRegion ??= Document?.rootVisualElement?.Q("nextPieceRegion");
}
