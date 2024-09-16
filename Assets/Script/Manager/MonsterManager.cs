using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public enum EMonsterType
    {
        Default,
        DefaultFaster,
    }
    [SerializeField] private CharacterObject characterObject;

    private List<MonsterBase> monsters = new();
    private Coroutine monsterGenerateCoroutine = null;

    private void Awake()
    {
        GameManager.Instance.AddEvent(EEvent.GameReady, OnGameReady);
        GameManager.Instance.AddEvent(EEvent.GameStart, OnGameStart);
        GameManager.Instance.AddEvent(EEvent.GameOver, OnGameOver);
    }

    private void OnGameReady(object param)
    {
        var goPool = GameManager.Instance.GameObjectPool;
        if (monsters.Count > 0)
        {
            foreach (var monster in monsters)
            {
                goPool.Release(monster);
            }
            monsters.Clear();
        }
    }

    private void OnGameStart(object param)
    {
        monsterGenerateCoroutine = StartCoroutine(MonsterGenerateCoroutine());
    }

    private IEnumerator MonsterGenerateCoroutine()
    {
        while (true)
        {
            bool makeFaster = Random.Range(0, 4).Equals(0);
            EMonsterType monsterType = EMonsterType.Default;
            if (makeFaster)
                monsterType = EMonsterType.DefaultFaster;

            CreateMonster(monsterType, GameManager.Instance.PlaygroundParent);

            yield return new WaitForSeconds(0.8f);
        }
    }

    public void CreateMonster(EMonsterType type, Transform parent)
    {
        MonsterBase monster = null;
        var goPool = GameManager.Instance.GameObjectPool;
        switch(type)
        {
            case EMonsterType.Default:
                monster = goPool.Get<MonsterDefault>(parent);
                break;
            case EMonsterType.DefaultFaster:
                monster = goPool.Get<MonsterDefaultFaster>(parent);
                break;
        }

        if (monster is not null)
        {
            monster.Initialize(characterObject, OnMonsterDead);
            monsters.Add(monster);
        }
    }

    private void OnMonsterDead(MonsterBase monster)
    {
        // dead effect
        if (monsters.Contains(monster))
        {
            monsters.Remove(monster);
        }
        GameManager.Instance.GameObjectPool.Release(monster);
    }

    private void OnGameOver(object param)
    {
        if (monsterGenerateCoroutine is not null)
        {
            StopCoroutine(monsterGenerateCoroutine);
            monsterGenerateCoroutine = null;
        }

        foreach (var monster in monsters)
        {
            monster.Stop();
        }
    }
}
