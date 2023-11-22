using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleplayerLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private Player player;

    void Start()
    {


        //GameObject.Find("CameraManager").GetComponent<CameraManager>().SetupSingleplayerCamera(0, 0);
    }
}
