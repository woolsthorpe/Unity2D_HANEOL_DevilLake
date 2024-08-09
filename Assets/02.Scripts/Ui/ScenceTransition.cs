using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenceTransition : MonoBehaviour
{
    public static ScenceTransition instance { get; private set; }
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadingTime;
    [SerializeField] private AnimationCurve fadeCurve;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        
    }
    

    public void ChangeScence(string scenceName)
    {
        //StartCoroutine(LoadScence(scenceName));
    }
    //IEnumerator LoadScence(string name)
    //{
    //    AsyncOperation op = SceneManager.LoadSceneAsync(name);
    //    op.allowSceneActivation = false;

    //}
   private  IEnumerator FadeIn(float start,float end)
    {
        
        float currentTime = 0;
        float percent = 0;

        while(percent <1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / fadingTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(start,end,fadeCurve.Evaluate(percent));
            fadeImage.color = color;

            yield return null;
        }
        

        
    }


}
