using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/WeaponSkill/TestWeaponSkill")]
public class TestWeaponSkill : WeaponSkill
{
    public override void Use()
    {
        Debug.Log("TestWeaponSkill 사용!");
    }
}
