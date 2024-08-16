using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Charcoalsc : Enemy
{
    public bool falling; // 추락 중 여부
    bool firing; // 점화 중

    public GameObject Legs; // 사망 시 떨어트리는 발
    public GameObject Explode; // 폭발 이펙트 & 공격 범위
    public float rayDistance;
    public LayerMask wallLayer;

    void Start()
    {
        TryGetComponent(out rb);
        TryGetComponent(out sr);
        TryGetComponent(out anim);
        TryGetComponent(out tr);
        
        StartCoroutine("Reding", 10f); // 10초 후 점화 <- 이건 써야 함
    }

    public override void Die()
    {
        Deading();
    }

    public void Deading() // 숯덩이 사망 시 발동 !
    {
        moveSpeed = 0f;
        anim.SetBool("isDead", true);
        rb.freezeRotation = false;

        for (int i = 0; i < 2; i++) {
            GameObject go = Instantiate(Legs);
            go.transform.localPosition = new Vector3(tr.position.x, tr.position.y, 1);
            Destroy(go, 3f);
        }

        if (!firing)
            StartCoroutine("Reding", 0f);
    }

    IEnumerator Reding(float time) // 점화 : 붉어지는 중, 2초 후 폭발
    {
        yield return new WaitForSeconds(time);
        if (!firing) {
            firing = true;
            for (float i = 0; i < 1f; i += 0.01f)
            {
                sr.color = new Color(1f, 1f - i, 1f - i);
                yield return new WaitForSeconds(0.02f);
            }
            GameObject go2 = Instantiate(Explode); 
            go2.transform.localPosition = new Vector3(tr.position.x, tr.position.y, 1);
            Destroy(go2, 5f);
            Destroy(gameObject);
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


        if(Physics2D.Raycast(transform.localPosition,Vector2.right*moveSpeed,rayDistance,wallLayer))
        {
            moveSpeed *= -1;
        }
       

    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.localPosition,transform.localPosition+Vector3.right*Mathf.Sign(moveSpeed)* rayDistance);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 벽에 부딪혔을 때 이동 방향 교체 -> 일단 벽을 차별화할게 Spr 의 Order in layer 만 보여서
        // 그걸로 구분을 지어 놨습니다. 다른 방안 있으면 교체 부탁드립니다.
        //if (other.gameObject.GetComponent<SpriteRenderer>().sortingOrder == -9
        //    || (other.gameObject.GetComponent<SpriteRenderer>().sortingOrder == 1 &&
        //    other.gameObject.layer == 9))
        //if (other.gameObject.GetComponent<SpriteRenderer>().sortingOrder == -9)
        //{
        //    moveSpeed *= -1;
        //}
    }
}
