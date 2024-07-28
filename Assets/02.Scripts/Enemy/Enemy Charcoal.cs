using System.Collections;
using UnityEngine;

public class EnemyCharcoal : MonoBehaviour
{
    Rigidbody2D rb;

    public float moveSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float waitTime = 2f; // 방향 전환 전에 기다릴 시간
    public float explosionRadius = 3f;
    public float explosionDelay = 2.5f;
    public float knockbackForce = 4f;
    public GameObject exprosionParticlePrefab;


    private bool isWaiting = false;
    private bool movingRight = true;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DelayedExplosion(10f)); // 생성된 후 10초 후 폭발
    }

    void FixedUpdate()
    {
        if (!isWaiting && !isDead)
        {
            // 레이캐스트를 시각화
            Debug.DrawRay(groundCheck.position, Vector2.down * 1.5f, Color.red);

            RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
            if (groundInfo.collider == true)
            {
                rb.velocity = new Vector2(moveSpeed * (movingRight ? -1 : 1), rb.velocity.y);
            }
            else
            {
                StartCoroutine(ChangeDirection());
            }
        }
    }

    private IEnumerator ChangeDirection()
    {
        isWaiting = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // 이동을 멈춤
        yield return new WaitForSeconds(waitTime); // 일정 시간 동안 기다림

        // 적의 위치를 약간 이동시켜 발판 끝에서 벗어나게 함
        float adjustment = movingRight ? 0.2f : -0.2f;
        transform.position = new Vector2(transform.position.x + adjustment, transform.position.y);

        // 방향 전환
        movingRight = !movingRight;
        transform.eulerAngles = new Vector3(0, movingRight ? 0 : 180, 0);

        isWaiting = false;
    }

    private IEnumerator DelayedExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDead) // 아직 죽지 않았다면
        {
            StartCoroutine(Explode());
        }
    }

    public void AttackCharcoal()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        isWaiting = true;
        // 점화 시 2.5초 후 반경 3m의 폭발
        yield return new WaitForSeconds(explosionDelay);

        GameObject exprosion = Instantiate(exprosionParticlePrefab, transform.position, Quaternion.identity);

        // 1초 후에 particle을 삭제
        Destroy(exprosion, 1f);


        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                    // 여기서 폭발 효과와 데미지 적용               
            }
        }
        // 이 오브젝트를 파괴
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)    // 플레이어가 인접 시
    {
        if (!isDead && other.CompareTag("Player"))
        {
            AttackCharcoal();
        }
    }

    public void Die(Vector2 attackDirection)
    {
        if (!isDead)
        {
            isDead = true;
            rb.velocity = new Vector2(-attackDirection.x * knockbackForce, rb.velocity.y); // 넉백 적용
            StartCoroutine(Explode());
        }
    }
}
