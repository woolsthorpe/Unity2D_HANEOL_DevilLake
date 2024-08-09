using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireball : MonoBehaviour, IDamageable
{
    Rigidbody2D rb;

    public float moveSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float waitTime = 2f; // ���� ��ȯ ���� ��ٸ� �ð�
    public float jumpForce = 5f;
    public GameObject fireParticlePrefab; // �ұ� ����Ʈ ������
    public GameObject exprosionParticlePrefab;
    public GameObject bigFireParticlePrefab;
    public int poolSize = 10; // ��ü Ǯ�� ũ��
    public float particleLifetime = 2f; // ��ƼŬ ���� �ð�
    public float explosionRadius = 1.5f;
    
    [Header("테스트용 체력 코드")] 
    public float maxHP;
    [SerializeField] private float _currentHP;

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
        
        // 체력 초기화
        _currentHP = maxHP;
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

        if (!isWaiting)
        {
            // ����ĳ��Ʈ�� �ð�ȭ
            Debug.DrawRay(groundCheck.position, Vector2.down * 0.6f, Color.blue);

            RaycastHit2D jumpInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.6f, groundLayer);
            if (jumpInfo.collider == true)
            {
                Fire();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

            //�׽�Ʈ
            if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Explode());
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
            Vector3 fireposition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

            particle.transform.position = fireposition;
            particle.transform.rotation = transform.rotation;
            particle.SetActive(true);
            StartCoroutine(DestroyAfterTime(particle, particleLifetime)); // 2�� �Ŀ� ��Ȱ��ȭ
        }
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
    private IEnumerator Explode()
    {
        isWaiting = true;
        // ��ȭ �� 2.5�� �� �ݰ� 3m�� ����
        yield return new WaitForSeconds(2.5f);

        Vector3 fireposition2 = new Vector3(transform.position.x , transform.position.y - 0.2f, transform.position.z);

        GameObject exprosion = Instantiate(exprosionParticlePrefab, transform.position, Quaternion.identity);
        GameObject fire = Instantiate(bigFireParticlePrefab, fireposition2, Quaternion.identity);

        // 1�� �Ŀ� particle�� ����
        Destroy(exprosion, 1f);
        Destroy(fire, 1f);


        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // ���⼭ ���� ȿ���� ������ ����               
            }
        }
        // �� ������Ʈ�� �ı�
        Destroy(gameObject);
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
}
