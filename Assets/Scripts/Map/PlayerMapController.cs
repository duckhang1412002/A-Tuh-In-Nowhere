using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMapController : MonoBehaviour
{
    private List<PlayerMap> ActiveMapList{get; set;}
    private PlayerMapAuthentication playerMapAuthentication;

    public async void Start(){
        playerMapAuthentication = PlayerMapAuthentication.GetInstance();
        if(playerMapAuthentication != null){
            ActiveMapList = await playerMapAuthentication.GetCurrentPlayerMaps();
            Debug.Log(ActiveMapList.Count + "====");
            UpdatePlayerMap();
        }
    }

    public async void UpdatePlayerMap(){
        int levelName = 0;
        int restartNumber = 0;

        if(SceneManager.GetActiveScene().name != "SingleLobby"){
            string txt_levelName = GameObject.Find("Txt_Level").GetComponent<TextMeshProUGUI>().text;
            string txt_restartNum = GameObject.Find("Txt_RestartNum").GetComponent<TextMeshProUGUI>().text;

            levelName = int.Parse(txt_levelName.Split(' ')[1].Trim());
            restartNumber = int.Parse(txt_restartNum.Split(':')[1].Trim());

            playerMapAuthentication.UpdatePlayerMap(this.ActiveMapList, levelName, restartNumber, 0);
            ActiveMapList = await playerMapAuthentication.GetCurrentPlayerMaps();
        }
    }
}
