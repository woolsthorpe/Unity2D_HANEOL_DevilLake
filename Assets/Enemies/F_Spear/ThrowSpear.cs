using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpear : MonoBehaviour
{
    [Header("Stats")] 
    public float damage;
    public float knockbackForce;
    
    Rigidbody2D rgd;

    bool ismove = true;

    void Start()
    {
        rgd = GetComponent<Rigidbody2D>();
        ismove = true;
        rgd.gravityScale = 0f;
    }

    void Update()
    {
        if (ismove) {
            transform.Translate(0f, -12f * Time.deltaTime, 0f);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
        {
            rgd.gravityScale = 1f;
            ismove = false;
            GetComponent<CapsuleCollider2D>().isTrigger = false;
            transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            Destroy(gameObject, 5f);
        }
        
        if (other.CompareTag("Body")) 
        {
            if (ismove)
            {
                // 피격 로직 작성
                IDamageable damageable = other.GetComponent<Body>();
                
                Vector2 hitDirection = (other.gameObject.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, true, hitDirection, knockbackForce);
            }
        }
    }
}
