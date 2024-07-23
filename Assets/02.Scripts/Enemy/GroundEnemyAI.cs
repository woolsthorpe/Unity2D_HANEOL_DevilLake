using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GroundEnemyAI : MonoBehaviour
{
    public Transform target; // 플레이어의 Transform
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float baseJumpHeight = 5f; // 기본 점프 높이
    public LayerMask groundLayer; // Ground 레이어
    public LayerMask obstacleLayer; // 장애물 레이어

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private bool isGrounded;

    private Seeker seeker;
    private Rigidbody2D rb;

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

    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

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
        Vector2 force = new Vector2(direction.x * speed * Time.deltaTime, rb.velocity.y);

        // 이동
        rb.velocity = force;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // 점프 로직 추가
        if (ShouldJump())
        {
            Jump();
        }
    }

    void Jump()
    {
        // 목표 지점의 높이에 따라 점프 높이 계산
        float jumpHeight = CalculateJumpHeight(path.vectorPath[currentWaypoint]);
        rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
    }

    bool ShouldJump()
    {
        // 현재 위치와 목표 지점 사이에 장애물이 있는지 확인
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position);
        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, direction.magnitude, obstacleLayer);

        if (isGrounded && hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            return true;
        }

        // 목표 지점이 위에 있는 경우
        if (isGrounded && path.vectorPath[currentWaypoint].y > rb.position.y + 0.5f)
        {
            return true;
        }

        return false;
    }

    float CalculateJumpHeight(Vector3 targetPosition)
    {
        // 목표 지점과의 높이 차이에 따라 점프 높이 계산
        float heightDifference = targetPosition.y - rb.position.y;
        return baseJumpHeight + heightDifference;
    }

    void Update()
    {
        // 적이 땅에 있는지 확인
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer);
    }
}
