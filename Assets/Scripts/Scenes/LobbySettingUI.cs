using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LobbySettingUI : MonoBehaviourPunCallbacks
{
    private static string codeName;
    [SerializeField] private GameObject inputFieldObj;
    [SerializeField] private GameObject settingUIObj;
    [SerializeField] private GameObject lobbyListUIObj;
    [SerializeField] private GameObject createNewUIObj;

    [Header("Menu button")]
    [SerializeField] private Button joinWCodeBtn;
    [SerializeField] private Button publicRoomBtn;
    [SerializeField] private Button createNewBtn;

    private TMP_InputField inp_code;

    public RoomOptions roomOptions = new RoomOptions(); // new RoomOption to create a room


    [Header("Create new Room")]
    [SerializeField]
    private Button vsModeBtn;
    [SerializeField] 
    private Button coopModeBtn;
    [SerializeField]
    private Button createBtn;
    [SerializeField] 
    private Toggle togglePublicRoom;
    private string gameMode;

    [Header("Room list")]
    [SerializeField]
    private GameObject roomInfo;
    private float refreshInterval = 10f; // Refresh interval in seconds

    private void Start()
    {
        inp_code = inputFieldObj.GetComponent<TMP_InputField>();
        inp_code.text = "";
        codeName = "";
        createBtn.interactable = false;

        //lobbySettingBtn = lobbySettingBtnObj.GetComponent<Button>();
        //lobbyListBtn = lobbyListBtnObj.GetComponent<Button>();
        //lobbyListUIObj.SetActive(false);
        //settingUIObj.SetActive(true);
        //lobbyListBtn.onClick.AddListener(SwitchToLobbyListUI);
        //lobbySettingBtn.onClick.AddListener(SwitchToLobbySettingUI);

        // Start the coroutine to auto-refresh the room list
        StartCoroutine(RefreshRoomList());

    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!lobbyListUIObj.activeSelf)
        {
            return;
        }
        Debug.Log("OnRoomListChecking...");
        // Find the RoomsContainer and clear its children
        Transform roomsContainer = GameObject.Find("RoomsContainer").transform;
        foreach (Transform child in roomsContainer)
        {
            if (child.childCount != 0) Destroy(child.gameObject.transform.GetChild(0).gameObject);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < roomList.Count; ++i)
        {
            print(roomList[i].Name);
            GameObject newRoomInfo = Instantiate(roomInfo, Vector3.zero, Quaternion.identity, GameObject.Find("RoomsContainer").transform);
            newRoomInfo.transform.Find("RoomCode").GetComponent<TextMeshProUGUI>().text = roomList[i].Name;
            newRoomInfo.transform.Find("RoomPlayer").GetComponent<TextMeshProUGUI>().text = $"{roomList[i].PlayerCount}/{roomList[i].MaxPlayers}";
            string roomInfoMode = roomList[i].CustomProperties.ContainsKey("GM") ? roomList[i].CustomProperties["GM"].ToString() : "N/A";
            newRoomInfo.transform.Find("RoomMode").GetComponent<TextMeshProUGUI>().text = roomInfoMode;
            //roomInfo.transform.Find("RoomJoinBtn").GetComponent<Button>().onClick.AddListener(() => OnClickJoinRoom(roomList[i].Name));
        }
    }

    public void OnClickJoinRoom(TextMeshProUGUI name)
    {
        Debug.Log("Joining room: " + name);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(name.text);
        }
    }

    IEnumerator RefreshRoomList()
    {
        while (true)
        {
            PhotonNetwork.JoinLobby();
            yield return new WaitForSeconds(5f); // Refresh the room list every 5 seconds
        }
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

    public void SwitchToJoinBoard()
    {
        joinWCodeBtn.interactable = false;
        publicRoomBtn.interactable = true;
        createNewBtn.interactable = true;
        settingUIObj.SetActive(true);
        lobbyListUIObj.SetActive(false);
        createNewUIObj.SetActive(false);
    }

    public void SwitchToListBoard()
    {
        joinWCodeBtn.interactable = true;
        publicRoomBtn.interactable = false;
        createNewBtn.interactable = true;
        settingUIObj.SetActive(false);
        lobbyListUIObj.SetActive(true);
        createNewUIObj.SetActive(false);
    }

    public void SwitchToCreateBoard()
    {
        joinWCodeBtn.interactable = true;
        publicRoomBtn.interactable = true;
        createNewBtn.interactable = false;
        settingUIObj.SetActive(false);
        lobbyListUIObj.SetActive(false);
        createNewUIObj.SetActive(true);
    }

    public string AutoGenerateCode(){
        codeName = "";
        for(int i=0; i<6; i++){
            codeName += Random.Range(1, 9);
        }

        Debug.Log(codeName);

        return codeName;
    }

    public void OnClickVSMode()
    {
        vsModeBtn.interactable = false;
        coopModeBtn.interactable = true;
        gameMode = "VS";
        createBtn.interactable = true;
    }

    public void OnClickCoopMode()
    {
        vsModeBtn.interactable = true;
        coopModeBtn.interactable = false;
        gameMode = "Co-op";
        createBtn.interactable = true;
    }
    public void CreateRoom(){
        if (PhotonNetwork.IsConnectedAndReady)
        {
            roomOptions.MaxPlayers = 2;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = !togglePublicRoom.isOn;
            //custom 
            ExitGames.Client.Photon.Hashtable setValue = new ExitGames.Client.Photon.Hashtable();
            setValue.Add("GM", gameMode);
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "GM" };
            roomOptions.CustomRoomProperties = setValue;

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
