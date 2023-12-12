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
using UnityEngine.UI;

public class PlayerMapController : MonoBehaviour
{
    private List<PlayerMap> ActiveMapList{get; set;}
    private PlayerMapAuthentication playerMapAuthentication;

    public Button backBtn, playBtn;


    /*Player Map variables*/
    public static int MapID = -1;
    public static int RestartNumber = -1;
    public static int StepNumber = 0;
    public static string MapRole = "";
    public static string CurrentGameMode = "";
    public static List<MapProjector> ProjectorList = null;

    public async void Start(){
        playerMapAuthentication = PlayerMapAuthentication.GetInstance();
        if(playerMapAuthentication != null){
            ActiveMapList = await playerMapAuthentication.GetCurrentPlayerMaps();
        }

        if(ActiveMapList != null){
            if(SceneManager.GetActiveScene().name == "SingleLobby"){
                GameObject[] projectors = FindObjectsWithNameContaining("GameObj_MapBlock_Map_");

                ProjectorList = new List<MapProjector>();
                foreach(GameObject m in projectors){
                    ProjectorList.Add(m.GetComponent<MapProjector>());
                }

                ProjectorList = ProjectorList.OrderBy(obj => obj.ProjectorID).ToList();

                for(int i=0; i<ProjectorList.Count; i++){
                    ProjectorList[i].MapInfo = GameObject.Find("MapController").GetComponent<MapController>().SingleMapList[i];
                }

                CurrentGameMode = "Single Mode";
            } else if(SceneManager.GetActiveScene().name == "MultiplayerLobby"){
                if(GameObject.Find("LobbyManager").GetComponent<MultiplayerLobby>().PlayGameMode == "Co-op"){
                    GameObject[] projectors = FindObjectsWithNameContaining("GameObj_MapBlock_Map_");

                    ProjectorList = new List<MapProjector>();
                    foreach(GameObject m in projectors){
                        ProjectorList.Add(m.GetComponent<MapProjector>());
                    }

                    ProjectorList = ProjectorList.OrderBy(obj => obj.ProjectorID).ToList();

                    for(int i=0; i<ProjectorList.Count; i++){
                        ProjectorList[i].MapInfo = GameObject.Find("MapController").GetComponent<MapController>().MultiplayerMapList[i];
                    }
                }

                CurrentGameMode = "Multiplayer Mode";
            } else if(SceneManager.GetActiveScene().name == "CreativeLobby"){
                CurrentGameMode = "Creative Mode";
            }

            GameObject.Find("UIManager").GetComponent<UIManager>().SetupPauseUI("", -1, -1, playerMapAuthentication.currentAccount.Fullname);

            if(SceneManager.GetActiveScene().name == "GameMode"){
                foreach(PlayerMap m in ActiveMapList){
                    if(m.MapID == 1){
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
                GameObject.Find("UIManager").GetComponent<UIManager>().SetupPauseUI(CurrentGameMode, MapID, ++RestartNumber, playerMapAuthentication.currentAccount.Fullname);
            } 
            else if(SceneManager.GetActiveScene().name == "SingleLobby" || SceneManager.GetActiveScene().name == "MultiplayerLobby" ){                
                if(SceneManager.GetActiveScene().name == "SingleLobby" || GameObject.Find("LobbyManager").GetComponent<MultiplayerLobby>().PlayGameMode == "Co-op"){
                    foreach(MapProjector m in ProjectorList){
                        PlayerMap foundMap = ActiveMapList.FirstOrDefault(playerMap => playerMap.MapID == m.MapInfo.MapID);

                        if(foundMap != null){
                            m.IsSolved = true;
                            m.ChangeColor();
                        }
                    }

                    foreach(MapProjector m in ProjectorList){
                        int[] previousMapID = m.GetPreviousMapProjectorID();
                        bool checkVar = true;

                        m.ChangeMapMachineStatus(m.IsSolved, m.gameObject);

                        foreach(int n in previousMapID){
                            GameObject foundProjector = GameObject.Find("GameObj_MapBlock_Map_" + n);
                            if(foundProjector != null){
                                if(!foundProjector.GetComponent<MapProjector>().IsSolved){                            
                                    checkVar = false;
                                    break;
                                }
                            }
                        }
                        if(checkVar){
                            m.IsUnlocked = true;
                        }
                    }
                } else {
                    GameObject.Find("GameObj_MapBlock_VSMap_0").GetComponent<MapProjector>().IsUnlocked = true;
                }
            }
        }
    }

    public async void UpdatePlayerMap(){
        if(SceneManager.GetActiveScene().name == "Game"){
            bool isActivateCut_1 = true;
            bool isActivateCut_2 = true;
            foreach(PlayerMap m in ActiveMapList){
                if(m.MapID == 10 && PlayerMapController.MapID == 10){
                    isActivateCut_1 = false;
                    break;
                }
            }
            foreach(PlayerMap m in ActiveMapList){
                if(m.MapID == 15 && PlayerMapController.MapID == 15){
                    isActivateCut_2 = false; 
                    break;
                }
            }

            GameMode.ShowCutSceneMultiplayerMode = isActivateCut_1;
            GameMode.ShowCutSceneCreativeMode = isActivateCut_2;

            playerMapAuthentication.UpdatePlayerMap(this.ActiveMapList, GetProjectorByID(PlayerMapController.MapID).MapInfo.MapID, RestartNumber, StepNumber);
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

    public void StartTheMap(){
        if(SceneManager.GetActiveScene().name == "SingleLobby"){
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("Single", new RoomOptions(), TypedLobby.Default);
            Hashtable myProperties = new Hashtable();
            myProperties["MapID"] = GetProjectorByID(MapID).MapInfo.MapID;
            PhotonNetwork.LocalPlayer.CustomProperties = myProperties;
            SceneManager.LoadScene("Game");
        } else {
            Hashtable myProperties = new Hashtable();

            if(GameObject.Find("LobbyManager").GetComponent<MultiplayerLobby>().PlayGameMode == "Co-op")
            {
                myProperties["MapID"] = GetProjectorByID(MapID).MapInfo.MapID;
            }
            else if(GameObject.Find("LobbyManager").GetComponent<MultiplayerLobby>().PlayGameMode == "VS"){
                if(PhotonNetwork.IsMasterClient){
                    List<Map> randomCreativeMaps = new List<Map>();
                    randomCreativeMaps = GameObject.Find("MapController").GetComponent<MapController>().CreativeMapList;

                    System.Random random = new System.Random();
                    int randomCreativeMapIndex = random.Next(0, randomCreativeMaps.Count);
                    MapID = randomCreativeMaps[randomCreativeMapIndex].MapID;

                    GameObject.Find("LobbyManager").GetComponent<MultiplayerLobby>().SetMapIDVersusMode(MapID);
                }
                
                myProperties["MapID"] = MapID;
            }

            PhotonNetwork.LocalPlayer.CustomProperties = myProperties;

            MultiplayerConfirmMap();
        }
    }

    public void Ready()
    {
        //Debug.Log("Did i found?" + playBtn + " " + backBtn);
        playBtn.interactable = false;
        playBtn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Wating for others!";
        backBtn.interactable = false;
    }

    public void Cancel()
    {

    }

    public void MultiplayerConfirmMap(){
        GameObject lobbyManager = GameObject.Find("LobbyManager");
        lobbyManager.GetComponent<MultiplayerLobby>().CheckBeforeStartTheMap(true, MapRole, MapID);
    }

    public MapProjector GetProjectorByID(int id){
        foreach(MapProjector m in ProjectorList){
            if(id == m.ProjectorID) return m;
        }
        return null;
    }
}