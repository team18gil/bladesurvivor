using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDefaultFaster : MonsterDefault
{
    protected override float FollowSpeed => base.FollowSpeed * 2f;
}
