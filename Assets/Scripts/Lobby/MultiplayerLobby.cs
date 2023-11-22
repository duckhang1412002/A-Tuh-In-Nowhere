using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] GameObject playerPrefabM, playerPrefabF;
    [SerializeField] Button playBtn;
    [SerializeField] Text roomName;
    GameObject myPlayer, otherPlayer;

    [HideInInspector]
    public bool IsReadyToStartTheMap;
    [HideInInspector]
    public string MapRole;
    [HideInInspector]
    public int CurrentChosingMap;
    public string PlayGameMode{get; set;}
    void Start()
    {
        IsReadyToStartTheMap = false;
        MapRole = "";
        CurrentChosingMap = -1;
        PlayGameMode = PhotonNetwork.CurrentRoom.CustomProperties["GM"].ToString();
        roomName.text = PhotonNetwork.CurrentRoom.Name + " - GM : " + PlayGameMode;
        Debug.Log($"Public room ?: {PhotonNetwork.CurrentRoom.IsVisible}");

        int playerM_Init_XPos = 0;
        int playerM_Init_YPos = 0;
        int playerF_Init_XPos = 0;
        int playerF_Init_YPos = 0;

        if(PlayGameMode == "Co-op"){
            playerM_Init_XPos = -3;
            playerM_Init_YPos = -4;
            playerF_Init_XPos = -1;
            playerF_Init_YPos = -4;
        } else{
            playerM_Init_XPos = -32;
            playerM_Init_YPos = 1;
            playerF_Init_XPos = -28;
            playerF_Init_YPos = 1;
             GameObject.Find("CameraManager").GetComponent<CameraManager>().SetupMultiplayerCamera(0, 0, "Versus");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            myPlayer = PhotonInstantiate(playerPrefabM, playerM_Init_XPos, playerM_Init_YPos);
            myPlayer.GetComponent<LobbyMove>().enabled = true;
            photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID); //buffer remember when new player joined
        } else
        {
            myPlayer = PhotonInstantiate(playerPrefabF, playerF_Init_XPos, playerF_Init_YPos);
            myPlayer.GetComponent<LobbyMove>().enabled = true;
            photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID);
            playBtn.interactable = false;
            TextMeshProUGUI btnText = playBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Ready";
            Debug.Log("I'm not master client!");
        }
    }

    [PunRPC]
    void SetOtherPlayer(int viewID)
    {
        otherPlayer = PhotonView.Find(viewID).gameObject;
    }

    public void CheckBeforeStartTheMap(bool mineIsReady, string mineMapRole, int mineCurrentChosingMap){
        IsReadyToStartTheMap = mineIsReady;
        MapRole = mineMapRole;
        CurrentChosingMap = mineCurrentChosingMap;
        //InputManager.fileName = 101 + ".txt"; //map 100 for multiple 1 instead for choosingMap;
        Debug.Log(mineIsReady + " " + mineMapRole + " " + mineCurrentChosingMap + " viewID: " + photonView.ViewID);

        photonView.RPC("Pun_CheckBeforeStartTheMap", RpcTarget.OthersBuffered, IsReadyToStartTheMap, MapRole, CurrentChosingMap);
        PhotonNetwork.LocalPlayer.CustomProperties[$"Gender"] = MapRole;
        // if (mineCurrentChosingMap == 0)
        //     PhotonNetwork.LocalPlayer.CustomProperties["GM"] = "Versus";
        // else
        //     PhotonNetwork.LocalPlayer.CustomProperties["GM"] = "Co-op";
    }

    [PunRPC]
    void Pun_CheckBeforeStartTheMap(bool otherIsReady, string otherMapRole, int otherCurrentChosingMap){
        if (!PhotonNetwork.IsMasterClient)
        {
            playBtn.interactable = true;
        }
        if (otherIsReady && IsReadyToStartTheMap){
            if(otherMapRole != MapRole && otherCurrentChosingMap == CurrentChosingMap){
                Debug.Log("Validate BEFORE JOIN MAP SUCCESSFULLY");
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.LoadLevel("Game");
            }
        }
    }

    public void SetMapIDVersusMode(int id){
        if(PlayGameMode == "VS"){
            photonView.RPC("Pun_SetMapIDVersusMode", RpcTarget.OthersBuffered, id);
        }
    }

    [PunRPC]
    void Pun_SetMapIDVersusMode(int id){
        PlayerMapController.MapID = id;
    }

    //other player join
    public override void OnJoinedRoom()
    {
/*        myPlayer = PhotonInstantiate(playerPrefabF, 5, -4);
        myPlayer.GetComponent<LobbyMove>().enabled = true;
        photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID);*/
    }

    private GameObject PhotonInstantiate(GameObject prefab, int x, int y)
    {
        Quaternion rotation = prefab.transform.rotation;

        // Calculate the child's local position relative to the parent's position
        Vector3 localPosition = new Vector3(x, y, 6);

        // Set the child's position relative to the parent
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(prefab.name, localPosition, rotation) as GameObject;

        return instantiatedPrefab;
    }
}
