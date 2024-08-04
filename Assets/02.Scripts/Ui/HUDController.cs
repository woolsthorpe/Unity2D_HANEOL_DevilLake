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
    [SerializeField] private RawImage hpBar;
    [SerializeField] private Vector2 maxHpImageSize;
    [SerializeField] private float hpFlowSpeed=1f;
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
        HpImageFlow();

    if(UnityEngine.Input.GetKeyDown(KeyCode.E))
        {
            ChangeHpBar(currentHealth+10,maxHeadlth);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
        {
            ChangeHpBar(currentHealth - 10, maxHeadlth);
        }

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

        RectTransform rect = hpBar.rectTransform;
        float lerpAmount = rect.sizeDelta.y;

         while(percent<1)
        {
            currentTIme += Time.deltaTime;
            percent = currentTIme / hpChangeTime;

            //hpBar.rectTransform. = Mathf.Lerp(hpBar.fillAmount,changeHp/maxHp,hpChangeCurve.Evaluate(percent));
            lerpAmount = Mathf.Lerp(lerpAmount, (changeHp / maxHp)* maxHpImageSize.y, hpChangeCurve.Evaluate(percent));
            rect.sizeDelta = new Vector2(maxHpImageSize.x,lerpAmount);

        
            yield return null;
        }

        hpBar.rectTransform.sizeDelta = new Vector2(maxHpImageSize.x,Mathf.Clamp(hpBar.rectTransform.sizeDelta.y,0,maxHpImageSize.y));
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

    private void HpImageFlow()
    {
        Rect uvRect = hpBar.uvRect;
        uvRect.x -= hpFlowSpeed * Time.deltaTime;
        hpBar.uvRect = uvRect;
    }
    public void Initialize_HpData(float currentHp,float maxHp)
    {
        currentHealth = currentHp;
        maxHeadlth = maxHp;
        hpText.text = string.Format("{0}", (int)currentHealth);
        ChangeHpBar(currentHp,maxHeadlth);
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
