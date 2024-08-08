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
    public float groundCheckDistance = 1.5f; // 낭떠러지 감지 거리
    public Vector2 detectionBoxSize = new Vector2(10f, 5f); // 오버랩 박스 크기
    public Vector2 attackDetectionBoxSize = new Vector2(5f, 4f); // 공격 감지 오버랩 박스 크기
    public float minChaseDistance = 2f; // 플레이어와의 최소 거리

    private bool isWaiting = false;
    private bool movingRight = false;
    private bool isDead = false;
    private bool isChasing = false;
    private bool isAttacking = false;

    private Transform player;

    public float attackDelay = 1.5f; // 공격 대기 시간
    public float chargeDuration = 0.5f; // 돌진 시간
    public float chargeSpeed = 8f; // 돌진 속도
    public float attackCooldown = 5f; // 공격 후 대기 시간

    private float lastAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어를 찾음
        StartCoroutine(PerformActionRoutine()); // 코루틴 시작
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            DetectPlayer(); // 플레이어 감지

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
            moveSpeed = 3.5f; // 추적 시 이동 속도 상승
        }
        else
        {
            isChasing = false;
            moveSpeed = 2f; // 추적 끝나면 이동 속도 복귀
        }

        // 공격 패턴 감지
        Collider2D attackHit = Physics2D.OverlapBox(transform.position, attackDetectionBoxSize, 0);
        if (attackHit != null && attackHit.CompareTag("Player") && Time.time - lastAttackTime > attackCooldown)
        {
            StartCoroutine(AttackPattern()); // 공격 패턴 시작
        }
    }

    private void Patrol()
    {
        // 레이캐스트를 시각화
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

        // 레이캐스트로 낭떠러지 감지
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        if (groundInfo.collider != true || distanceToPlayer < minChaseDistance)
        {
            rb.velocity = Vector2.zero; // 낭떠러지거나 너무 가까우면 멈춤
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed * direction.x, rb.velocity.y); // 플레이어를 향해 이동

            // 방향 전환 (플레이어가 왼쪽에 있으면 왼쪽, 오른쪽에 있으면 오른쪽으로)
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

            yield return new WaitForSeconds(2f); // 2초 대기 후 다음 행동 실행
        }
    }

    private IEnumerator ChangeDirection()
    {
        isWaiting = true;
        rb.velocity = new Vector2(0, rb.velocity.y); // 이동을 멈춤
        yield return new WaitForSeconds(waitTime); // 일정 시간 동안 기다림

        ChangeDirectionWithoutWait();

        isWaiting = false;
    }

    private void ChangeDirectionWithoutWait()
    {
        // 적의 위치를 약간 이동시켜 발판 끝에서 벗어나게 함
        float adjustment = movingRight ? -0.2f : 0.2f;
        transform.position = new Vector2(transform.position.x + adjustment, transform.position.y);

        // 방향 전환
        movingRight = !movingRight;
        transform.eulerAngles = new Vector3(0, movingRight ? 180 : 0, 0);
    }

    private IEnumerator AttackPattern()
    {
        if (isAttacking) yield break; // 이미 공격 중이면 코루틴 종료
        isAttacking = true;
        lastAttackTime = Time.time; // 마지막 공격 시간 기록

        // 공격 대기 시간
        rb.velocity = Vector2.zero; // 공격 전 멈춤
        yield return new WaitForSeconds(attackDelay);

        // 랜덤으로 공격 패턴 선택
        bool performDashAttack = Random.value > 0.5f;

        if (performDashAttack)
        {
            // 두 번째 공격 패턴: 돌진
            Debug.Log("돌진 시작");
            float startTime = Time.time;
            Vector2 originalPosition = transform.position;
            while (Time.time - startTime < chargeDuration)
            {
                rb.velocity = new Vector2((movingRight ? 1 : -1) * chargeSpeed, rb.velocity.y);
                yield return null;
            }
            rb.velocity = Vector2.zero; // 돌진 후 멈춤
        }
        else
        {
            // 첫 번째 공격 패턴: 검 휘두르기
            Debug.Log("검을 휘두름");
            // (여기에 검을 휘두르는 로직 추가)

            // 플레이어가 맞았는지 체크 (여기서는 예시로 가정)
            bool playerHit = Random.value > 0.5f; // 임의로 플레이어가 맞았다고 가정
            if (!playerHit)
            {
                yield return new WaitForSeconds(0.2f); // 두 번째 공격 대기 시간
                Debug.Log("검기를 발사함");
                // (여기에 검기를 발사하는 로직 추가)
            }
        }

        // 공격이 끝난 후 공격 상태 해제
        isAttacking = false;
    }

    // Gizmos를 사용하여 OverlapBox의 범위를 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, attackDetectionBoxSize);
    }
}
