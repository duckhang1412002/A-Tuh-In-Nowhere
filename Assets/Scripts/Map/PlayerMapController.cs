using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerMapController : MonoBehaviour
{
    private List<PlayerMap> ActiveMapList{get; set;}
    private PlayerMapAuthentication playerMapAuthentication;


    /*Player Map variables*/
    public static int MapID = -1;
    public static int RestartNumber = -1;
    public static int StepNumber = 0;
    public static string MapRole = "";

    public async void Start(){
        playerMapAuthentication = PlayerMapAuthentication.GetInstance();
        if(playerMapAuthentication != null){
            ActiveMapList = await playerMapAuthentication.GetCurrentPlayerMaps();    
            UpdatePlayerMap();
        }

        if(ActiveMapList != null){
            if(SceneManager.GetActiveScene().name == "GameMode"){
                foreach(PlayerMap m in ActiveMapList){
                    if(m.MapID == 10){
                        GameMode.IsUnlockMultiplayerMode = true;
                        GameMode.UpdateMultiplayerMode();
                    }
                    if(m.MapID == 2){
                        GameMode.IsUnlockCreativeMode = true;
                        GameMode.UpdateCreativeMode();
                    }
                }
            }
            else if(SceneManager.GetActiveScene().name == "Game" && MapID != -1){
                GameObject.Find("UIManager").GetComponent<UIManager>().SetupPauseUI("Single Mode", MapID, ++RestartNumber, playerMapAuthentication.currentAccount.Fullname);
            } 
            else if(SceneManager.GetActiveScene().name == "SingleLobby" || SceneManager.GetActiveScene().name == "MultiplayerLobby"){
                foreach(PlayerMap m in ActiveMapList){
                    GameObject singleMap = GameObject.Find("GameObj_MapBlock_Map_" + m.MapID);
                    if(singleMap != null){
                        singleMap.GetComponent<MapBlock>().IsSolved = true;
                        singleMap.GetComponent<MapBlock>().ChangeColor();
                    }
                }

                GameObject[] singleMaps = FindObjectsWithNameContaining("GameObj_MapBlock_Map_");
                foreach(GameObject m in singleMaps){
                    int[] previousMapID = m.GetComponent<MapBlock>().GetPreviousMapID();
                    bool checkVar = true;

                    m.GetComponent<MapBlock>().ChangeMapMachineStatus(m.GetComponent<MapBlock>().IsSolved, m);

                    foreach(int n in previousMapID){
                        GameObject singleMap = GameObject.Find("GameObj_MapBlock_Map_" + n);
                        if(singleMap != null){
                            if(!singleMap.GetComponent<MapBlock>().IsSolved){                            
                                checkVar = false;
                                break;
                            }
                        }
                    }
                    if(checkVar){
                        m.GetComponent<MapBlock>().IsUnlocked = true;
                    }
                }
            }
        }
    }

    public async void UpdatePlayerMap(){
        if(SceneManager.GetActiveScene().name == "Game"){
            bool isActivateCut_1 = true;
            bool isActivateCut_2 = true;
            foreach(PlayerMap m in ActiveMapList){
                if(m.MapID == 1 && PlayerMapController.MapID == 1){
                    isActivateCut_1 = false;
                    break;
                }
            }
            foreach(PlayerMap m in ActiveMapList){
                if(m.MapID == 2 && PlayerMapController.MapID == 2){
                    isActivateCut_2 = false; 
                    break;
                }
            }

            GameMode.ShowCutSceneMultiplayerMode = isActivateCut_1;
            GameMode.ShowCutSceneCreativeMode = isActivateCut_2;

            playerMapAuthentication.UpdatePlayerMap(this.ActiveMapList, MapID, RestartNumber, StepNumber);
            ActiveMapList = await playerMapAuthentication.GetCurrentPlayerMaps();
        }
    }

    GameObject[] FindObjectsWithNameContaining(string partialName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Use LINQ to filter objects by name
        GameObject[] matchingObjects = allObjects.Where(obj => obj.name.Contains(partialName)).ToArray();

        return matchingObjects;
    }

    public void ShowConfirmMapUI(){
        GameObject singleMap = GameObject.Find("GameObj_MapBlock_Map_" + MapID);
        if(singleMap != null){
            GameObject.Find("UIManager").GetComponent<UIManager>().ShowConfirmMapUI(
                singleMap.GetComponent<MapBlock>().IsUnlocked,
                singleMap.GetComponent<MapBlock>()
            );
        }
    }

    public void StartTheMap(){
        //InputManager.fileName = MapID + ".txt";

        if(SceneManager.GetActiveScene().name == "SingleLobby"){
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("single", new RoomOptions(), TypedLobby.Default);
            Hashtable myProperties = new Hashtable();
            myProperties["MapID"] = MapID;
            PhotonNetwork.LocalPlayer.CustomProperties = myProperties;
            SceneManager.LoadScene("Game");
        } else {
            Hashtable myProperties = new Hashtable();
            myProperties["MapID"] = MapID;
            PhotonNetwork.LocalPlayer.CustomProperties = myProperties;
            MultiplayerConfirmMap();

            // InputManager.fileName = "mul-" + MapID + ".txt";
            // PhotonNetwork.OfflineMode = false;
            // PhotonNetwork.CreateRoom("multi", new RoomOptions(), TypedLobby.Default);
        }
    }

    public void MultiplayerConfirmMap(){
        GameObject lobbyManager = GameObject.Find("LobbyManager");
        lobbyManager.GetComponent<MultiplayerLobby>().CheckBeforeStartTheMap(true, MapRole, MapID);
    }
}