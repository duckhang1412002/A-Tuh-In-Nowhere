using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void MoveToNextScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToHomeScene(){
        SceneManager.LoadScene(0);
    }

    public void BackToLevelScene(){
        SceneManager.LoadScene(1);
    }

    public void ReloadThisScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}