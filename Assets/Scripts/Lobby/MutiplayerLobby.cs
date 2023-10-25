using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutiplayerLobby : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] GameObject playerPrefabM;
    [SerializeField] Transform parentTransform;
    GameObject p1, p2; //player 1 - 2
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InstantiatePlayerM(0, 1);
        }
    }

    //other player join
    public override void OnJoinedRoom()
    {
        Debug.Log("I joined room!");
        InstantiatePlayerM(5, 1);
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

        // Set the parent of the instantiated object
        //instantiatedPrefab.transform.parent = parentTransform;

        return instantiatedPrefab;
    }
}
