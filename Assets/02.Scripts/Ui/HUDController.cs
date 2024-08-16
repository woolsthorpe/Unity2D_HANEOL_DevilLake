using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    public static HUDController instance { get; private set; }
    [SerializeField] private GameObject playerUI;
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

    [Space(5)]
    [Header("Skill bar")]
    [SerializeField] private Animator skillAnimator;
    [SerializeField] private Image[] IcornOfWeaporn;
    [SerializeField] private float swapSpeed=2;

    [Space(5)]

    [Header("Pause Screen")]
    [SerializeField] private GameObject PausePanel;
    //[SerializeField] private 

    [Space(5)]
    [Header("Camera Shake")]
    [SerializeField]private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin multiChannelPerlin;
    [SerializeField] private float shakeIntencity;
    [SerializeField] private float shakeTime;
    [SerializeField] private float shakeFrequency;
    private float startingIntencity;
    private float shakeTimerTotal;
    private float shakeTimer;

    [Space(5)]
    [Header("Camera Settings")]
    [SerializeField] Animator guiEventAnimator;
    [SerializeField] private float defaultLensSize=4.5f;
    public bool isPlayerStatic { get; private set; }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        PausePanel.SetActive(false);

       if(SceneManager.GetActiveScene().name !="Start")
        {
            if (virtualCamera == null)
            {
                virtualCamera = GameObject.FindObjectOfType(typeof(CinemachineVirtualCamera)).
                    GetComponent<CinemachineVirtualCamera>();
            }
            //시내머신 Virtual Camera -> Noise ->60Shake변경
            multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        virtualCamera.m_Lens.OrthographicSize = defaultLensSize;
        multiChannelPerlin.m_AmplitudeGain = 0;
        isPlayerStatic = false;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Start")
            playerUI.SetActive(false);
        else
            playerUI.SetActive(true);


        HpImageFlow();

        if(shakeTimer>0f)
        {
            shakeTimer -= Time.deltaTime;
            shakeTimer = Mathf.Clamp(shakeTimer,0f,shakeTimerTotal);

            multiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntencity, 0f, 1-(shakeTimer / shakeTimerTotal));

        }
        

        //if(Input.GetKeyDown(KeyCode.Q))
        //{
        //    SkillChange("Spear");
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SkillChange("Sword");
        //}

    }

    #region Hp_interface
    public void ChangeHpBar(float changeHp,float maxHp)
    {
        if (changeHp < 0 || changeHp > maxHp)
        {
            Debug.LogError("HUDController.ChangeHpBar Value exceed");
            return;
        }
           

        if(changeHp<currentHealth && currentHealth-changeHp>1)
        {
            ShakeCamera(shakeIntencity,shakeTime);
            //StartCoroutine(PlayerDamagedPause());
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
    // private IEnumerator HpTextCounting(float currentHp,float targetHp)
    // {
    //
    //     float offset = (targetHp>currentHp)?targetHp-currentHp: currentHp-targetHp;
    //     offset /= hpChangeTime;
    //
    //     float currentTIme = 0.0f;
    //     float percent = 0.0f;
    //
    //     while(percent<1)
    //     {
    //       
    //         currentHp += offset * Time.deltaTime;
    //
    //         currentTIme += Time.deltaTime;
    //         percent = currentTIme / hpChangeTime;
    //
    //         hpText.text = string.Format("{0}",(int)currentHp);
    //         yield return null;
    //     }
    //
    //     currentHp = targetHp;
    //     hpText.text = string.Format("{0}", (int)currentHp);
    // }
    
    private IEnumerator HpTextCounting(float currentHp, float targetHp)
    {
        float offset = (targetHp - currentHp) / hpChangeTime; 
        float currentTime = 0.0f;

        while (currentTime < hpChangeTime)
        {
            currentTime += Time.deltaTime;

            // 목표에 거의 도달한 경우 바로 목표값으로 설정
            if (Mathf.Abs(targetHp - currentHp) < Mathf.Abs(offset * Time.deltaTime))
            {
                currentHp = targetHp;
            }
            else
            {
                currentHp += offset * Time.deltaTime;
            }

            hpText.text = string.Format("{0}", (int)currentHp); 
            yield return null;
        }

        // 마지막으로 정확한 값으로 설정
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

    public void SkillChange(string weapornName)
    {
        skillAnimator.speed = swapSpeed;

        if (weapornName =="Spear" && IcornOfWeaporn[0].transform.GetSiblingIndex()==0)
        {
            IcornOfWeaporn[0].transform.SetSiblingIndex(1);
            IcornOfWeaporn[1].transform.SetSiblingIndex(0);
            skillAnimator.SetBool("SwapSpear",true);
            skillAnimator.SetBool("SwapSword",false);
        }
        else if(weapornName =="Sword" && IcornOfWeaporn[0].transform.GetSiblingIndex() == 1)
        {
            IcornOfWeaporn[0].transform.SetSiblingIndex(0);
            IcornOfWeaporn[1].transform.SetSiblingIndex(1);
            skillAnimator.SetBool("SwapSpear", false);
            skillAnimator.SetBool("SwapSword", true);
        }
        else
        {
            Debug.Log($"{weapornName}에 해당하는 이름이 없습니다");
        }
       
    }

    public void currentWeapornIcon(List<Weapon> weaporn)
    {
        Debug.Log($"{weaporn.Count}    {weaporn[0].name}");
        if(weaporn.Count<=1)
        {
            if (weaporn[0].name =="Spear")
            {
                SkillChange("Spear");
                IcornOfWeaporn[1].enabled = false;
            }
            else
            {
                IcornOfWeaporn[0].enabled = false;
            }
        }
        else
        {
            IcornOfWeaporn[0].enabled = true;
            IcornOfWeaporn[1].enabled = true;
        }
    }

    public void ShakeCamera(float intensity,float time)
    {
        multiChannelPerlin.m_AmplitudeGain = intensity;
        multiChannelPerlin.m_FrequencyGain = shakeFrequency;

        startingIntencity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
     IEnumerator PlayerDamagedPause()
    {
        Time.timeScale = 0;
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 1;
    }
    public void OnBlackBoard()
    {
        guiEventAnimator.SetBool("OnEvent",true);
        isPlayerStatic = true;
    }
    public void OffBlackBoard()
    {
        guiEventAnimator.SetBool("OnEvent", false);
        isPlayerStatic = false;
    }

    public void ChangeCameraFollowTarget(Transform newTarget)
    {
        virtualCamera.LookAt = newTarget;
    }
    public void ChangeCameraLensSize(float size)
    {
        StartCoroutine(SmoothChangeLensSize(0.65f,size));
    }
    IEnumerator SmoothChangeLensSize(float targetTime,float size)
    {
        float currentTime = 0;
        float percent = 0;
        while(percent<1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / targetTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(defaultLensSize,size,percent);

            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = size;
    }
}
