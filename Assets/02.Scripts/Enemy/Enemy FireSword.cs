using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireSword : MonoBehaviour
{
    Rigidbody2D rb;

    public float moveSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float waitTime = 2f;
    public float groundCheckDistance = 1.5f; // �������� ���� �Ÿ�
    public Vector2 detectionBoxSize = new Vector2(10f, 5f); // ������ �ڽ� ũ��
    public Vector2 attackDetectionBoxSize = new Vector2(5f, 4f); // ���� ���� ������ �ڽ� ũ��
    public float minChaseDistance = 2f; // �÷��̾���� �ּ� �Ÿ�

    private bool isWaiting = false;
    private bool movingRight = false;
    private bool isDead = false;
    private bool isChasing = false;
    private bool isAttacking = false;

    private Transform player;

    public float attackDelay = 1.5f; // ���� ��� �ð�
    public float chargeDuration = 0.5f; // ���� �ð�
    public float chargeSpeed = 8f; // ���� �ӵ�
    public float attackCooldown = 5f; // ���� �� ��� �ð�

    private float lastAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // �÷��̾ ã��
        StartCoroutine(PerformActionRoutine()); // �ڷ�ƾ ����
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            DetectPlayer(); // �÷��̾� ����

            if (!isWaiting && !isDead)
            {
                if (isChasing)
                {
                    ChasePlayer();
                }
                else
                {
                    Patrol();
                }
            }
        }
    }

    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, detectionBoxSize, 0);
        if (hit != null && hit.CompareTag("Player"))
        {
            isChasing = true;
            moveSpeed = 3.5f; // ���� �� �̵� �ӵ� ���
        }
        else
        {
            isChasing = false;
            moveSpeed = 2f; // ���� ������ �̵� �ӵ� ����
        }

        // ���� ���� ����
        Collider2D attackHit = Physics2D.OverlapBox(transform.position, attackDetectionBoxSize, 0);
        if (attackHit != null && attackHit.CompareTag("Player") && Time.time - lastAttackTime > attackCooldown)
        {
            StartCoroutine(AttackPattern()); // ���� ���� ����
        }
    }

    private void Patrol()
    {
        // ����ĳ��Ʈ�� �ð�ȭ
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        if (groundInfo.collider == true)
        {
            rb.velocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.velocity.y);
        }
        else
        {
            StartCoroutine(ChangeDirection());
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ����ĳ��Ʈ�� �������� ����
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        if (groundInfo.collider != true || distanceToPlayer < minChaseDistance)
        {
            rb.velocity = Vector2.zero; // ���������ų� �ʹ� ������ ����
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed * direction.x, rb.velocity.y); // �÷��̾ ���� �̵�

            // ���� ��ȯ (�÷��̾ ���ʿ� ������ ����, �����ʿ� ������ ����������)
            if ((!movingRight && direction.x > 0) || (movingRight && direction.x < 0))
            {
                ChangeDirectionWithoutWait();
            }
        }
    }

    private IEnumerator PerformActionRoutine()
    {
        while (!isDead && !isChasing)
        {
            int action = Random.Range(0, 3);
            switch (action)
            {
                case 0:
                    isWaiting = true; break;
                case 1:
                    isWaiting = false; break;
                case 2:
                    StartCoroutine(ChangeDirection()); break;
            }

            yield return new WaitForSeconds(2f); // 2�� ��� �� ���� �ൿ ����
        }
    }

    private IEnumerator ChangeDirection()
    {
        isWaiting = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // �̵��� ����
        yield return new WaitForSeconds(waitTime); // ���� �ð� ���� ��ٸ�

        ChangeDirectionWithoutWait();

        isWaiting = false;
    }

    private void ChangeDirectionWithoutWait()
    {
        // ���� ��ġ�� �ణ �̵����� ���� ������ ����� ��
        float adjustment = movingRight ? -0.2f : 0.2f;
        transform.position = new Vector2(transform.position.x + adjustment, transform.position.y);

        // ���� ��ȯ
        movingRight = !movingRight;
        transform.eulerAngles = new Vector3(0, movingRight ? 180 : 0, 0);
    }

    private IEnumerator AttackPattern()
    {
        if (isAttacking) yield break; // �̹� ���� ���̸� �ڷ�ƾ ����
        isAttacking = true;
        lastAttackTime = Time.time; // ������ ���� �ð� ���

        // ���� ��� �ð�
        rb.velocity = Vector2.zero; // ���� �� ����
        yield return new WaitForSeconds(attackDelay);

        // �������� ���� ���� ����
        bool performDashAttack = Random.value > 0.5f;

        if (performDashAttack)
        {
            // �� ��° ���� ����: ����
            Debug.Log("���� ����");
            float startTime = Time.time;
            Vector2 originalPosition = transform.position;
            while (Time.time - startTime < chargeDuration)
            {
                rb.velocity = new Vector2((movingRight ? 1 : -1) * chargeSpeed, rb.velocity.y);
                yield return null;
            }
            rb.velocity = Vector2.zero; // ���� �� ����
        }
        else
        {
            // ù ��° ���� ����: �� �ֵθ���
            Debug.Log("���� �ֵθ�");
            // (���⿡ ���� �ֵθ��� ���� �߰�)

            // �÷��̾ �¾Ҵ��� üũ (���⼭�� ���÷� ����)
            bool playerHit = Random.value > 0.5f; // ���Ƿ� �÷��̾ �¾Ҵٰ� ����
            if (!playerHit)
            {
                yield return new WaitForSeconds(0.2f); // �� ��° ���� ��� �ð�
                Debug.Log("�˱⸦ �߻���");
                // (���⿡ �˱⸦ �߻��ϴ� ���� �߰�)
            }
        }

        // ������ ���� �� ���� ���� ����
        isAttacking = false;
    }

    // Gizmos�� ����Ͽ� OverlapBox�� ������ �ð�ȭ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, attackDetectionBoxSize);
    }
}
