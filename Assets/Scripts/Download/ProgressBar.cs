using Firebase.Database;
using Firebase;
using Firebase.Storage;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    private float targetProgress;
    private long totalFile;
    private long checkedFile;
    string folderPath;

    public float fillSpeed = 0.75f;
    public string FileName { get; set; }

    DatabaseReference dataRef;

    public static bool needRepair = false;

    [SerializeField]
    private Button repairBtn, yesBtn, noBtn;
    [SerializeField]
    private GameObject confirmPanel;
    [SerializeField]
    private TextMeshProUGUI askText;

    private void Awake()
    {
        //slider = gameObject.GetComponent<Slider>(); 
    }
    private void Start()
    {
        FileName = "1.txt";
        //filePath = $"{Application.persistentDataPath}/Maps/{FileName}";
        folderPath = $"{Application.persistentDataPath}/Maps/";
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
        targetProgress = 0;
        repairBtn.onClick.AddListener(Repair);
        yesBtn.onClick.AddListener(AcceptDownload);
        noBtn.onClick.AddListener(DenyDownload);
        //StartCoroutine(VerifyData());

    }

    private void AcceptDownload()
    {
        confirmPanel.SetActive(false);
        StartCoroutine(VerifyData());
    }

    private void DenyDownload()
    {
        Application.Quit();
    }

    private void Repair()
    {
        askText.text = "Are you sure to start Repair?";
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.AddListener(AcceptRepair);
        noBtn.onClick.AddListener(DenyRepair);
    }

    private void AcceptRepair()
    {
        askText.text = "The repair process start! Please close the application and open again";
        yesBtn.gameObject.SetActive(false);
        noBtn.gameObject.SetActive(false);
        StartCoroutine(RepairData());
    }

    private void DenyRepair()
    {
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.AddListener(AcceptDownload);
        noBtn.onClick.AddListener(DenyDownload);
    }

    private void Update()
    {
        float targetProgress = (float)checkedFile / totalFile;
        if (slider.value < targetProgress)
        {
            Debug.Log("update value!");
            slider.value += fillSpeed * Time.deltaTime;
        }
    }

    private IEnumerator RepairData()
    {
        string mapsFolderPath = Path.Combine(Application.persistentDataPath, "Maps");
        string thumbsFolderPath = Path.Combine(Application.persistentDataPath, "Thumbs");

        if (Directory.Exists(mapsFolderPath))
        {
            Directory.Delete(mapsFolderPath, true); // The second parameter indicates whether to delete subdirectories.
            Debug.Log("Maps folder deleted.");
        }

        if (Directory.Exists(thumbsFolderPath))
        {
            Directory.Delete(thumbsFolderPath, true);
            Debug.Log("Thumbs folder deleted.");
        }

        yield return null;
    }

    private List<string> filesToDownload = new List<string>();
    private bool isDataLoaded;
    private IEnumerator VerifyData()
    {
        // Load data
        dataRef.Child("Map").GetValueAsync().ContinueWith(readTask =>
        {
            if (readTask.IsFaulted)
            {
                Debug.LogError("Failed to read accounts data: " + readTask.Exception);
            }
            else if (readTask.IsCompleted)
            {
                DataSnapshot snapshot = readTask.Result;

                if (snapshot != null && snapshot.HasChildren)
                {
                    Debug.Log("Map count: " + snapshot.ChildrenCount);
                    totalFile = snapshot.ChildrenCount;

                    foreach (var mapSnapShot in snapshot.Children)
                    {
                        string mapID = mapSnapShot.Child("MapID").Value.ToString();
                        string path = folderPath + mapID + ".txt";

                        if (File.Exists(path))
                        {
                            Debug.Log("Found the " + mapID + " file locally, Loading!!!");
                            checkedFile+=2; //map txt and image
                        }
                        else
                        {
                            Debug.Log("Adding " + mapID + " to download queue");
                            filesToDownload.Add(mapID);
                        }
                    }

                    // Set the data loaded flag to true
                    isDataLoaded = true;
                }
                else
                {
                    Debug.Log("No accounts data found.");
                }
            }
        });

        // Wait until the data is loaded
        yield return new WaitUntil(() => isDataLoaded);
        Debug.Log(filesToDownload.Count);
        // Now, you can proceed with the download
        if (filesToDownload.Count > 0)
        {
            foreach(var mapName in filesToDownload)
            {
                string urlMap = $"https://firebasestorage.googleapis.com/v0/b/atuhinnowhere-testing.appspot.com/o/{mapName}.txt?alt=media";
                string fileMapPath = $"{Application.persistentDataPath}/Maps/{mapName}.txt";
                StartCoroutine(GetFileRequest(urlMap, fileMapPath, (UnityWebRequest req) =>
                {
                    if (req.isNetworkError || req.isHttpError)
                    {
                        //Logging any errors that may happen
                        Debug.Log($"{req.error} : {req.downloadHandler.text}");
                    }

                    else
                    {
                        Debug.Log("I end download here!");
                        checkedFile++;
                    }
                }

                ));

                // %2F = /
                string urlThumb = $"https://firebasestorage.googleapis.com/v0/b/atuhinnowhere-testing.appspot.com/o/Thumbnail%2F{mapName}.png?alt=media";
                string thumbPath = $"{Application.persistentDataPath}/Thumbs/{mapName}.png";
                StartCoroutine(GetFileRequest(urlThumb, thumbPath, (UnityWebRequest req) =>
                {
                    if (req.isNetworkError || req.isHttpError)
                    {
                        //Logging any errors that may happen
                        Debug.Log($"{req.error} : {req.downloadHandler.text}");
                    }

                    else
                    {
                        Debug.Log("I end download here!");
                        checkedFile++;
                    }
                }

                ));
            }

        }
        else
        {
            Debug.Log("All files are already downloaded.");
        }

        yield return new WaitUntil(() => slider.value >= 1f);
        SceneManager.LoadScene("Home");
    }

    IEnumerator GetFileRequest(string url, string filePath, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(filePath);

            yield return req.SendWebRequest();
            Debug.Log("I'm downloading!");
            callback(req);

        }
    }
}
