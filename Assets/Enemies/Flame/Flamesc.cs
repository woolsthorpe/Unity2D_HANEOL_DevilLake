using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Flamesc : Enemy
{
    public bool falling; // 추락 중 여부
    bool firing; // 점화 중

    bool isdead; // 사망

    public GameObject Explode; // 폭발 이펙트 & 공격 범위
    public GameObject Firewalk; // 불길
    public LayerMask wallLayer;
    public float rayDistance;
    void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out sr);
        TryGetComponent(out anim);
        TryGetComponent(out tr);

        Invoke("Deading", 12f);
    }

    public override void Die()
    {
        Deading();
    }

    public void Deading() // 사망 시 발동 !
    {
        moveSpeed = 0f;
        anim.SetBool("isDead", true);
        rb.freezeRotation = false;

        StartCoroutine("Reding");
        isdead = true;
    }

    IEnumerator Reding() {
        for (float i = 0; i < 1f; i += 0.01f)
        {
            sr.color = new Color(1f, 1f - i, 1f - i);
            yield return new WaitForSeconds(0.02f);
        }
    }

    void Update()
    {
        rb.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
        if (rb.velocity.x > 3f)
            rb.velocity = new Vector2(3f, rb.velocity.y);
        else if (rb.velocity.x < -3f)
            rb.velocity = new Vector2(-3f, rb.velocity.y);

        if (moveSpeed < 0)
            sr.flipX = false;
        else
            sr.flipX = true;

        if (rb.velocity.y < 0)
            falling = true;
        else
            falling = false;

        if (falling)
        {
            anim.SetBool("isFall", true);
        }
        else
            anim.SetBool("isFall", false);

        if (Physics2D.Raycast(transform.localPosition, Vector2.right * moveSpeed, rayDistance, wallLayer))
        {
            moveSpeed *= -1;
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        // 바닥에 접촉 시 바로 점프
        if (other.gameObject.layer == 9 || other.gameObject.GetComponent<SpriteRenderer>().sortingOrder == 1)
        {
            if (!falling && !isdead && rb.velocity.y == 0) {
                rb.AddForce(Vector2.up * 4f, ForceMode2D.Impulse);

                GameObject go2 = Instantiate(Firewalk);
                go2.transform.localPosition = new Vector3(tr.position.x, tr.position.y - 0.65f, -1);
                Destroy(go2, 5f);
            }
        }

        if (other.gameObject.GetComponent<SpriteRenderer>().sortingOrder == -9
            || (other.gameObject.GetComponent<SpriteRenderer>().sortingOrder == 1 &&
            other.gameObject.layer == 9))
        {
            moveSpeed *= -1;
        }

        if (isdead) {
            GameObject go2 = Instantiate(Explode);
            go2.transform.localPosition = new Vector3(tr.position.x, tr.position.y, -1);
            Destroy(go2, 5f);
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.localPosition, transform.localPosition + Vector3.right * Mathf.Sign(moveSpeed) * rayDistance);
    }
}
