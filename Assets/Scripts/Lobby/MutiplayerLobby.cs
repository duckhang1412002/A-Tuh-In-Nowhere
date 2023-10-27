using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutiplayerLobby : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] GameObject playerPrefabM;
    [SerializeField] GameObject playerPrefabF;
    [SerializeField] Transform parentTransform;
    GameObject myPlayer, otherPlayer;
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            myPlayer = InstantiatePlayerM(0, 1);
            Debug.Log("Player1 ID: " + myPlayer.GetComponent<PhotonView>().ViewID);
            photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID); //buffer remember when new player joined

            myPlayer.GetComponent<LobbyMove>().enabled = true;
        } else {
            myPlayer = InstantiatePlayerF(5, 1);
            Debug.Log("Player2 ID: " + myPlayer.GetComponent<PhotonView>().ViewID);
            photonView.RPC("SetOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID); //buffer remember when new player joined

            myPlayer.GetComponent<LobbyMove>().enabled = true;
        }
    }

    [PunRPC]
    void SetOtherPlayer(int viewID)
    {
        otherPlayer = PhotonView.Find(viewID).gameObject;
    }

    private void Update()
    {

    }

    //other player join
    public override void OnJoinedRoom()
    {
        Debug.Log("I joined room!");
        myPlayer = InstantiatePlayerF(5, 1);
        Debug.Log("Player2 ID: " + myPlayer.GetComponent<PhotonView>().ViewID);
        photonView.RPC("setOtherPlayer", RpcTarget.OthersBuffered, myPlayer.GetComponent<PhotonView>().ViewID);
    }

    private GameObject InstantiatePlayerM(int x, int y)
    {
        Debug.Log("Init new player! at " + x + " - " + y);
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
        Debug.Log("Init new player! at " + x + " - " + y);
        Quaternion rotation = playerPrefabF.transform.rotation;

        // Calculate the child's local position relative to the parent's position
        Vector3 localPosition = new Vector3(x, y, 0);

        // Set the child's position relative to the parent
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabF.name, Vector3.zero, rotation) as GameObject;
        instantiatedPrefab.transform.localPosition = localPosition;

        return instantiatedPrefab;
    }
}
