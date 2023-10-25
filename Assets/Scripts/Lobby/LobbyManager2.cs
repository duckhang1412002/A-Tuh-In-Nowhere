using Photon.Pun;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class LobbyManager2 : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefabM;
    [SerializeField] GameObject playerPrefabF;

    [PunRPC]
    private GameObject playerHost;
    [PunRPC]
    private GameObject playerClient;

    void Start(){
        playerHost = playerClient = null;

        if(SceneManager.GetActiveScene().name == "MultiplayerLobby"){
            Debug.Log("Player " + PhotonNetwork.LocalPlayer.ActorNumber);
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                Debug.Log("FIRST");
                SetPlayerM(1,0,0);

                playerHost.GetComponent<LobbyMove>().enabled = true;
            }
            else if(PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                Debug.Log("SECOND");
                SetPlayerF(2,5,1);

                playerClient.GetComponent<LobbyMove>().enabled = true;
            }
        }
    }

    /*---Test---*/
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        // Called when a room is successfully created
        if(SceneManager.GetActiveScene().name != "MultiplayerLobby"){
            SceneManager.LoadScene("MultiplayerLobby");
        }
    }

    public override void OnJoinedRoom()
    {
        // Called when a player successfully joins a room
        // You can load the game scene here
        if(SceneManager.GetActiveScene().name != "MultiplayerLobby"){
            SceneManager.LoadScene("MultiplayerLobby");
        }
    }

    [PunRPC]
    public void SetPlayerM(int playerID, int x, int y)
    {
        Debug.Log("PLAYER IS  RUN SETPLAYERM FUNC: " + PhotonNetwork.LocalPlayer.ActorNumber);
        if (playerHost == null)
        {
            playerHost = InstantiatePlayerM(playerID, x, y);
            playerHost.GetComponent<Player>().ID = playerID;

            // Synchronize the player object across the network
            PhotonView.Get(this).RPC("SetPlayerM", RpcTarget.OthersBuffered, playerID, x, y);
        }
        else
        {
            playerHost = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault(go => go.name.Contains("Lob_M"));
        }
    }

    [PunRPC]
    public void SetPlayerF(int playerID, int x, int y)
    {
        if (playerClient == null && PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            playerClient = InstantiatePlayerF(playerID, x, y);
            playerClient.GetComponent<Player>().ID = playerID;

            // Synchronize the player object across the network
            PhotonView.Get(this).RPC("SetPlayerF", RpcTarget.OthersBuffered, playerID, x, y);
        }
        else
        {
            playerClient = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault(go => go.name.Contains("Lob_F"));
        }
    }

    private GameObject InstantiatePlayerM(int id, int x, int y)
    {
        Quaternion rotation = playerPrefabM.transform.rotation;
        float z = playerPrefabM.transform.position.z;
        int roundedX = Mathf.RoundToInt(x);
        int roundedY = Mathf.RoundToInt(y);
        Vector3 flooredPosition = new Vector3(roundedX, roundedY, z);
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabM.name, flooredPosition, rotation) as GameObject;
        instantiatedPrefab.transform.position = flooredPosition;
        return instantiatedPrefab;
    }
    private GameObject InstantiatePlayerF(int id, int x, int y)
    {
        Quaternion rotation = playerPrefabF.transform.rotation;
        float z = playerPrefabF.transform.position.z;
        int roundedX = Mathf.RoundToInt(x);
        int roundedY = Mathf.RoundToInt(y);
        Vector3 flooredPosition = new Vector3(roundedX, roundedY, z);
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabF.name, flooredPosition, rotation) as GameObject;
        instantiatedPrefab.transform.position = flooredPosition;
        return instantiatedPrefab;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("IS RUNNING PLAYER: " + newPlayer.ActorNumber);
        if(newPlayer.ActorNumber == 2 && PhotonNetwork.LocalPlayer.ActorNumber == 1){
            //SetPlayerM(playerHost.GetComponent<Player>().ID, (int)playerHost.GetComponent<Player>().CurrentPosition.x, (int)playerHost.GetComponent<Player>().CurrentPosition.y);
            PhotonView.Get(this).RPC("SetPlayerM", RpcTarget.OthersBuffered, playerHost.GetComponent<Player>().ID, (int)playerHost.GetComponent<Player>().CurrentPosition.x, (int)playerHost.GetComponent<Player>().CurrentPosition.y);
        }     
    }
}