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

    private void Update()
    {

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
