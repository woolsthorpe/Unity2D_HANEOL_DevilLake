using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyFireLance : MonoBehaviour
{
    public Transform target;
    public float speed = 200f;
    public float thrustSpeed = 500f; // ��� �ӵ�
    public float throwSpeed = 300f; // â ������ �ӵ�
    public float nextWaypointDistance = 3f;
    public float stopDistance = 2f; // ��ǥ������ �ּ� �Ÿ�

    public float thrustWaitTime = 0.5f; // ��� ���� ��ٸ��� �ð�
    public float throwWaitTime = 0.7f; // â ������ ���� ��ٸ��� �ð�
    public float actionCooldown = 3f; // �ൿ �� ��� �ð�

    public Transform enemyGFX;
    public GameObject lancePrefab; // â ������
    public Transform lanceSpawnPoint; // â ���� ��ġ

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private bool actionChosen = false;
    private bool canUseSkills = true; // ��ų ��� ���� ����

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

        // ��ǥ������ �Ÿ��� stopDistance �̻��� ���� �̵�
        if (distanceToTarget > stopDistance && !isWaiting)
        {
            actionChosen = false; // ��Ÿ��� ����� �� �ൿ �ʱ�ȭ

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
            rb.velocity = Vector2.zero; // ����

            if (!actionChosen && canUseSkills)
            {
                StartCoroutine(ChooseAction());
                actionChosen = true; // �ൿ ���� �� �÷��� ����
                 // ��ų ��� �Ұ��� ���·� ����
                canUseSkills = false;
            }
        }
    }

    IEnumerator ChooseAction()
    {
        // �÷��̾ �� �ٷ� �Ʒ��� ���� ��
        if (Mathf.Abs(target.position.x - transform.position.x) < 1f && target.position.y < transform.position.y)
        {
            PerformDownwardStrike(); // �ൿ ���� 1: �������
        }
        else
        {
            // ��� �Ǵ� â ������ �� ���� ����
            int action = Random.Range(1, 3);

            if (action == 1)
            {
                yield return PerformThrust(); // �ൿ ���� 2: ���
            }
            else if (action == 2)
            {
                yield return ThrowLance(); // �ൿ ���� 3: â ������
            }
        }


        // ��ų ��� �� ��� �ð� ���� ���
        yield return new WaitForSeconds(actionCooldown);
        // ��ų ��� ���� ���·� ����
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
        Vector2 targetPosition = new Vector2(transform.position.x, target.position.y - 2f); // ��ǥ ��ġ ���� (y��ǥ�� �������� 2�ܰ� �Ʒ��� �̵�)

        float strikeTime = 0.5f; // ������ �̵��ϴ� �ð�
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
        rb.velocity = Vector2.zero; // �̵� ���߱�

        // ��� ���� ��ٸ��� �ð�
        isWaiting = true;
        yield return new WaitForSeconds(thrustWaitTime);
        isWaiting = false;

        Debug.Log("Performing thrust");

        // ���� ��ġ�� Ÿ�� ��ġ�� Vector2�� ��ȯ
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = (Vector2)target.position;

        // Ÿ�� �������� �̵��ϱ� ���� ���� ���
        Vector2 directionToTarget = (targetPosition - startPosition).normalized;

        // Ÿ�� ��ġ���� ���� �̵��ϱ� ���� �Ÿ� ����
        float thrustDistance = 2f; // Ÿ�� ��ġ���� �� ���� � �Ÿ�
        Vector2 finalTargetPosition = targetPosition + directionToTarget * thrustDistance;

        float thrustTime = 0.5f; // ������ �̵��ϴ� �ð�
        float elapsedTime = 0f;

        while (elapsedTime < thrustTime)
        {
            transform.position = Vector2.Lerp(startPosition, finalTargetPosition, elapsedTime / thrustTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �̵� �Ϸ� �� ��ġ ����
        transform.position = finalTargetPosition;

        Debug.Log("Thrust complete");
    }



    IEnumerator ThrowLance()
    {
        Debug.Log("Preparing to throw lance");
        rb.velocity = Vector2.zero;
        // â ������ ���� ��ٸ��� �ð�
        isWaiting = true;
        yield return new WaitForSeconds(throwWaitTime);
        isWaiting = false;

        Debug.Log("Throwing lance");
        GameObject lance = Instantiate(lancePrefab, lanceSpawnPoint.position, lanceSpawnPoint.rotation);
        Rigidbody2D lanceRb = lance.GetComponent<Rigidbody2D>();
        Vector2 direction = (target.position - lanceSpawnPoint.position).normalized;
        lanceRb.velocity = direction * throwSpeed; // â ������ �ӵ�
    }
}
