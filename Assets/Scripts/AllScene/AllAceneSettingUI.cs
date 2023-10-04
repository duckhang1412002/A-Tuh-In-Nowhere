using Firebase.Auth;
using Firebase;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using Firebase.Database;
using TMPro;

public class AllAceneSettingUI : MonoBehaviourPunCallbacks
{
    // Firebase variable
    [Header("Firebase")]
    [SerializeField]
    private DependencyStatus dependencyStatus;
    [SerializeField]
    private FirebaseAuth auth;
    [SerializeField]
    private FirebaseUser user;
    [SerializeField]
    FirebaseAuthentication firebaseAuth;
    private int? currentAccountID;

    [Header("Panel")]
    [SerializeField]
    private GameObject panel;

    [Header("User Name")]
    [SerializeField]
    private TMP_Text nickname;

    public void Start()
    {
        firebaseAuth = FirebaseAuthentication.GetInstance();
        if (firebaseAuth != null)
        {
            firebaseAuth.InitializeFirebase();
            auth = firebaseAuth.auth;
            user = firebaseAuth.user;
            currentAccountID = firebaseAuth.currentAccountID;
            nickname.text = PhotonNetwork.NickName;
        } else
        {
            nickname.text = "Anomyous";
            PhotonNetwork.NickName = "Anomyous";
        }
        Debug.Log("I'm in here "+ currentAccountID);
    }

    public void OnClickOpen()
    {
        panel.SetActive(true);
    }

    public void OnClickClose()
    {
        panel.SetActive(false);
    }

    public void OnClickBackToPrevious()
    {
        if (PhotonNetwork.OfflineMode && SceneManager.GetActiveScene().name == "Game") {
            SceneManager.LoadScene("Map");
        }
        else if (PhotonNetwork.IsConnected && SceneManager.GetActiveScene().name == "Game")
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");
        } 
        else if (SceneManager.GetActiveScene().name == "Map" || SceneManager.GetActiveScene().name == "Lobby") 
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("PlayMode");
        } 
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnClickBackToPlayMode()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("PlayMode");
    }

    //Logout Method
    public void LogOut()
    {
        if (currentAccountID != null && firebaseAuth != null)
        {
            auth.SignOut();
            StartCoroutine(firebaseAuth.UpdateStatus(currentAccountID, false));
            Debug.Log("Signed out " + currentAccountID);
        }
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Login");
    }

    public void OnApplicationQuit()
    {
        if (currentAccountID != null && firebaseAuth != null)
        {
            auth.SignOut();
            StartCoroutine(firebaseAuth.UpdateStatus(currentAccountID, false));
            Debug.Log("Signed out " + currentAccountID);
        } else
        {
            Debug.Log("No one log in");
        }
    }
}
