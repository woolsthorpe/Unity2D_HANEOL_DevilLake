using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireball : MonoBehaviour
{
    Rigidbody2D rb;

    public float moveSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float waitTime = 2f; // 방향 전환 전에 기다릴 시간
    public float jumpForce = 5f;
    public GameObject fireParticlePrefab; // 불길 이펙트 프리팹
    public int poolSize = 10; // 객체 풀의 크기
    public float particleLifetime = 2f; // 파티클 생존 시간

    private Queue<GameObject> particlePool;
    private bool isWaiting = false;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 객체 풀 초기화
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
            // 레이캐스트를 시각화
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

        // 레이캐스트를 시각화
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

    private void Fire()
    {
        GameObject particle = GetPooledObject();
        if (particle != null)
        {
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.SetActive(true);
            StartCoroutine(DestroyAfterTime(particle, particleLifetime)); // 2초 후에 비활성화
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
            // 객체 풀이 비어 있으면 새로운 객체 생성
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
        // 일정 시간 동안 대기
        yield return new WaitForSeconds(delay);

        // 파티클을 비활성화하고 객체 풀로 다시 추가
        ReturnToPool(obj);
    }
    public void Explode()
    {

    }
}
