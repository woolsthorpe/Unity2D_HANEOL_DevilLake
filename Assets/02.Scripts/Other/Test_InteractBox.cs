using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_InteractBox : MonoBehaviour,IInteractable
{
    public void Interact(Player player)
    {
        Debug.Log("씬 이동");
        ScenceTransition.instance.GoToTower();
    }
}
