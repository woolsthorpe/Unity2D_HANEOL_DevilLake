using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float amount, bool damageReduction = true, Vector2 hitDirection = new Vector2(), float knockbackForce = 0f);
}
