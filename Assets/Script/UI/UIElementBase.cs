using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIElementBase : MonoBehaviour
{
    private VisualElement rootVisualElement;

    protected virtual void Awake()
    {
        if (!TryGetComponent(out UIDocument uiDocument))
        {
            Debug.LogAssertion("Attach UIDocument on UIElement Component");
            return;
        }

        rootVisualElement = uiDocument.rootVisualElement;
    }

    public void SetVisible(bool visible)
    {
        rootVisualElement.visible = visible;
    }

    public T Q<T>(string name) where T : VisualElement
    {
        var queried = rootVisualElement.Q<T>(name);
        if (queried is null)
        {
            Debug.LogAssertion($"VisualElement {typeof(T)} not found on the name {name}");
            return null;
        }

        return queried;
    }
}
