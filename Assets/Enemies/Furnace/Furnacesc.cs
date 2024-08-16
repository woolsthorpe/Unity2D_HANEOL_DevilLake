using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnacesc : MonoBehaviour
{
    public GameObject Coals;
    public GameObject Flames;

    public float Times;

    bool isActive;

    Animator ani;

    void Start()
    {
        ani = GetComponent<Animator>();
    }

    IEnumerator MobSpawns() {
        yield return new WaitForSeconds(Times / 2);
        ani.SetBool("isOpen", true);
        yield return new WaitForSeconds(0.7f);
        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        int rr = Random.Range(1, 3);
        GameObject go;
        if (rr == 1)
            go = Instantiate(Coals);
        else
            go = Instantiate(Flames);
        go.transform.localPosition = new Vector2(transform.position.x + 0.8f, transform.position.y - 0.1f);
        yield return new WaitForSeconds(1f);
        ani.SetBool("isOpen", false);
        yield return new WaitForSeconds(Times / 2);
        StartCoroutine("MobSpawns");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player") {
            if (!isActive)
                StartCoroutine("MobSpawns");
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            if (isActive)
                StopCoroutine("MobSpawns");
            isActive = false;
        }
    }
}
