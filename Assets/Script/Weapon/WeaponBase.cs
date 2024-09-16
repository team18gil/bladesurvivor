using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    protected virtual float Damage => 0f;
    protected virtual float DamageInterval => 9999f;

    [SerializeField] private List<WeaponProjectileDefault> projectiles;

    private void Awake()
    {
        foreach(var projectile in projectiles)
        {
            InitializeProjectile(projectile);
        }
    }

    /// <summary>
    /// seperate function to be used on weapon making projectile during gameplay
    /// </summary>
    protected void InitializeProjectile(WeaponProjectileDefault projectile)
    {
        projectile.AttackCallback = OnProjectileAttack;
        projectile.AttackEndCallback = OnProjectileAttackEnd;
    }

    protected virtual void OnProjectileAttack(WeaponProjectileDefault projectile, MonsterBase monster)
    {
        monster.HitStart(projectile, Damage, DamageInterval);
    }

    protected virtual void OnProjectileAttackEnd(WeaponProjectileDefault projectile, MonsterBase monster)
    {
        monster.HitEnd(projectile);
    }
}
