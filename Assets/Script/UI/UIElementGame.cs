using UnityEngine;
using UnityEngine.UIElements;

public class UIElementGame : UIElementBase
{
    private Label timerLabel;
    private ProgressBar xpProgressBar;

    protected override void Awake()
    {
        base.Awake();

        timerLabel = Q<Label>("TimerLabel");
        xpProgressBar = Q<ProgressBar>("XPProgressBar");
        xpProgressBar.lowValue = 0f;

        var pauseButton = Q<Button>("PauseButton");
        pauseButton.clicked += OnPauseButtonClicked;

        GameManager.Instance.AddEvent(EEvent.GameStart, OnGameStart);
        GameManager.Instance.AddEvent(EEvent.GameTimerTick, OnTimerTick);
        GameManager.Instance.AddEvent(EEvent.CharacterSetLevelFirst, OnLevelChanged);
        GameManager.Instance.AddEvent(EEvent.CharacterChangeExp, OnExpChanged);
        GameManager.Instance.AddEvent(EEvent.CharacterLevelUp, OnLevelChanged);
    }

    private void OnPauseButtonClicked()
    {
        
    }

    private void OnGameStart(object param)
    {
        OnTimerTick(0);
    }

    private void OnTimerTick(object param)
    {
        if (param is int fullTimes)
        {
            int seconds = fullTimes % 60;
            fullTimes = (fullTimes - seconds) / 60;

            int minutes = fullTimes % 60;
            fullTimes = (fullTimes - minutes) / 60;

            timerLabel.text = $"{fullTimes:D2}:{minutes:D2}:{seconds:D2}";
        }
    }

    private void OnLevelChanged(object param)
    {
        if (param is (int _, float exp))
        {
            Debug.Log($"Changing highvalue: {exp}");
            xpProgressBar.highValue = exp;
        }
    }

    private void OnExpChanged(object param)
    {
        if (param is float exp)
        {
            Debug.Log($"Changing value: {exp}");
            xpProgressBar.value = exp;
        }
    }
}
