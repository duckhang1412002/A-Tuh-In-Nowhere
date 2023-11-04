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

    }
}
