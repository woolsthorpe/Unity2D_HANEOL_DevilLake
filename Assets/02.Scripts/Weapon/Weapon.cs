using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    public string weaponName;
    public string weaponDescription;
    // 공격력 등등 스탯 추가하기.
    public WeaponSkill weaponSkill;     // 혈기술

    public abstract void UseWeaponSkill();
}
