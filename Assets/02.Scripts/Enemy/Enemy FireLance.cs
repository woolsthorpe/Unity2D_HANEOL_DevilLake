using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyFireLance : MonoBehaviour
{
    public Transform target;
    public float speed = 200f;
    public float thrustSpeed = 500f; // 찌르기 속도
    public float throwSpeed = 300f; // 창 던지기 속도
    public float nextWaypointDistance = 3f;
    public float stopDistance = 2f; // 목표물과의 최소 거리

    public float thrustWaitTime = 0.5f; // 찌르기 전에 기다리는 시간
    public float throwWaitTime = 0.7f; // 창 던지기 전에 기다리는 시간
    public float actionCooldown = 3f; // 행동 후 대기 시간

    public Transform enemyGFX;
    public GameObject lancePrefab; // 창 프리팹
    public Transform lanceSpawnPoint; // 창 생성 위치

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private bool actionChosen = false;
    private bool canUseSkills = true; // 스킬 사용 가능 여부

    private Seeker seeker;
    private Rigidbody2D rb;
    private bool isWaiting = false;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void Update()
    {
        if (path == null)
            return;

        float distanceToTarget = Vector2.Distance(rb.position, target.position);

        // 목표물과의 거리가 stopDistance 이상일 때만 이동
        if (distanceToTarget > stopDistance && !isWaiting)
        {
            actionChosen = false; // 사거리를 벗어났을 때 행동 초기화

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (force.x >= 0.01f)
            {
                enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (force.x <= -0.01f)
            {
                enemyGFX.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else
        {
            rb.velocity = Vector2.zero; // 멈춤

            if (!actionChosen && canUseSkills)
            {
                StartCoroutine(ChooseAction());
                actionChosen = true; // 행동 선택 후 플래그 설정
                 // 스킬 사용 불가능 상태로 설정
                canUseSkills = false;
            }
        }
    }

    IEnumerator ChooseAction()
    {
        // 플레이어가 적 바로 아래에 있을 때
        if (Mathf.Abs(target.position.x - transform.position.x) < 1f && target.position.y < transform.position.y)
        {
            PerformDownwardStrike(); // 행동 패턴 1: 내려찍기
        }
        else
        {
            // 찌르기 또는 창 던지기 중 랜덤 선택
            int action = Random.Range(1, 3);

            if (action == 1)
            {
                yield return PerformThrust(); // 행동 패턴 2: 찌르기
            }
            else if (action == 2)
            {
                yield return ThrowLance(); // 행동 패턴 3: 창 던지기
            }
        }


        // 스킬 사용 후 대기 시간 동안 대기
        yield return new WaitForSeconds(actionCooldown);
        // 스킬 사용 가능 상태로 복원
        canUseSkills = true;
    }

    void PerformDownwardStrike()
    {
        Debug.Log("Preparing to perform downward strike");

        StartCoroutine(PerformDownwardStrikeCoroutine());
    }

    IEnumerator PerformDownwardStrikeCoroutine()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1f);
        isWaiting = false;

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = new Vector2(transform.position.x, target.position.y - 2f); // 목표 위치 설정 (y좌표를 기준으로 2단계 아래로 이동)

        float strikeTime = 0.5f; // 빠르게 이동하는 시간
        float elapsedTime = 0f;

        while (elapsedTime < strikeTime)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, (elapsedTime / strikeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        Debug.Log("Downward strike complete");
    }

    IEnumerator PerformThrust()
    {
        Debug.Log("Preparing to thrust");
        rb.velocity = Vector2.zero; // 이동 멈추기

        // 찌르기 전에 기다리는 시간
        isWaiting = true;
        yield return new WaitForSeconds(thrustWaitTime);
        isWaiting = false;

        Debug.Log("Performing thrust");

        // 현재 위치와 타겟 위치를 Vector2로 변환
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = (Vector2)target.position;

        // 타겟 방향으로 이동하기 위한 벡터 계산
        Vector2 directionToTarget = (targetPosition - startPosition).normalized;

        // 타겟 위치보다 깊이 이동하기 위한 거리 설정
        float thrustDistance = 2f; // 타겟 위치보다 더 깊이 찌를 거리
        Vector2 finalTargetPosition = targetPosition + directionToTarget * thrustDistance;

        float thrustTime = 0.5f; // 빠르게 이동하는 시간
        float elapsedTime = 0f;

        while (elapsedTime < thrustTime)
        {
            transform.position = Vector2.Lerp(startPosition, finalTargetPosition, elapsedTime / thrustTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동 완료 후 위치 고정
        transform.position = finalTargetPosition;

        Debug.Log("Thrust complete");
    }



    IEnumerator ThrowLance()
    {
        Debug.Log("Preparing to throw lance");
        rb.velocity = Vector2.zero;
        // 창 던지기 전에 기다리는 시간
        isWaiting = true;
        yield return new WaitForSeconds(throwWaitTime);
        isWaiting = false;

        Debug.Log("Throwing lance");
        GameObject lance = Instantiate(lancePrefab, lanceSpawnPoint.position, lanceSpawnPoint.rotation);
        Rigidbody2D lanceRb = lance.GetComponent<Rigidbody2D>();
        Vector2 direction = (target.position - lanceSpawnPoint.position).normalized;
        lanceRb.velocity = direction * throwSpeed; // 창 던지기 속도
    }
}
