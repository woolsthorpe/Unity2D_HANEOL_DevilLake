using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage;
    public Collider2D attackRange;
    public Animator anim;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Body body)) // 육체이면
        {
            if (body.parasiticPlayer != null) // 플레이어가 현재 기생한 육체인지 검사
            {
                return;
            }
        }
        
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }
    }

    private void OnEnable()
    {
        Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }
    
    
}
