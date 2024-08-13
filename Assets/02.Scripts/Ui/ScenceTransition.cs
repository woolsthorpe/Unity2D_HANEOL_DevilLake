using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ScenceTransition : MonoBehaviour
{
    public static ScenceTransition instance { get; private set; }
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadingTime;
    [SerializeField] private AnimationCurve fadeCurve;
    private bool _isSceneChanging;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            ChangeScence("Start");
        }
    }

    public void ChangeScence(string scenceName)
    {
        if (_isSceneChanging)
        {
            return;
        }
        StartCoroutine(LoadScence(scenceName));
    }
    IEnumerator LoadScence(string name)
    {
        _isSceneChanging = true;
            
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        op.allowSceneActivation = false;
        StartCoroutine(Fade(0,1));
        yield return new WaitForSeconds(fadingTime);
       while(op.progress<0.8f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
        
        StartCoroutine(Fade(1,0));

        _isSceneChanging = false;
    }
    private  IEnumerator Fade(float start,float end)
    {
        
        float currentTime = 0;
        float percent = 0;
        Color color=new Color();

        while (percent <1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / fadingTime;

            color = fadeImage.color;
            color.a = Mathf.Lerp(start,end,fadeCurve.Evaluate(percent));
            fadeImage.color = color;

            yield return null;
        }
        color.a = end;
        fadeImage.color = color;

        
    }

    public void GoToStart()
    {
        ChangeScence("Start");
    }
    public void GoToTower()
    {
        ChangeScence("Tower");
    }
    public void GoToMain()
    {
        ChangeScence("Lake");
    }
    public void GameExit()
    {
        StartCoroutine(Fade(0, 1));
        Application.Quit();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
