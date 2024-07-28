using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/TestWeapon")]
public class TestWeapon : Weapon
{
    public override void UseWeaponSkill()
    {
        if (weaponSkill != null)
        {
            weaponSkill.Use();
        }
    }
}
