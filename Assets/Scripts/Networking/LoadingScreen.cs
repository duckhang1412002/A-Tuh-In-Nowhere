/*using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text loading;
    [SerializeField] private TMP_Text numberPlayer;

    public float duration = 2f;

    private bool otherPlayerEntered = false;

    private GameManager gameManager; // Reference to the GameManager script


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // Assign the GameManager reference

        StartCoroutine(LoadText());
    }

    private IEnumerator WaitForPlayers()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount < 2) // Modify the player count condition as needed
        {
            numberPlayer.text = PhotonNetwork.CurrentRoom.Players.Count.ToString() + "/2";
            yield return null;
        }

        canvaWaiting.SetActive(false); // Disable the waiting canvas
        otherPlayerEntered = true; // Set the flag to true when the other player enters

        // Start loading the game scene
        string gameSceneName = "Games"; // Replace with your game scene name
        StartCoroutine(LoadGameSceneAsync(gameSceneName));
    }
    // check the time for loading
    private IEnumerator LoadGameSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Normalize progress to 0-1 range

            if (otherPlayerEntered)
            {
                // Complete the loading when the other player enters
                progress = 1f;
            }
            else
            {
                // Limit the progress to 90% if the other player has not entered
                progress = Mathf.Clamp01(progress * 0.9f);
            }

            progressBar.value = progress;

            yield return null;
        }
    }

    
}
*/