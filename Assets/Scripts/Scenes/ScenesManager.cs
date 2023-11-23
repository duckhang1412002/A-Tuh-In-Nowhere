using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class ScenesManager : MonoBehaviour
{
    private FirebaseAuthentication authentication; 
    public void MoveToScene(int sceneID){
        SceneManager.LoadScene(sceneID);
    }

    public void QuitGame() {
        //UnityEditor.EditorApplication.isPlaying = false;
        //authentication.LogOut();
        Debug.Log("Exit the Game!");
        Application.Quit();
    }

    public void BackToLobbyScene(){
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        if(PlayerMapController.CurrentGameMode == "Single Mode"){
            SceneManager.LoadScene("SingleLobby");
        } else if(PlayerMapController.CurrentGameMode == "Multiplayer Mode"){
            SceneManager.LoadScene("MultiplayerLobby"); 
        } else if(PlayerMapController.CurrentGameMode == "Creative Mode"){
            SceneManager.LoadScene("CreativeLobby");
        }
    }

    public void BackToGameModeScene(){
        if(PlayerMapController.CurrentGameMode == "Single Mode"){
            SceneManager.LoadScene("GameMode");
        }
        else if(SceneManager.GetActiveScene().name == "MultiplayerLobby"){
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Loading"); 
        }
        else if (SceneManager.GetActiveScene().name == "LobbySetting"){
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("GameMode");
        }
        else if(PlayerMapController.CurrentGameMode == "Multiplayer Mode"){
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("GameMode");
        }
        else {
            SceneManager.LoadScene("GameMode"); 
        }
    }

    public void ReloadThisScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}