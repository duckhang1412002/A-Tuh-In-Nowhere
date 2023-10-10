using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmMapUI : MonoBehaviour {
    public void ConfirmMapUISetup(bool isActive, MapBlock map){
        GameObject btn_Play = this.gameObject.transform.Find("Board").Find("Btn_Play").gameObject;

        if(!isActive){
            btn_Play.GetComponent<Button>().interactable = false;
        } else {
            btn_Play.GetComponent<Button>().interactable = true;
        }

        this.gameObject.SetActive(true);
    }
}
