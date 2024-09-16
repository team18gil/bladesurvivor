using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterObject : MonoBehaviour
{
    public enum ECharacterStatus
    {
        Disappear,
        Alive,
        Dead,
    }

    public enum EWeaponType
    {
        Sword,
    }

    private Dictionary<EWeaponType, WeaponBase> weaponDic = new();

    [SerializeField] private CharacterViewObject characterViewObject;
    [SerializeField] private CharacterFollowingObject characterFollowingObject;

    private new Rigidbody2D rigidbody2D;

    private void Awake()
    {
        if (!TryGetComponent(out rigidbody2D))
        {
            Debug.LogAssertion("Attach Rigidbody2D on CharacterObject");
        }
    }

    public void MakeWeapon(EWeaponType weaponType)
    {
        if (weaponDic.TryGetValue(weaponType, out _))
        {
            Debug.LogWarning($"Weapon already added for {weaponType}");
            return;
        }

        WeaponBase newWeapon = null;
        if (weaponType.Equals(EWeaponType.Sword))
        {
            newWeapon = GameManager.Instance.GameObjectPool.Get<WeaponSword>(characterFollowingObject.transform);
        }

        if (newWeapon is not null)
        {
            weaponDic.Add(weaponType, newWeapon);
        }
    }

    public void SetStatus(ECharacterStatus status)
    {
        if (status.Equals(ECharacterStatus.Disappear))
        {
            characterViewObject.gameObject.SetActive(false);
            characterFollowingObject.gameObject.SetActive(false);
        }
        else
        {
            characterViewObject.gameObject.SetActive(true);
            characterFollowingObject.gameObject.SetActive(true);
        }
    }

    public void LookAt(Vector2 delta)
    {
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void MoveCharacterBy(Vector2 delta)
    {
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        float velocity = 0.1f;
        Vector2 dest = rigidbody2D.position + (delta * velocity);

        rigidbody2D.MovePositionAndRotation(dest, angle);
    }

    public void ClearAllWeapons()
    {
        foreach (var pair in weaponDic)
        {
            GameManager.Instance.GameObjectPool.Release(pair.Value);
        }
        weaponDic.Clear();
    }
}
