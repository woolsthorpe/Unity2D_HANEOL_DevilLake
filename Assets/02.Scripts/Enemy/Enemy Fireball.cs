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
    public GameObject exprosionParticlePrefab;
    public GameObject bigFireParticlePrefab;
    public int poolSize = 10; // 객체 풀의 크기
    public float particleLifetime = 2f; // 파티클 생존 시간
    public float explosionRadius = 1.5f;

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

        if (!isWaiting)
        {
            // 레이캐스트를 시각화
            Debug.DrawRay(groundCheck.position, Vector2.down * 0.6f, Color.blue);

            RaycastHit2D jumpInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.6f, groundLayer);
            if (jumpInfo.collider == true)
            {
                Fire();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

            //테스트
            if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Explode());
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
            Vector3 fireposition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

            particle.transform.position = fireposition;
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
    private IEnumerator Explode()
    {
        isWaiting = true;
        // 점화 시 2.5초 후 반경 3m의 폭발
        yield return new WaitForSeconds(2.5f);

        Vector3 fireposition2 = new Vector3(transform.position.x , transform.position.y - 0.2f, transform.position.z);

        GameObject exprosion = Instantiate(exprosionParticlePrefab, transform.position, Quaternion.identity);
        GameObject fire = Instantiate(bigFireParticlePrefab, fireposition2, Quaternion.identity);

        // 1초 후에 particle을 삭제
        Destroy(exprosion, 1f);
        Destroy(fire, 1f);


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
}
