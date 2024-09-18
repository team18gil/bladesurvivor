using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public delegate Vector2 MoveValueDelegate();
    private MoveValueDelegate moveValueDelegate;

    [SerializeField] private CharacterObject characterObject;
    private float hp;

    private Coroutine moveCoroutine;

    private void Awake()
    {
        GameManager.Instance.AddEvent(EEvent.GameReady, OnGameReady);
        GameManager.Instance.AddEvent(EEvent.GameStart, OnGameStart);
        GameManager.Instance.AddEvent(EEvent.MonsterHitCharacter, OnMonsterHitCharacter);
        GameManager.Instance.AddEvent(EEvent.GameOver, OnGameOver);
    }

    private void OnGameReady(object param)
    {
        characterObject.ClearAllWeapons();
        characterObject.SetStatus(CharacterObject.ECharacterStatus.Disappear);
    }

    private void OnGameStart(object param)
    {
        var gameData = (GameData)param;
        hp = gameData.maxHP;

        characterObject.InitialVelocity = gameData.velocity;
        characterObject.SetStatus(CharacterObject.ECharacterStatus.Alive);
        characterObject.transform.localPosition = Vector3.zero;
        characterObject.MakeWeapon(CharacterObject.EWeaponType.Sword); // set with character or initial setting
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
            GameManager.Instance.SendEvent(EEvent.ChangeHP, nextHP);
        }
        hp = nextHP;
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
