using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float moveSpeed;
    
    protected float curHealth;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Transform tr;

    private void Awake()
    {
        curHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount, bool damageReduction = true, Vector2 hitDirection = new Vector2(),
        float knockbackForce = 0)
    {
        // 넉백
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);   
        
        // 대미지
        curHealth = -amount;
        if (curHealth <= 0f)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        
    }
}
