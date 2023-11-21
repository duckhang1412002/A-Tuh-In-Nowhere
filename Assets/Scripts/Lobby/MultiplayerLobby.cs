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
    void Start()
    {
        IsReadyToStartTheMap = false;
        MapRole = "";
        CurrentChosingMap = -1;
        string gameMode = PhotonNetwork.CurrentRoom.CustomProperties["GM"].ToString();
        roomName.text = PhotonNetwork.CurrentRoom.Name + " - GM : " + gameMode;
        Debug.Log($"Public room ?: {PhotonNetwork.CurrentRoom.IsVisible}");
        if (PhotonNetwork.IsMasterClient)
        {
            myPlayer = PhotonInstantiate(playerPrefabM, 0, 0);
            myPlayer.GetComponent<LobbyMove>().enabled = true;
            photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID); //buffer remember when new player joined
        } else
        {
            myPlayer = PhotonInstantiate(playerPrefabF, 4, 0);
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
        if (mineCurrentChosingMap <= 100)
            PhotonNetwork.LocalPlayer.CustomProperties["GM"] = "Versus";
        else
            PhotonNetwork.LocalPlayer.CustomProperties["GM"] = "Co-op";
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
