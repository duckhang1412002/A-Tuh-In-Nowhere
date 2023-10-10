using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject confirmMapUI;


    public void ToggleUI(GameObject UI){
        if(UI.activeSelf == true){
            UI.SetActive(false);
        } else {
            UI.SetActive(true);
        }
    }

    public void ShowConfirmMapUI(bool isActive, MapBlock map){
        confirmMapUI.SetActive(true);
        confirmMapUI.GetComponent<ConfirmMapUI>().ConfirmMapUISetup(isActive, map);
    }

    public void HideConfirmMapUI(){
        confirmMapUI.SetActive(false);
    }
}