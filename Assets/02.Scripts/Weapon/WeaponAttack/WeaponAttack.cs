using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : ScriptableObject
{
    public string attackName;
    public string attackDescription;

    public abstract void Attack();
}
