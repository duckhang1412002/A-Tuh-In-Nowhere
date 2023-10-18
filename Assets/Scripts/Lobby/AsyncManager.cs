using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AsyncManager : MonoBehaviourPunCallbacks
{
    private bool connectedToMaster = false;

    [SerializeField]
    private TMP_Text loadingBtn;

    private void Start()
    {
        loadingBtn.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    /*public void OnClickConnect()
    {
        loadingBtn.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }*/

    public override void OnConnectedToMaster()
    {   
        connectedToMaster = true;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (connectedToMaster)
        {
            SceneManager.LoadScene("LobbySetting");
        }
    }

}
