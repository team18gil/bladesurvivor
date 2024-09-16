using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "ScriptableObjects/UserData")]
public class UserData : ScriptableObject
{
    [SerializeField] private float defaultHP = 100;
    public float DefaultHP => defaultHP;
}
