using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public void MoveToScene(int sceneID){
        Debug.Log("Click");
        SceneManager.LoadScene(sceneID);
    }
}