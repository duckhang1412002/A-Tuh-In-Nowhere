using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void ToggleUI(GameObject UI){
        if(UI.activeSelf == true){
            UI.SetActive(false);
        } else {
            UI.SetActive(true);
        }
    }
}