using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDefault : MonsterBase
{
    protected override float MaxHP => 10f;
    protected override float Damage => 1f;
    protected override float DamageInterval => 0.33f;

    protected override void SetInitialPosition()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector3 dest = new(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);
        dest *= 8f; // radius
        dest += characterObject.transform.localPosition; // start from character position

        transform.localPosition = dest;
    }

    protected virtual float FollowSpeed => 1f;

    protected override IEnumerator MoveCoroutine()
    {
        while(true)
        {
            var delta = characterObject.transform.localPosition - transform.localPosition;
            delta = FollowSpeed * Time.deltaTime * delta.normalized;

            transform.localPosition = transform.localPosition + delta;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }
    }
}
