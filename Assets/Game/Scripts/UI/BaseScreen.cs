using UnityEngine;
using UnityEngine.UIElements;

public class BaseScreen : MonoBehaviour
{
    protected UIDocument Document { get; private set; }

    public bool IsVisible =>
        Document != null &&
        Document.rootVisualElement != null &&
        Document.rootVisualElement.style.display.value == DisplayStyle.Flex;

    protected virtual void Awake()
    {
        Document = GetComponent<UIDocument>();
        Hide();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        Document.rootVisualElement.style.display = DisplayStyle.Flex;
        OnShow();
    }

    public virtual void Hide()
    {
        if (Document != null && Document.rootVisualElement != null)
            Document.rootVisualElement.style.display = DisplayStyle.None;
        OnHide();
    }

    protected virtual void OnShow() { }

    protected virtual void OnHide() { }
}
