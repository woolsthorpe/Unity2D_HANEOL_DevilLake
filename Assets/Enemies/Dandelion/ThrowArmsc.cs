using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThrowArmsc : MonoBehaviour
{
    public int type;

    Transform ptr;

    void Start()
    {
        ptr = GameObject.Find("Player").transform;

        if (type == 2)
            StartCoroutine("ThrowArms2");
        else if (type == 1)
            StartCoroutine("ThrowArms1");
    }

    IEnumerator ThrowArms1()
    {
        yield return new WaitForSeconds(8f);
        transform.GetChild(1).localPosition = new Vector2(Random.Range(-25f, 25f), 20f);
        transform.GetChild(0).localPosition = new Vector2(transform.GetChild(1).localPosition.x, ptr.localPosition.y);
        transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.75f);
        for (float t = 0f; t < 1f; t += 0.01f)
        {
            transform.GetChild(1).Translate(0f, -60f * Time.deltaTime, 0f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(13f);
        StartCoroutine("ThrowArms1");
    }

    IEnumerator ThrowArms2() {
        yield return new WaitForSeconds(10f);
        transform.GetChild(1).localPosition = new Vector2(-35f, Random.Range(-8f, 2.5f));
        transform.GetChild(0).localPosition = new Vector2(ptr.localPosition.x, transform.GetChild(1).localPosition.y);
        transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.75f);
        for (float t = 0f; t < 1f; t += 0.01f)
        {
            transform.GetChild(1).Translate(0f, -120f * Time.deltaTime, 0f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(10f);
        StartCoroutine("ThrowArms2");
    }
}
