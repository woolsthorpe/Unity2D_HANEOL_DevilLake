using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GroundEnemyAI : MonoBehaviour
{
    public Transform target; // �÷��̾��� Transform
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float baseJumpHeight = 5f; // �⺻ ���� ����
    public LayerMask groundLayer; // Ground ���̾�
    public LayerMask obstacleLayer; // ��ֹ� ���̾�

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

        // �̵�
        rb.velocity = force;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // ���� ���� �߰�
        if (ShouldJump())
        {
            Jump();
        }
    }

    void Jump()
    {
        // ��ǥ ������ ���̿� ���� ���� ���� ���
        float jumpHeight = CalculateJumpHeight(path.vectorPath[currentWaypoint]);
        rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
    }

    bool ShouldJump()
    {
        // ���� ��ġ�� ��ǥ ���� ���̿� ��ֹ��� �ִ��� Ȯ��
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position);
        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, direction.magnitude, obstacleLayer);

        if (isGrounded && hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            return true;
        }

        // ��ǥ ������ ���� �ִ� ���
        if (isGrounded && path.vectorPath[currentWaypoint].y > rb.position.y + 0.5f)
        {
            return true;
        }

        return false;
    }

    float CalculateJumpHeight(Vector3 targetPosition)
    {
        // ��ǥ �������� ���� ���̿� ���� ���� ���� ���
        float heightDifference = targetPosition.y - rb.position.y;
        return baseJumpHeight + heightDifference;
    }

    void Update()
    {
        // ���� ���� �ִ��� Ȯ��
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer);
    }
}
