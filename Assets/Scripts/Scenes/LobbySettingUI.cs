using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbySettingUI : MonoBehaviourPunCallbacks
{
    private static string codeName;
    [SerializeField] private GameObject inputFieldObj;
    [SerializeField] private GameObject lobbySettingBtnObj;
    [SerializeField] private GameObject lobbyListBtnObj;
    [SerializeField] private GameObject settingUIObj;
    [SerializeField] private GameObject lobbyListUIObj;

    private TMP_InputField inp_code;
    private Button lobbySettingBtn;
    private Button lobbyListBtn;

    public RoomOptions roomOptions = new RoomOptions(); // new RoomOption to create a room


    private void Start()
    {
        if(!PhotonNetwork.IsConnected) 
            PhotonNetwork.ConnectUsingSettings();


        PhotonNetwork.JoinLobby(); // auto join lobby as the scene load

        inp_code = inputFieldObj.GetComponent<TMP_InputField>();
        inp_code.text = "";
        codeName = "";
          
        lobbySettingBtn = lobbySettingBtnObj.GetComponent<Button>();
        lobbyListBtn = lobbyListBtnObj.GetComponent<Button>();
        lobbyListUIObj.SetActive(false);
        settingUIObj.SetActive(true);
        lobbyListBtn.onClick.AddListener(SwitchToLobbyListUI);
        lobbySettingBtn.onClick.AddListener(SwitchToLobbySettingUI);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    private void Update()
    {

    }
    
    // Call if enter a room fail
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            //DisplayErrorText("The room does not exist!");
        }
        else
        {
            // Handle other join room failure cases
            //DisplayErrorText("Failed to join the room: " + message);
        }
    }

    public void OnBacModeScene()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("GameMode");
    }


    // enter the lobby_dual after create a room successfully
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        PhotonNetwork.LoadLevel("MultiplayerLobby");
    }

    public void OnClickLeftRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ConcatStringCode(GameObject btn){
        if(inp_code.text.Length == 6) return;
        
        inp_code.text += btn.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text.Trim();
        codeName = inp_code.text;
    }

    public void RemoveStringCode(){
        inp_code.text = "";
        codeName = "";
    }


    private void SwitchToLobbyListUI()
    {
        lobbySettingBtn.interactable = true;
        lobbyListBtn.interactable = false;
        lobbyListUIObj.SetActive(true);
        settingUIObj.SetActive(false);
    }

    private void SwitchToLobbySettingUI()
    {
        lobbySettingBtn.interactable = false;
        lobbyListBtn.interactable = true;
        lobbyListUIObj.SetActive(false);
        settingUIObj.SetActive(true);
    }

    public string AutoGenerateCode(){
        codeName = "";
        for(int i=0; i<6; i++){
            codeName += Random.Range(1, 9);
        }

        Debug.Log(codeName);

        return codeName;
    }

    public void CreateLobby(){
        if (PhotonNetwork.IsConnectedAndReady)
        {
            roomOptions.MaxPlayers = 2;
            roomOptions.IsOpen = true;
            PhotonNetwork.CreateRoom(AutoGenerateCode(), roomOptions, TypedLobby.Default); // enter the room
        }
        else
        {

        }
    }

    public void JoinLobby(){
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (string.IsNullOrEmpty(codeName) || codeName.Trim().Length != 6)
            {
                //DisplayErrorText("Please enter a valid room with 6 digits.");
            }
            else 
            {
                PhotonNetwork.JoinRoom(codeName);
            }   
        }
        else
        {
            //DisplayErrorText("Not connected to the server!");
        }
    }
}
