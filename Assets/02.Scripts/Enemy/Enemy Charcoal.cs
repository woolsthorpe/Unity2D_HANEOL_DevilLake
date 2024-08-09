using System;
using System.Collections;
using UnityEngine;

public class EnemyCharcoal : MonoBehaviour, IDamageable
{
    Rigidbody2D rb;

    public float moveSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float waitTime = 2f; // ���� ��ȯ ���� ��ٸ� �ð�
    public float explosionRadius = 3f;
    public float explosionDelay = 2.5f;
    public float knockbackForce = 4f;
    public GameObject exprosionParticlePrefab;

    [Header("테스트용 체력 변수")] 
    public float maxHP;
    private float _currentHP;

    [Header("테스트용 공격력 변수")] 
    public float damage;

    private bool isWaiting = false;
    private bool movingRight = true;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DelayedExplosion(10f)); // ������ �� 10�� �� ����
        
        // 체력 초기화
        _currentHP = maxHP;
    }

    void FixedUpdate()
    {
        if (!isWaiting && !isDead)
        {
            // ����ĳ��Ʈ�� �ð�ȭ
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
        rb.velocity = new Vector2(0, rb.velocity.y); // �̵��� ����
        yield return new WaitForSeconds(waitTime); // ���� �ð� ���� ��ٸ�

        // ���� ��ġ�� �ణ �̵����� ���� ������ ����� ��
        float adjustment = movingRight ? 0.2f : -0.2f;
        transform.position = new Vector2(transform.position.x + adjustment, transform.position.y);

        // ���� ��ȯ
        movingRight = !movingRight;
        transform.eulerAngles = new Vector3(0, movingRight ? 0 : 180, 0);

        isWaiting = false;
    }

    private IEnumerator DelayedExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDead) // ���� ���� �ʾҴٸ�
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
        // ��ȭ �� 2.5�� �� �ݰ� 3m�� ����
        yield return new WaitForSeconds(explosionDelay);

        GameObject exprosion = Instantiate(exprosionParticlePrefab, transform.position, Quaternion.identity);

        // 1�� �Ŀ� particle�� ����
        Destroy(exprosion, 1f);


        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Body"))
            {
                if (hitCollider.gameObject.GetComponent<Body>().parasiticPlayer)
                {
                    // ���⼭ ���� ȿ���� ������ ����         
                    hitCollider.gameObject.TryGetComponent(out IDamageable damageable);
                    Vector2 hitDirection = (hitCollider.gameObject.transform.position - hitCollider.transform.position).normalized;
                    Debug.Log(damageable);
                    damageable.TakeDamage(damage, true, hitDirection, knockbackForce);
                }
            }
        }
        // �� ������Ʈ�� �ı�
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)    // �÷��̾ ���� ��
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
            rb.velocity = new Vector2(-attackDirection.x * knockbackForce, rb.velocity.y); // �˹� ����
            StartCoroutine(Explode());
        }
    }

    public void TakeDamage(float amount, bool damageReduction = true, Vector2 hitDirection = new Vector2(),
        float knockbackForce = 0)
    {
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);    // 넉백
        
        // 대미지 피해 계산
        float damage = amount;
        _currentHP -= damage;
        
        if (_currentHP <= 0f)
        {
            // 사망 로직 작성
            Debug.Log("적 사망");
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
