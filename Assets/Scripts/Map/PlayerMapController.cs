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


    /*Player Map variables*/
    public static int MapID = -1;
    public static int RestartNumber = -1;
    public static int StepNumber = 0;

    /*TextMeshPro fields*/
    [SerializeField]
    private TextMeshProUGUI Txt_Level;

    [SerializeField]
    private TextMeshProUGUI Txt_RestartNum;

    public async void Start(){
        playerMapAuthentication = PlayerMapAuthentication.GetInstance();
        if(SceneManager.GetActiveScene().name != "SingleLobby" && MapID > 0){
            Txt_Level.text = "Level " + MapID;
            Txt_RestartNum.text = "Restart Number: " + ++RestartNumber;
        }

        if(playerMapAuthentication != null){
            ActiveMapList = await playerMapAuthentication.GetCurrentPlayerMaps();      
            UpdatePlayerMap();
        }
    }

    public async void UpdatePlayerMap(){
        if(SceneManager.GetActiveScene().name != "SingleLobby"){
            playerMapAuthentication.UpdatePlayerMap(this.ActiveMapList, MapID, RestartNumber, StepNumber);
            ActiveMapList = await playerMapAuthentication.GetCurrentPlayerMaps();
        }
    }
}
