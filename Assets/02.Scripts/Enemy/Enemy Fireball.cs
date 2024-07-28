using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireball : MonoBehaviour
{
    Rigidbody2D rb;

    public float moveSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float waitTime = 2f; // ���� ��ȯ ���� ��ٸ� �ð�
    public float jumpForce = 5f;
    public GameObject fireParticlePrefab; // �ұ� ����Ʈ ������
    public int poolSize = 10; // ��ü Ǯ�� ũ��
    public float particleLifetime = 2f; // ��ƼŬ ���� �ð�

    private Queue<GameObject> particlePool;
    private bool isWaiting = false;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ��ü Ǯ �ʱ�ȭ
        particlePool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(fireParticlePrefab);
            obj.SetActive(false);
            particlePool.Enqueue(obj);
        }
    }

    void FixedUpdate()
    {
        if (!isWaiting)
        {
            // ����ĳ��Ʈ�� �ð�ȭ
            Debug.DrawRay(groundCheck.position, Vector2.down * 2f, Color.red);

            RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 2f, groundLayer);
            if (groundInfo.collider == true)
            {
                rb.velocity = new Vector2(moveSpeed * (movingRight ? -1 : 1), rb.velocity.y);
            }
            else
            {
                StartCoroutine(ChangeDirection());
            }
        }

        // ����ĳ��Ʈ�� �ð�ȭ
        Debug.DrawRay(groundCheck.position, Vector2.down * 0.6f, Color.blue);

        RaycastHit2D jumpInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.6f, groundLayer);
        if (jumpInfo.collider == true)
        {
            Fire();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

    private void Fire()
    {
        GameObject particle = GetPooledObject();
        if (particle != null)
        {
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.SetActive(true);
            StartCoroutine(DestroyAfterTime(particle, particleLifetime)); // 2�� �Ŀ� ��Ȱ��ȭ
        }

        Debug.Log("Fire!");
    }

    private GameObject GetPooledObject()
    {
        if (particlePool.Count > 0)
        {
            GameObject obj = particlePool.Dequeue();
            return obj;
        }
        else
        {
            // ��ü Ǯ�� ��� ������ ���ο� ��ü ����
            return Instantiate(fireParticlePrefab);
        }
    }

    private void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        particlePool.Enqueue(obj);
    }

    IEnumerator DestroyAfterTime(GameObject obj, float delay)
    {
        // ���� �ð� ���� ���
        yield return new WaitForSeconds(delay);

        // ��ƼŬ�� ��Ȱ��ȭ�ϰ� ��ü Ǯ�� �ٽ� �߰�
        ReturnToPool(obj);
    }
    public void Explode()
    {

    }
}
