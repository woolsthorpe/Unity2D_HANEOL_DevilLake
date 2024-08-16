using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dandelsc : MonoBehaviour, IDamageable
{
    public float maxhealth;
    public float curhealth;

    public GameObject hearts;

    public GameObject Coals;
    public GameObject Flames;
    public GameObject FSpearman;

    public GameObject SpawnFlame;
    public GameObject SpawnManss;

    public GameObject Arm1;
    public GameObject Arm2;



    GameObject[] Arms = new GameObject[2];

    int armnum;
    int goarms;



    protected bool isdead;
    protected bool isInvincible = false;
    void Start()
    {
        curhealth = maxhealth;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        armnum = Random.Range(1, 3);

        //Appear_Boss(); <- 보스 등장 연출 구문임
    }

    public void Damaged(float dmg) { // 피격 구문
        curhealth -= dmg;
        if (curhealth < 200f && goarms == 0)
        {
            goarms++;
            StartCoroutine("ArmThrow", armnum);
        }
        else if (curhealth < 100f && goarms == 1) {
            goarms++;
            if (armnum == 1)
                armnum = 2;
            else
                armnum = 1;
            StartCoroutine("ArmThrow", armnum);
        }

        if (curhealth <= 0f && goarms == 2) {
            StopAllCoroutines();
            Destroy(Arms[0]); 
            Destroy(Arms[1]);
            StartCoroutine("Dying");
            goarms++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적의 공격에 맞을 시 발동 -> Damaged(dmg) 
        // (심장 Collider 는 충돌 불가 
    }

    public void Appear_Boss() {
        StartCoroutine("Appearing");
    }

    IEnumerator Dying() {
        transform.GetChild(1).GetChild(3).GetComponent<ParticleSystem>().Play();
        transform.GetChild(1).GetChild(6).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(5f);
        transform.GetChild(1).GetChild(7).GetComponent<ParticleSystem>().Play();
        for (int i = 0; i < transform.childCount; i++) {
            if (i == 1)
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
            else 
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    IEnumerator ArmThrow(int types) {
        for (float t = 0f; t < 1f; t += 0.02f)
        {
            transform.GetChild(types + 4).Translate(0f, -Time.deltaTime * (t+1) * 50f, 0f);
            yield return new WaitForSeconds(0.02f);
        }
        transform.GetChild(types + 4).gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        GameObject go;
        if (types == 1)
        {
            go = Instantiate(Arm1);
            Arms[0] = go;
        }
        else {
            go = Instantiate(Arm2); 
            Arms[1] = go;
        }
    }

    IEnumerator Idling() {
        for (float t = 0f; t < 1.5f; t += 0.01f) {
            transform.GetChild(1).Translate(0f, -0.3f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(2).Translate(0f, -0.4f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(3).Translate(-0.3f * Time.deltaTime * (1.5f - t)
                , 0.3f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(4).Translate(0.3f * Time.deltaTime * (1.5f - t)
                , 0.3f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(6).Translate(-0.2f * Time.deltaTime * (1.5f - t)
                , -0.4f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(5).Translate(0.2f * Time.deltaTime * (1.5f - t)
                , -0.4f * Time.deltaTime * (1.5f - t), 0f);
            yield return new WaitForSeconds(0.01f);
        }
        for (float t = 0f; t < 1.5f; t += 0.01f)
        {
            transform.GetChild(1).Translate(0f, 0.3f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(2).Translate(0f, 0.4f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(3).Translate(0.3f * Time.deltaTime * (1.5f - t)
                , -0.3f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(4).Translate(-0.3f * Time.deltaTime * (1.5f - t)
                , -0.3f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(6).Translate(0.2f * Time.deltaTime * (1.5f - t)
                , 0.4f * Time.deltaTime * (1.5f - t), 0f);
            transform.GetChild(5).Translate(-0.2f * Time.deltaTime * (1.5f - t)
                , 0.4f * Time.deltaTime * (1.5f - t), 0f);
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine("Idling");
    }

    IEnumerator SpawnMans()
    {
        int rr = Random.Range(1, 3);

        GameObject go1 = Instantiate(SpawnManss);
        if (rr == 1)
            go1.transform.localPosition = new Vector3(-6f, 200f, -1f);
        else
            go1.transform.localPosition = new Vector3(6f, 200f, -1f);
        Destroy(go1, 6f);

        yield return new WaitForSeconds(3f);
        GameObject go = Instantiate(FSpearman);
        if (rr == 1)
            go.transform.localPosition = new Vector3(-6f, 200f, 0f);
        else
            go.transform.localPosition = new Vector3(6f, 200, 0f);

        yield return new WaitForSeconds(14f);
        StartCoroutine("SpawnMans");
    }

    IEnumerator SpawnCoals()
    {
        for (int i = 0; i < 3; i++) {
            int rr = Random.Range(1, 3);
            int mob = Random.Range(1, 3);

            GameObject go = Instantiate(SpawnFlame);
            if (rr == 1)
                go.transform.localPosition = new Vector3(-29.5f, 198f, -1f);
            else
                go.transform.localPosition = new Vector3(27f, 198f, -1f);
            for (int j = 0; j < 2; j++) {
                if (rr == 1)
                    go.transform.GetChild(j).transform.localScale = new Vector2(1f, 1f);
                else
                    go.transform.GetChild(j).transform.localScale = new Vector2(-1f, 1f);
            }
            Destroy(go, 3f);
            GameObject go1;
            if (mob == 1)
                go1 = Instantiate(Coals);
            else
                go1 = Instantiate(Flames);
            if (rr == 1)
            {
                go1.transform.localPosition = new Vector3(-29.5f, 198f, 0f);
            }
            else
            {
                go1.transform.localPosition = new Vector3(27f, 198f, -1f);

                if (mob == 1)
                    go1.GetComponent<Charcoalsc>().moveSpeed *= -1;
                else
                    go1.GetComponent<Flamesc>().moveSpeed *= -1;
            }

            Vector2 vec;
            if (rr == 1)
                vec = new Vector2(1f, 1f);
            else
                vec = new Vector2(-1f, 1f);

            
            go1.GetComponent<Rigidbody2D>().AddForce(vec * Random.Range(3f, 5f), ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(10f);
        StartCoroutine("SpawnCoals");
    }

    IEnumerator Appearing() {
        yield return new WaitForSeconds(2f);
        transform.GetChild(1).localPosition = new Vector2(transform.GetChild(1).localPosition.x, -10f);
        transform.GetChild(1).gameObject.SetActive(true);
        Vector2 vec2 = new Vector2(0.42f, 6.18f);
        Vector2 vec1 = transform.GetChild(1).localPosition;
        for (float t = 0f; t < 2f; t += 0.02f) {
            if (t >= 0.3f && t <= 0.5f)
                transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
            transform.GetChild(1).Translate(0f, (vec2.y - vec1.y) * 0.01f * (2f - t), 0f);
            if (t <= 1.5f)
                transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>().Play();
            if (transform.GetChild(1).localPosition.y >= 6.18f) {
                transform.GetChild(1).localPosition = new Vector2(transform.GetChild(1).localPosition.x, 6.18f);
                break;
            }
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1f);
        transform.GetChild(1).GetChild(2).GetComponent<ParticleSystem>().Play();

        // 카메라 심장을 중심으로 이동시키기 + 줌아웃 4.5 -> 8~9
        HUDController.instance.ChangeCameraLensSize(10);
        HUDController.instance.ChangeCameraFollowTarget(hearts.transform);



        transform.GetChild(1).GetChild(3).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1f);
        transform.GetChild(1).GetChild(5).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(3.5f);
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        transform.GetChild(1).GetChild(4).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1.5f);
        transform.GetChild(2).GetChild(0).GetComponent<ParticleSystem>().Play();
        transform.GetChild(2).GetChild(1).GetComponent<ParticleSystem>().Play();

        // 전투 시작. 보스 및 플레이어 행동 가능

        HUDController.instance.OffBlackBoard();

        StartCoroutine("SpawnCoals");
        StartCoroutine("SpawnMans");
        StartCoroutine("Idling");
    }


    public virtual void TakeDamage(float amount, bool damageReduction = true, Vector2 hitDirection = new Vector2(),
       float knockbackForce = 0)
    {
        if (isdead)
        {
            return;
        }

        if (isInvincible)
        {
            return;
        }

        // 무적 시간 적용
        StartCoroutine(InvincibilityCoroutine());


        // 대미지
        curhealth -= amount;
        Debug.Log("보스 피격");
        if (curhealth <= 0f)
        {
            StartCoroutine(Dying());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float timer = 0f;
        float duration = 0.2f;
        SpriteRenderer sr = hearts.GetComponent<SpriteRenderer>();

        // 색 변경
        while (timer < duration)
        {
            // 경과 시간에 따른 비율 계산
            float t = timer / duration;

            // 색상 보간
            sr.color = Color.Lerp(Color.white, Color.red, t);

            timer += Time.deltaTime;
            yield return null; // 한 프레임 대기
        }
        // 마지막으로 목표 색상으로 설정
        sr.color = Color.red;

        yield return new WaitForSeconds(0.5f); // 무적 시간

        // 색 변경
        timer = 0f;
        while (timer < duration)
        {
            // 경과 시간에 따른 비율 계산
            float t = timer / duration;

            // 색상 보간
            sr.color = Color.Lerp(Color.red, Color.white, t);

            timer += Time.deltaTime;
            yield return null; // 한 프레임 대기
        }
        // 마지막으로 목표 색상으로 설정
        sr.color = Color.white;

        isInvincible = false;
    }
}
