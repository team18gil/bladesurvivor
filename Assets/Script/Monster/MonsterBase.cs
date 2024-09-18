using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    protected virtual float MaxHP => 0f;
    protected virtual float Damage => 0f;
    protected virtual float DamageInterval => 9999f;

    protected float hp;
    protected CharacterObject characterObject;

    private Coroutine characterHitCoroutine;
    private Dictionary<WeaponProjectileDefault, Coroutine> weaponDamageCoroutineDic = new();

    protected virtual void OnEnable()
    {
        hp = MaxHP;
    }

    public void Initialize(CharacterObject characterObject)
    {
        this.characterObject = characterObject;

        StartCoroutine(MoveCoroutine());
        SetInitialPosition();
    }

    protected virtual void SetInitialPosition()
    {
        // functions about initial position setting
        Debug.LogError($"Don't call base's SetInitialPosition!");
    }

    protected virtual IEnumerator MoveCoroutine()
    {
        // functions about movements
        Debug.LogError($"Don't call base's MoveCoroutine!");
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (characterHitCoroutine is null)
            {
                characterHitCoroutine = StartCoroutine(CharacterHitCoroutine());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (characterHitCoroutine is not null)
            {
                StopCoroutine(characterHitCoroutine);
            }
        }
    }

    private IEnumerator CharacterHitCoroutine()
    {
        while(true)
        {
            GameManager.Instance.SendEvent(EEvent.MonsterHitCharacter, Damage);
            yield return new WaitForSeconds(DamageInterval);
        }
    }

    public void HitStart(WeaponProjectileDefault projectile, float damage, float damageInterval)
    {
        StopCoroutineAtDicIfRun(projectile);

        var newCoroutine = StartCoroutine(HitCoroutine(damage, damageInterval));
        weaponDamageCoroutineDic.Add(projectile, newCoroutine);
    }

    private void StopCoroutineAtDicIfRun(WeaponProjectileDefault projectile)
    {
        if (weaponDamageCoroutineDic.TryGetValue(projectile, out var prevCoroutine))
        {
            weaponDamageCoroutineDic.Remove(projectile);
            if (prevCoroutine is not null) StopCoroutine(prevCoroutine);
        }
    }

    private IEnumerator HitCoroutine(float damage, float damageInterval)
    {
        while (true)
        {
            hp -= damage;
            if (hp <= 0)
            {
                GameManager.Instance.SendEvent(EEvent.MonsterDead, this);
                Stop();
                
                break;
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    public void HitEnd(WeaponProjectileDefault projectile)
    {
        StopCoroutineAtDicIfRun(projectile);
    }



    public void Stop()
    {
        StopAllCoroutines();
        weaponDamageCoroutineDic.Clear();
        characterHitCoroutine = null;
    }

    private void OnDisable()
    {
        Stop();
    }
}
