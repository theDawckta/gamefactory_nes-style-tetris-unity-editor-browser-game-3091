using UnityEngine;
using UnityEngine.UIElements;

public class BaseScreen : MonoBehaviour
{
    protected UIDocument Document { get; private set; }

    public bool IsVisible
    {
        get
        {
            var root = Document?.rootVisualElement;
            return root != null && root.style.display.value == DisplayStyle.Flex;
        }
    }

    protected virtual void Awake()
    {
        Document = GetComponent<UIDocument>();
        Hide();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        var root = Document.rootVisualElement;
        if (root != null)
        {
            root.style.display = DisplayStyle.Flex;
        }
        OnShow();
    }

    public virtual void Hide()
    {
        var root = Document?.rootVisualElement;
        if (root != null)
        {
            root.style.display = DisplayStyle.None;
        }
        OnHide();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}
