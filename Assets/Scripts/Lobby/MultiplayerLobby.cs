using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            myPlayer = InstantiatePlayerM(0, -4);
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

        Debug.Log(mineIsReady + " " + mineMapRole + " " + mineCurrentChosingMap);

        photonView.RPC("Pun_CheckBeforeStartTheMap", RpcTarget.OthersBuffered, IsReadyToStartTheMap, MapRole, CurrentChosingMap);
    }

    [PunRPC]
    void Pun_CheckBeforeStartTheMap(bool otherIsReady, string otherMapRole, int otherCurrentChosingMap){
        if(otherIsReady && IsReadyToStartTheMap){
            if(otherMapRole != MapRole && otherCurrentChosingMap == CurrentChosingMap){
                Debug.Log("Validate BEFORE JOIN MAP SUCCESSFULLY");
            }
        }
    }

    //other player join
    public override void OnJoinedRoom()
    {
        myPlayer = InstantiatePlayerF(5, -4);
        myPlayer.GetComponent<LobbyMove>().enabled = true;
        photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID);
    }

    private GameObject InstantiatePlayerM(int x, int y)
    {
        Quaternion rotation = playerPrefabM.transform.rotation;

        // Calculate the child's local position relative to the parent's position
        Vector3 localPosition = new Vector3(x, y, 0);

        // Set the child's position relative to the parent
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabM.name, Vector3.zero, rotation) as GameObject;
        instantiatedPrefab.transform.localPosition = localPosition;

        return instantiatedPrefab;
    }

        private GameObject InstantiatePlayerF(int x, int y)
    {
        Quaternion rotation = playerPrefabF.transform.rotation;

        // Calculate the child's local position relative to the parent's position
        Vector3 localPosition = new Vector3(x, y, 0);

        // Set the child's position relative to the parent
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabF.name, Vector3.zero, rotation) as GameObject;
        instantiatedPrefab.transform.localPosition = localPosition;
        instantiatedPrefab.transform.localScale = new Vector3(-instantiatedPrefab.transform.localScale.x, instantiatedPrefab.transform.localScale.y, instantiatedPrefab.transform.localScale.z);

        return instantiatedPrefab;
    }
}
