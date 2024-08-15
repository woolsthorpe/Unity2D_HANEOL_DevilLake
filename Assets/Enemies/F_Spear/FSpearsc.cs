using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSpearsc : Enemy
{
    bool isFind; // 플레이어 인식

    public float AttackCooldown;
    bool isAttack;
    int attype;

    public bool isBack;

    [SerializeField] Transform Playertr;

    public GameObject ThrowSpear;

    public GameObject selfGameObject;

    void Start()
    {
        Playertr = GameObject.Find("Player").transform;
    }

    public override void Die()
    {
        base.Die();
        
        Deading();
    }

    public void Deading() // 사망 시 발동 !
    {
        Destroy(selfGameObject); // 자기 객체 삭제 --- (임시)
    }
    
    void Update()
    {
        if (isFind) {
            if (!isAttack) {
                Vector2 vec = Playertr.position - tr.position;
                vec.Normalize();
                if (!isBack)
                    rb.AddForce(vec * 0.01f * moveSpeed, ForceMode2D.Impulse);
                else
                    rb.AddForce(vec * -0.01f * moveSpeed, ForceMode2D.Impulse);
                if (rb.velocity.x > moveSpeed)
                {
                    if (rb.velocity.y > moveSpeed)
                        rb.velocity = new Vector2(moveSpeed, moveSpeed);
                    else if (rb.velocity.y < -moveSpeed)
                        rb.velocity = new Vector2(moveSpeed, -moveSpeed);
                }
                else if (rb.velocity.x < -moveSpeed)
                {
                    if (rb.velocity.y > moveSpeed)
                        rb.velocity = new Vector2(-moveSpeed, moveSpeed);
                    else if (rb.velocity.y < -moveSpeed)
                        rb.velocity = new Vector2(-moveSpeed, -moveSpeed);
                }

                if (tr.position.x > Playertr.position.x)
                    tr.localScale = new Vector2(0.75f, 0.75f);
                else
                    tr.localScale = new Vector2(-0.75f, 0.75f);

                if (AttackCooldown >= 10f) {
                    ChargeAttack(3);
                }
            }
        }

        if (!isAttack)
        {
            AttackCooldown += 1f * Time.deltaTime;
        }
        else {
            if (attype == 1) {
                rb.AddForce(Vector2.right * moveSpeed * tr.localScale.x * -2.5f, ForceMode2D.Impulse);
                if (rb.velocity.x > moveSpeed * 5)
                    rb.velocity = new Vector2(moveSpeed * 5, 0f);
                else if (rb.velocity.x < -moveSpeed * 5)
                    rb.velocity = new Vector2(-moveSpeed * 5, 0f);
            } else if (attype == 2)
            {
                rb.AddForce(Vector2.down * moveSpeed, ForceMode2D.Impulse);
                if (rb.velocity.y < -moveSpeed * 5)
                    rb.velocity = new Vector2(0f, -moveSpeed * 5);
            }
        }
        

    }

    public void ChargeAttack(int type) {
        AttackCooldown = 0f;
        if (type < 3)
            anim.SetBool("isAttack", true);
        attype = type;
        StartCoroutine("CATT", type);
        
    }
    IEnumerator CATT(int ty) {
        if (ty == 1)
        {
            anim.SetTrigger("Hatk");
        }
        else if (ty == 2)
        {
            anim.SetTrigger("Vatk");
        }
        else if (ty == 3)
        {
            anim.SetTrigger("Throw");
        }
        yield return new WaitForSeconds(0.5f);
        if (ty == 3) {
            GameObject go = Instantiate(ThrowSpear);
            go.transform.position = tr.position;
            float angle = Mathf.Atan2(Playertr.position.y - go.transform.position.y,
                Playertr.position.x - go.transform.position.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        } else
            isAttack = true;

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        isAttack = false;
        anim.SetBool("isAttack", false);
        anim.ResetTrigger("Hatk");
        anim.ResetTrigger("Vatk");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player") {
            isFind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isFind = false;
        }
    }
}
