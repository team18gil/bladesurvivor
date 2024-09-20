using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private enum EUIElementType
    {
        Ready,
        Game,
        GameOver,
    }

    [System.Serializable]
    private struct UIElementSerializableObject
    {
        public EUIElementType Type;
        public UIElementBase UIDocument;
    }

    [SerializeField] private List<UIElementSerializableObject> uiElements;
    private Dictionary<EUIElementType, UIElementBase> uiElementDic = new();

    private void Awake()
    {
        GameManager.Instance.AddEvent(EEvent.GameReady, OnGameReady);
        GameManager.Instance.AddEvent(EEvent.GameStart, OnGameStart);
        GameManager.Instance.AddEvent(EEvent.GameOver, OnGameOver);

        foreach (var obj in uiElements)
        {
            if (obj.UIDocument != null)
            {
                uiElementDic[obj.Type] = obj.UIDocument;
            }
        }
    }

    private void SetUIElementActive(EUIElementType elementType)
    {
        foreach (var pair in uiElementDic)
        {
            pair.Value.SetVisible(pair.Key.Equals(elementType));
        }
    }

    private void OnGameReady(object param)
    {
        SetUIElementActive(EUIElementType.Ready);
    }

    private void OnGameStart(object param)
    {
        SetUIElementActive(EUIElementType.Game);
    }

    private void OnGameOver(object param)
    {
        SetUIElementActive(EUIElementType.GameOver);
    }
}
