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

    private GameObject playerHost;

    private void Start()
    {
        // Display the room name and default map name
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        if(PhotonNetwork.LocalPlayer.ActorNumber == 1){
            playerHost = GameObject.Find("PlayerM(Clone)");
            playerHost.GetComponent<LobbyMove>().enabled = true;
        }
    }
}
