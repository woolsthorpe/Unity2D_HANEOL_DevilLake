using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController hudController_Instance;

    public float maxHeadlth=90f;
    public float currentHealth=60f;

    public int currentLavelCount = 1;
    public int currentLavelPoint = 0;

    [Header ("Link Component")]

    [Header("HP Bar")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private float hpChangeTime;
    [SerializeField] private AnimationCurve hpChangeCurve;

    [Space(5)]
    [SerializeField] private Image growLavel_Bar;
    [SerializeField] private TextMeshProUGUI currentLavel_Text;

    [Header("Pause Screen")]
    [SerializeField] private GameObject PausePanel;
    //[SerializeField] private 

    private void Awake()
    {
        if (hudController_Instance == null)
            hudController_Instance = this;
        else if (hudController_Instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        PausePanel.SetActive(false);
    }

    private void Update()
    {
    
    }

    #region Hp_interface
    public void ChangeHpBar(float changeHp,float maxHp)
    {
        if (changeHp < 0 || changeHp > maxHp)
        {
            Debug.LogError("HUDController.ChangeHpBar Value exceed");
            return;
        }
           

        if(changeHp<currentHealth)
        {
            //피격효과
        }
        else if(changeHp>=currentHealth)
        {
            //치유효과
        }


        StartCoroutine(SmoothChange_ImageSize(changeHp,maxHp));
        StartCoroutine(HpTextCounting(currentHealth , changeHp));
        currentHealth = changeHp;
        maxHeadlth = maxHp;
    }

    private IEnumerator SmoothChange_ImageSize(float changeHp,float maxHp)
    {
        float currentTIme = 0.0f;
        float percent = 0.0f;

         while(percent<1)
        {
            currentTIme += Time.deltaTime;
            percent = currentTIme / hpChangeTime;

            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount,changeHp/maxHp,hpChangeCurve.Evaluate(percent));
        
            yield return null;
        }

       
    }
    private IEnumerator HpTextCounting(float currentHp,float targetHp)
    {

        float offset = (targetHp>currentHp)?targetHp-currentHp: currentHp-targetHp;
        offset /= hpChangeTime;

        float currentTIme = 0.0f;
        float percent = 0.0f;

        while(percent<1)
        {
          
            currentHp += offset * Time.deltaTime;

            currentTIme += Time.deltaTime;
            percent = currentTIme / hpChangeTime;

            hpText.text = string.Format("{0}",(int)currentHp);
            yield return null;
        }

        currentHp = targetHp;
        hpText.text = string.Format("{0}", (int)currentHp);
    }

    public void Initialize_HpData(float currentHp,float maxHp)
    {
        currentHealth = currentHp;
        maxHeadlth = maxHp;
        hpText.text = string.Format("{0}", (int)currentHealth);
    }
    #endregion
    public void LavelUP()
    {

    }


    public void OnPressPause()
    {
        if(Time.timeScale <= 0)
        {
            Time.timeScale = 1;
            PausePanel.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            PausePanel.SetActive(true);
        }
    }
    public void OnPressInformation_Screen()
    {

    }

    public void SkillChange()
    {

    }
}
