using Photon.Pun;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Room Name")]
    [SerializeField] private TMP_Text roomNameText;

    private void Start()
    {
        // Display the room name and default map name
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }


    public void ConfirmSelection()
    {
        // Transfer back to the room
        PhotonNetwork.LoadLevel("Game");
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
    }

    public override void OnJoinedRoom()
    {
        // Called when a player successfully joins a room
        // You can load the game scene here
    }
}
