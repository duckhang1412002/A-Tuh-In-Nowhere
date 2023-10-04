using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    public void BackToLobbyScene(GameObject mode){
        if(mode.GetComponent<TextMeshProUGUI>().text == "Single Mode"){
            SceneManager.LoadScene("SingleLobby");
        } else if(mode.GetComponent<TextMeshProUGUI>().text == "Multiplayer Mode"){
            SceneManager.LoadScene("MultiplayerLobby"); 
        } else if(mode.GetComponent<TextMeshProUGUI>().text == "Creative Mode"){
            SceneManager.LoadScene("CreativeLobby");
        }
    }

    public void ReloadThisScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}