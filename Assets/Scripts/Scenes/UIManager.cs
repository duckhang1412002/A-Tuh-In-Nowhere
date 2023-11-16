using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Space]
    [Header("Pause Game UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private TextMeshProUGUI txt_pause_mode;
    [SerializeField] private TextMeshProUGUI txt_pause_level;
    [SerializeField] private TextMeshProUGUI txt_pause_player;
    [SerializeField] private TextMeshProUGUI txt_pause_restart;


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

    public void Awake()
    {
        playerMapAuthentication = PlayerMapAuthentication.GetInstance();
    }
    public void ToggleUI(GameObject UI){
        if(UI.activeSelf == true){
            UI.SetActive(false);
        } else {
            UI.SetActive(true);
        }
    }

    public async void ShowConfirmMapUI(bool isActive, MapBlock map){
        confirmMapUI.SetActive(true);
        await confirmMapUI.GetComponent<ConfirmMapUI>().ConfirmMapUISetup(isActive, map);
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
        if (gender == "M")
        {
            vsPlayerF.SetActive(true);
            vsPlayerM.SetActive(false);
        } else
        {
            vsPlayerM.SetActive(true);
            vsPlayerF.SetActive(false);
        }
        resultUI.SetActive(true);
    }

    public void SetupPauseUI(string txt_mode, int txt_level, int txt_restart, string txt_player){
        if(SceneManager.GetActiveScene().name == "Game"){
            txt_pause_mode.text = txt_mode;
            txt_pause_level.text = "Level " + txt_level;
            txt_pause_restart.text = "Restart Number: " + txt_restart;
            txt_pause_player.text = txt_player;
        } else if(SceneManager.GetActiveScene().name == "GameMode") {
            txt_pause_mode.text = "";
            txt_pause_level.text = "";
            txt_pause_restart.text = "";
            txt_pause_player.text = txt_player;
        }
    }
}