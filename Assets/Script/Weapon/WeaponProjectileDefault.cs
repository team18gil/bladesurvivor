using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileDefault : MonoBehaviour
{
    /// <returns>true if monster is alive after hit</returns>
    public delegate void WeaponProjectileAttackDelegate(WeaponProjectileDefault projectile, MonsterBase monster);
    public WeaponProjectileAttackDelegate AttackCallback { set; private get; }
    public WeaponProjectileAttackDelegate AttackEndCallback { set; private get; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") && collision.TryGetComponent(out MonsterBase monster))
        {
            AttackCallback?.Invoke(this, monster);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") && collision.TryGetComponent(out MonsterBase monster))
        {
            AttackEndCallback?.Invoke(this, monster);
        }
    }
}
