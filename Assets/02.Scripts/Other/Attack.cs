using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Animator anim;                           // 애니메이터 
    public Collider2D attackCollider;               // 공격 콜라이더
    
    [HideInInspector] public float damage;          // 대미지
    [HideInInspector] public float knockbackForce;  // 강도
    [HideInInspector] public GameObject attacker;   // 공격자

    public void Start()
    {
        // 프리팹 인스펙터에서 컴포넌트 할당하기.
        
        // 기본적으로 인스펙터에서 콜라이더 비활성화 해두기
    }

    public void StartAttack()
    {
        attackCollider.enabled = true;
    }

    public void EndAttack()
    {
        attackCollider.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == attacker) // 공격자는 피격하지 않음
        {
            return;
        }

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Vector2 hitDirection = (other.gameObject.transform.position - attacker.transform.position).normalized;
                damageable.TakeDamage(damage, true, hitDirection, knockbackForce);
                if (other.gameObject.GetComponent<Enemy>().curHealth <= 0f) // 죽이면 흡혈
                {
                    Body body = attacker.gameObject.GetComponent<Body>();
                    Enemy enemy = other.gameObject.GetComponent<Enemy>();
                    
                    body.currentBodyHealth += 
                        enemy.bloodAmount + body.bodyData.extractedBloodAmount;
                    if (body.currentBodyHealth > body.bodyData.maxBodyHealth)
                    {
                        body.currentBodyHealth = body.bodyData.maxBodyHealth;
                    }
                    
                    //UI연동
                    HUDController.instance.ChangeHpBar(body.currentBodyHealth, body.bodyData.maxBodyHealth);
                }
            }
        }
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
