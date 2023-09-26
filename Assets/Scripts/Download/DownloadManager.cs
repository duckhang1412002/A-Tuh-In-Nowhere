using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DownloadManager : MonoBehaviour
{
    public static DownloadManager Instance; // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDownload(string url)
    {
        StartCoroutine(Download(url));
    }

    private IEnumerator Download(string url)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.error);
        }
        else
        {
            Debug.Log("Download success!");
            // Handle the downloaded data or save it to a file
        }

        req.Dispose();
    }
}
