using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private CharacterObject characterObject;
    private float hp;
    

    private void Awake()
    {
        GameManager.Instance.AddEvent(EEvent.GameReady, OnGameReady);
        GameManager.Instance.AddEvent(EEvent.GameStart, OnGameStart);
        GameManager.Instance.AddEvent(EEvent.MonsterHitCharacter, OnMonsterHitCharacter);
        GameManager.Instance.AddEvent(EEvent.ChangeHP, OnChangeHP);
        GameManager.Instance.AddEvent(EEvent.GameOver, OnGameOver);
    }

    private void OnGameReady(object param)
    {
        characterObject.ClearAllWeapons();
        characterObject.SetStatus(CharacterObject.ECharacterStatus.Disappear);
    }

    private void OnGameStart(object param)
    {
        characterObject.SetStatus(CharacterObject.ECharacterStatus.Alive);
        characterObject.transform.localPosition = Vector3.zero;
        characterObject.MakeWeapon(CharacterObject.EWeaponType.Sword); // set with character or initial setting

        float initialHP = GameManager.Instance.UserData.DefaultHP;
        GameManager.Instance.SendEvent(EEvent.ChangeHP, (0f, initialHP));
    }

    private void OnMonsterHitCharacter(object param)
    {
        float damage = (float)param;
        float prevHP = hp;
        float nextHP = hp - damage;
        if (prevHP <= 0 && nextHP < 0) return;

        GameManager.Instance.SendEvent(EEvent.ChangeHP, (prevHP, nextHP));
    }

    private void OnChangeHP(object param)
    {
        var pair = ((float, float))param;
        float prevHP = pair.Item1;
        float nextHP = pair.Item2;

        hp = nextHP;
        if (prevHP > nextHP && hp <= 0)
        {
            GameManager.Instance.EndGame();
        }
    }

    private void OnGameOver(object param)
    {
        characterObject.SetStatus(CharacterObject.ECharacterStatus.Dead);
    }



    public void MoveCharacterBy(Vector3 delta)
    {
        characterObject.MoveCharacterBy(delta);
    }
}
