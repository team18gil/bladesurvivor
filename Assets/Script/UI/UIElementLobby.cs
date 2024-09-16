using UnityEngine;
using UnityEngine.UIElements;

public class UIElementLobby : UIElementBase
{
    protected override void Awake()
    {
        base.Awake();

        var startButton = Q<Button>("StartButton");
        startButton.clicked += () =>
        {
            GameManager.Instance.StartGame();
        };
    }
}
