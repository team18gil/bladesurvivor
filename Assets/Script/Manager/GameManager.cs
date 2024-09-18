using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance is null) _instance = FindFirstObjectByType<GameManager>();
            return _instance;
        }
    }

    #endregion

    #region Events

    [SerializeField] private EventManager eventManager;

    public void AddEvent(EEvent eventType, EventManager.EventDelegate eventDelegate)
    {
        eventManager.AddEventDelegate(eventType, eventDelegate);
    }

    public void RemoveEvent(EEvent eventType, EventManager.EventDelegate eventDelegate)
    {
        eventManager.RemoveEventDelegate(eventType, eventDelegate);
    }

    public void SendEvent(EEvent eventType, object parameter = null)
    {
        eventManager.SendEvent(eventType, parameter);
    }

    #endregion

    #region Pool

    [SerializeField] private GameObjectPool gameObjectPool;

    public GameObjectPool GameObjectPool => gameObjectPool;

    #endregion



    #region Games

    [SerializeField] private Camera mainCamera;
    [SerializeField] private UserData userData;
    [SerializeField] private Transform playgroundParent;
    // use direcly instead of event manager to call frequently
    [SerializeField] private CharacterManager characterManager;

    public UserData UserData => userData;
    public Transform PlaygroundParent => playgroundParent;

    private enum EGameStatus
    {
        Ready,
        Playing,
        Over,
    }

    private DefaultInputActionAsset inputActionAsset;
    private EGameStatus gameStatus;
    private Coroutine timerCoroutine = null;



    public CharacterObject CharacterObject => characterManager != null ? characterManager.CharacterObject : null;

    private void Awake()
    {
        _instance = this;

        inputActionAsset = new();
        gameObjectPool.Initialize();
    }

    private void Start()
    {
        inputActionAsset.Player.Enable();
        ReadyGame();
    }

    public void ReadyGame()
    {
        gameStatus = EGameStatus.Ready;
        eventManager.SendEvent(EEvent.GameReady);
    }



    public void StartGame()
    {
        // TODO: set initial data on here
        var newGameData = new GameData()
        {
            velocity = 4f,
            maxHP = 100f,
        };

        gameStatus = EGameStatus.Playing;
        eventManager.SendEvent(EEvent.GameStart, newGameData);
        inputActionAsset.Player.Move.started += OnMoveStarted;
        inputActionAsset.Player.Move.canceled += OnMoveCanceled;

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private void OnMoveStarted(InputAction.CallbackContext obj)
    {
        characterManager.StartMove(GetMoveValue);
    }

    private Vector2 GetMoveValue()
    {
        return inputActionAsset.Player.Move.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        characterManager.EndMove();
    }

    private IEnumerator TimerCoroutine()
    {
        // Set to -1 to call TimerTick initially on 0 seconds
        int timer = -1;
        while (true)
        {
            eventManager.SendEvent(EEvent.GameTimerTick, ++timer);

            // time goes slightly faster to make gamers feel they are good at playing
            yield return new WaitForSeconds(0.9f);
        }
    }

    public void PauseGame()
    {
        if (gameStatus.Equals(EGameStatus.Playing))
        {
            Time.timeScale = 0f;
            eventManager.SendEvent(EEvent.GamePause);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        eventManager.SendEvent(EEvent.GameResume);
    }

    public void EndGame()
    {
        gameStatus = EGameStatus.Over;
        eventManager.SendEvent(EEvent.GameOver);
        inputActionAsset.Player.Move.started -= OnMoveStarted;
        inputActionAsset.Player.Move.canceled -= OnMoveCanceled;

        if (timerCoroutine is not null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    #endregion
}
