using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    [Space]
    [Header("Pause Game UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private TextMeshProUGUI txt_pause_mode;
    [SerializeField] private TextMeshProUGUI txt_pause_level;
    [SerializeField] private TextMeshProUGUI txt_pause_player;
    [SerializeField] private TextMeshProUGUI txt_pause_restart;
    [SerializeField] private Button restartBtn;


    [Space]
    [Header("Confirm Map UI")]
    [SerializeField] private GameObject confirmMapUI;

    [Space]
    [Header("Result UI")]
    [SerializeField] private GameObject resultUI;
    [SerializeField] private TextMeshProUGUI txt_result_mode;
    [SerializeField] private TextMeshProUGUI txt_result_level;
    [SerializeField] private TextMeshProUGUI txt_result_player;
    [SerializeField] private TextMeshProUGUI txt_result_restart;

    [Space]
    [Header("Versus Result UI")]
    [SerializeField] private GameObject VSresultUI;
    [SerializeField] private GameObject vsPlayerM, vsPlayerF;
    [SerializeField] private TextMeshProUGUI txt_vsresult_playerName;
    [SerializeField] private TextMeshProUGUI txt_vsresult_levelName;
    [SerializeField] private TextMeshProUGUI txt_vsresult_stepCount;
    private PlayerMapAuthentication playerMapAuthentication;

    public void ToggleUI(GameObject UI){
        if(UI.activeSelf == true){
            UI.SetActive(false);
        } else {
            UI.SetActive(true);
        }
    }
    public async void ShowConfirmMapUI(MapProjector map){
        confirmMapUI.GetComponent<ConfirmMapUI>().SetupNavigateButton();
        confirmMapUI.SetActive(true);
        await confirmMapUI.GetComponent<ConfirmMapUI>().ConfirmMapUISetup(map);
    }

    public void HideConfirmMapUI(){
        confirmMapUI.SetActive(false);
    }

    public void SetupResultUI(string txt_mode, int txt_level, int txt_restart){
        txt_result_mode.text = txt_mode;
        txt_result_level.text = "Level " + txt_level;
        txt_result_restart.text = "Restart Number: " + txt_restart;
        resultUI.SetActive(true);
    }

    public void SetupVSResultUI(string playerName, string levelName, int stepCnt, string gender)
    {
        txt_vsresult_playerName.text = playerName;
        txt_vsresult_levelName.text = "Level " + levelName;
        txt_vsresult_stepCount.text = "Number of steps: " + stepCnt;
        if (stepCnt == -1) //other surrenderd
            txt_vsresult_stepCount.text = "Other player surrendered!";
        if (gender == "F")
        {
            vsPlayerF.SetActive(true);
            vsPlayerM.SetActive(false);
        } else
        {
            vsPlayerM.SetActive(true);
            vsPlayerF.SetActive(false);
        }
        VSresultUI.SetActive(true);
    }

    public void SetupPauseUI(string txt_mode, int txt_level, int txt_restart, string txt_player){
        if(SceneManager.GetActiveScene().name == "Game"){
            txt_pause_mode.text = txt_mode;
            txt_pause_level.text = "Level " + txt_level;
            txt_pause_restart.text = "Restart Number: " + txt_restart;
            txt_pause_player.text = txt_player;
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") && PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Co-op" && !PhotonNetwork.IsMasterClient)
            {
                restartBtn.interactable = false;
            }
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") && PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Versus")
            {
                TextMeshProUGUI btnText = restartBtn.GetComponentInChildren<TextMeshProUGUI>();
                btnText.text = "Surrender";
                restartBtn.interactable = true;
            }

        } else if(SceneManager.GetActiveScene().name == "GameMode") {
            txt_pause_mode.text = "";
            txt_pause_level.text = "";
            txt_pause_restart.text = "";
            txt_pause_player.text = txt_player;
        }
    }
}