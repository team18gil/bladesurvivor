using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public delegate Vector2 MoveValueDelegate();
    private MoveValueDelegate moveValueDelegate;

    [SerializeField] private CharacterObject characterObject;
    [SerializeField] private TextAsset levelExpDataCSVText;

    private LevelExpData levelExpData;

    private int level;
    private float exp;
    private float targetExp;
    private float hp;

    private Coroutine moveCoroutine;

    public CharacterObject CharacterObject => characterObject;

    private void Awake()
    {
        GameManager.Instance.AddEvent(EEvent.GameReady, OnGameReady);
        GameManager.Instance.AddEvent(EEvent.GameStart, OnGameStart);
        GameManager.Instance.AddEvent(EEvent.MonsterHitCharacter, OnMonsterHitCharacter);
        GameManager.Instance.AddEvent(EEvent.ItemAddExp, OnItemAddExp);
        GameManager.Instance.AddEvent(EEvent.GameOver, OnGameOver);

        levelExpData = new(levelExpDataCSVText.text);
    }

    private void OnGameReady(object param)
    {
        characterObject.ClearAllWeapons();
        characterObject.SetStatus(CharacterObject.ECharacterStatus.Disappear);
    }

    private void OnGameStart(object param)
    {
        if (param is GameData gameData)
        {
            SetLevel(1);
            exp = 0f;
            hp = gameData.maxHP;

            GameManager.Instance.SendEvent(EEvent.CharacterSetLevelFirst, (level, targetExp));
            GameManager.Instance.SendEvent(EEvent.CharacterChangeExp, 0f);

            characterObject.InitialVelocity = gameData.velocity;
            characterObject.SetStatus(CharacterObject.ECharacterStatus.Alive);
            characterObject.transform.localPosition = Vector3.zero;
            characterObject.MakeWeapon(CharacterObject.EWeaponType.Sword); // set with character or initial setting
        }
    }

    private void SetLevel(int level)
    {
        this.level = level;
        targetExp = levelExpData.GetTargetExp(level);
    }
    
    private void OnMonsterHitCharacter(object param)
    {
        if (hp <= 0) return;
        float damage = (float)param;
        float nextHP = hp - damage;

        if (hp > nextHP && nextHP <= 0)
        {
            GameManager.Instance.EndGame();
        }
        else
        {
            GameManager.Instance.SendEvent(EEvent.CharacterChangeHP, nextHP);
        }
        hp = nextHP;
    }

    private void OnItemAddExp(object param)
    {
        if (param is float exp)
        {
            float nextExp = this.exp + exp;

            if (nextExp >= targetExp)
            {
                nextExp -= targetExp;
                SetLevel(level + 1); // targetExp changed

                GameManager.Instance.SendEvent(EEvent.CharacterLevelUp, (level, targetExp));
                Debug.Log($"Level up: targetExp is {targetExp}");
            }
            GameManager.Instance.SendEvent(EEvent.CharacterChangeExp, nextExp);
            Debug.Log($"Exp changed: {this.exp} > {nextExp} ( / {targetExp})");
            this.exp = nextExp;
        }
    }

    private void OnGameOver(object param)
    {
        characterObject.SetStatus(CharacterObject.ECharacterStatus.Dead);
    }



    public void StartMove(MoveValueDelegate moveValueDelegate)
    {
        this.moveValueDelegate = moveValueDelegate;
        if (moveCoroutine is null)
        {
            moveCoroutine = StartCoroutine(MoveCoroutine());
        }
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            var delta = moveValueDelegate is not null ? moveValueDelegate() : Vector2.zero;
            characterObject.MoveCharacterBy(delta);

            yield return new WaitForFixedUpdate();
        }
    }

    public void EndMove()
    {
        moveValueDelegate = null;
        if (moveCoroutine is not null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
}
