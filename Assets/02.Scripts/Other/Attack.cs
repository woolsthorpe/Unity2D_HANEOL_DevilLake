using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [HideInInspector] public GameObject attacker;
    [HideInInspector] public float damage;
    [HideInInspector] public float knockbackForce;
    public Collider2D attackRange;
    public Animator anim;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == attacker) // 공격자는 피격하지 않음
        {
            return;
        }
        
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            Vector2 hitDirection = (other.gameObject.transform.position - attacker.transform.position).normalized;
            damageable.TakeDamage(damage, true, hitDirection, knockbackForce);
        }
    }

    private void OnEnable()
    {
        Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }
    
    
}
