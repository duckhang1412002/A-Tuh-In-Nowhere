using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenIdentifier : MonoBehaviour
{
    //Pu Bu
    public string targetScreenName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove sceneLoader = other.GetComponent<PlayerMove>();
            if (sceneLoader != null)
            {
                sceneLoader.LoadSceneByName(targetScreenName);
            }
        }
    }
}
