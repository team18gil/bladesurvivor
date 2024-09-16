using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSword : WeaponBase
{
    protected override float Damage => 4f;
    protected override float DamageInterval => 0.03f;

    private float rotationSpeed = 120f;

    private void Update()
    {
        float angle = transform.localRotation.eulerAngles.z;
        angle += rotationSpeed * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
