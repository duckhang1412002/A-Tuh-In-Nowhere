using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] GameObject playerPrefabM;
    [SerializeField] GameObject playerPrefabF;
    [SerializeField] Transform parentTransform;
    GameObject myPlayer, otherPlayer;

    public bool IsReadyToStartTheMap{get; set;}
    public string MapRole{get; set;}
    public int CurrentChosingMap{get; set;}

    void Start()
    {
        IsReadyToStartTheMap = false;
        MapRole = "";
        CurrentChosingMap = -1;

        if (PhotonNetwork.IsMasterClient)
        {
            myPlayer = PhotonInstantiate(playerPrefabM, 0, -4);
            myPlayer.GetComponent<LobbyMove>().enabled = true;
            photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID); //buffer remember when new player joined
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
        InputManager.fileName = 100 + ".txt"; //map 100 for multiple 1 instead for choosingMap;
        Debug.Log(mineIsReady + " " + mineMapRole + " " + mineCurrentChosingMap);

        photonView.RPC("Pun_CheckBeforeStartTheMap", RpcTarget.All, IsReadyToStartTheMap, MapRole, CurrentChosingMap);
    }

    [PunRPC]
    void Pun_CheckBeforeStartTheMap(bool otherIsReady, string otherMapRole, int otherCurrentChosingMap){
        if(otherIsReady && IsReadyToStartTheMap){
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
        myPlayer = PhotonInstantiate(playerPrefabF, 5, -4);
        myPlayer.GetComponent<LobbyMove>().enabled = true;
        photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID);
    }

    private GameObject PhotonInstantiate(GameObject prefab, int x, int y)
    {
        Quaternion rotation = prefab.transform.rotation;

        // Calculate the child's local position relative to the parent's position
        Vector3 localPosition = new Vector3(x, y, 0);

        // Set the child's position relative to the parent
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, rotation) as GameObject;
        instantiatedPrefab.transform.localPosition = localPosition;

        return instantiatedPrefab;
    }
}
