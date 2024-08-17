using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float moveSpeed;
    public float bloodAmount;
    public float invincibleDuration = 0.35f;        // 무적 시간 
    public GameObject dropBody = null;
    public Sprite dropBodySprite;
    
    [SerializeField] public float curHealth;
    protected bool isdead;
    protected bool isInvincible = false;
    
    [Header("Components")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator anim;
    public Transform tr;

    private void Awake()
    {
        curHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount, bool damageReduction = true, Vector2 hitDirection = new Vector2(),
        float knockbackForce = 0)
    {
        if (isdead)
        {
            return;
        }

        if (isInvincible)
        {
            return;
        }

        HUDController.instance.ShakeCamera(2.5f, 1f);
        HUDController.instance.TimeStop(0.05f);


        // 무적 시간 적용
        StartCoroutine(InvincibilityCoroutine());
        
        // 넉백
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);   
        
        // 대미지
        curHealth -= amount;
        Debug.Log($"받은 대미지 : {amount}\n" +
                  $"남은 체력 : {curHealth}");

       
        if (curHealth <= 0f)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        isdead = true;
        Drop();
    }

    public virtual void Drop()
    {
        if (dropBody != null)
        {
            // 육체 드롭
            GameObject dropBodyObject = Instantiate(dropBody);
            dropBodyObject.transform.position = transform.position;
            
            // 육체 스프라이트 전달
            Body body = dropBodyObject.GetComponent<Body>();
            body.bodyDropSprite = dropBodySprite;

            // 육체 초기화
            body.Initialize();
        }
    }
    
    // 코루틴을 사용하여 무적 상태 관리
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float timer = 0f;
        float duration = 0.2f;
        
        // 색 변경
        while (timer < duration)
        {
            // 경과 시간에 따른 비율 계산
            float t = timer / duration;

            // 색상 보간
            sr.color = Color.Lerp(Color.white, Color.red, t);

            timer += Time.deltaTime;
            yield return null; // 한 프레임 대기
        }
        // 마지막으로 목표 색상으로 설정
        sr.color = Color.red;

        yield return new WaitForSeconds(invincibleDuration - duration * 2f); // 무적 시간
        
        // 색 변경
        timer = 0f;
        while (timer < duration)
        {
            // 경과 시간에 따른 비율 계산
            float t = timer / duration;

            // 색상 보간
            sr.color = Color.Lerp(Color.red, Color.white, t);

            timer += Time.deltaTime;
            yield return null; // 한 프레임 대기
        }
        // 마지막으로 목표 색상으로 설정
        sr.color = Color.white;
            
        isInvincible = false;
    }
}
