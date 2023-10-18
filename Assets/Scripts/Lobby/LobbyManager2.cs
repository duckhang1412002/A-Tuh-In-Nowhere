using Photon.Pun;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbyManager2 : MonoBehaviourPunCallbacks
{
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
        SceneManager.LoadScene("MultiplayerLobby");
    }

    public override void OnJoinedRoom()
    {
        // Called when a player successfully joins a room
        // You can load the game scene here
        SceneManager.LoadScene("MultiplayerLobby");
    }
}
