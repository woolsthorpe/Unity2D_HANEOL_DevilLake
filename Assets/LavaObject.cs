using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;

public class LavaObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damageAmount=8;
    [SerializeField] private float knockBackAmount;

    [Space(10)]
    [Header("RiseUp Settings")]

    [SerializeField] private float riseUpTime;
    [SerializeField] private AnimationCurve riseUpCurve;
    [SerializeField] private Vector3 targetPos;
    [Space(10)]
    [Header("movement Settings")]
    [SerializeField] private float[] speed;
    [SerializeField] private float triggerPosX=-29f;
    [SerializeField] private float addDistance;
    [SerializeField] private Transform[] frontSprite;
    [SerializeField] private Transform[] middleSprite;
    [SerializeField] private Transform[] backSprite;

    private float leftPosX = 0f;
    private float rightPosX = 0f;
    private float xScreenHalfSize;
    private float yScreenHalfSize;

    void Start()
    {
        gameObject.SetActive(false);

        yScreenHalfSize = Camera.main.orthographicSize;
        xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;

        leftPosX = -(xScreenHalfSize * 2);
        rightPosX = xScreenHalfSize * 2 * frontSprite.Length;
    }

    void Update()
    {
        for (int i = 0; i < frontSprite.Length; i++)
        {
            frontSprite[i].position += new Vector3(-speed[0], 0, 0) * Time.deltaTime;
            middleSprite[i].position += new Vector3(-speed[1], 0, 0) * Time.deltaTime;
            backSprite[i].position += new Vector3(-speed[2], 0, 0) * Time.deltaTime;

            if (frontSprite[i].position.x < triggerPosX)
            {
                Vector3 nextPos = frontSprite[i].position;
                nextPos = new Vector3(nextPos.x + addDistance, nextPos.y, nextPos.z);
                frontSprite[i].position = nextPos;
            }
            if (middleSprite[i].position.x < triggerPosX)
            {
                Vector3 nextPos = middleSprite[i].position;
                nextPos = new Vector3(nextPos.x + addDistance, nextPos.y, nextPos.z);
                middleSprite[i].position = nextPos;
            }
            if (backSprite[i].position.x < triggerPosX)
            {
                Vector3 nextPos = backSprite[i].position;
                nextPos = new Vector3(nextPos.x + addDistance, nextPos.y, nextPos.z);
                backSprite[i].position = nextPos;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Body"))
        {
            // collision.TryGetComponent(out IDamageable damageable);
            // damageable.TakeDamage(damageAmount, false, Vector2.up, knockBackAmount);


            IDamageable damageable = collision.GetComponent<Body>();

            damageable.TakeDamage(damageAmount, false, Vector2.up, knockBackAmount);

        }

        if (collision.CompareTag("Enemy"))
        {
            // collision.TryGetComponent(out IDamageable damageable);
            // damageable.TakeDamage(damageAmount, false, Vector2.up, knockBackAmount);


            IDamageable damageable = collision.GetComponent<Body>();

            damageable.TakeDamage(damageAmount*2, false, Vector2.up, 0);

        }
    }


    public void LavaRisehUp()
    {
       // Debug.Log("asdasd");
        gameObject.SetActive(true);

        StartCoroutine(LavaSmoothUp(riseUpTime,targetPos));
    }

    IEnumerator LavaSmoothUp(float targetTime, Vector3 targetPos)
    {
        
        float currentTime=0f;
        float percent = 0f;
        Vector3 currentPos = this.transform.position;
        Vector3 vec=currentPos;

       HUDController.instance.ShakeCamera(0.8f,targetTime);

        while(percent<1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / targetTime;
            vec.y = Mathf.Lerp(currentPos.y, targetPos.y, riseUpCurve.Evaluate(percent));
            this.transform.position = vec;
            

            yield return null;

        }
        this.transform.position = targetPos;
    }
}


