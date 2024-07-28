using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDLoader : MonoBehaviour
{
    public string hudScenceName= "UITestScene";
    void Start()
    {
        SceneManager.LoadSceneAsync(hudScenceName,LoadSceneMode.Additive);
    }

   
}
