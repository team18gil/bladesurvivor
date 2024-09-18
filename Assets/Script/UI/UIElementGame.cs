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

        var pauseButton = Q<Button>("PauseButton");
        pauseButton.clicked += OnPauseButtonClicked;
    }

    private void OnPauseButtonClicked()
    {
        
    }

    private void OnEnable()
    {
        GameManager.Instance.AddEvent(EEvent.TimerTick, OnTimerTick);
        xpProgressBar.value = 0;
    }

    private void OnTimerTick(object param)
    {
        int fullTimes = (int)param;

        int seconds = fullTimes % 60;
        fullTimes = (fullTimes - seconds) / 60;

        int minutes = fullTimes % 60;
        fullTimes = (fullTimes - minutes) / 60;

        timerLabel.text = $"{fullTimes:D2}:{minutes:D2}:{seconds:D2}";
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveEvent(EEvent.TimerTick, OnTimerTick);
    }
}
