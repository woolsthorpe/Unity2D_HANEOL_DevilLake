using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class WeaponSkill : ScriptableObject
{
    public string skillName;
    public string skillDescription;

    public abstract void Use();
}
