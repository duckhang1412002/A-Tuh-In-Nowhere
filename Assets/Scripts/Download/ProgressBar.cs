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

public class ProgressBar : MonoBehaviour
{

    private Slider slider;
    private float targetProgress;
    private long totalFile;
    private long checkedFile;
    string folderPath;

    public float fillSpeed = 0.75f;
    public string FileName { get; set; }

    DatabaseReference dataRef;

    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>(); 
    }
    private void Start()
    {
        FileName = "1.txt";
        //filePath = $"{Application.persistentDataPath}/Maps/{FileName}";
        folderPath = $"{Application.persistentDataPath}/Maps/";
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
        targetProgress = 0;

        StartCoroutine(VerifyData());

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
                        string fileName = mapSnapShot.Key + ".txt";
                        string path = folderPath + fileName;

                        if (File.Exists(path))
                        {
                            Debug.Log("Found the " + fileName + " file locally, Loading!!!");
                            checkedFile++;
                        }
                        else
                        {
                            Debug.Log("Adding " + fileName + " to download queue");
                            filesToDownload.Add(fileName);
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
            foreach(var file in filesToDownload)
            {
                string url = $"https://firebasestorage.googleapis.com/v0/b/atuhinnowhere-testing.appspot.com/o/{file}?alt=media";
                string filePath = $"{Application.persistentDataPath}/Maps/{file}";
                StartCoroutine(GetFileRequest(url, filePath, (UnityWebRequest req) =>
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
        SceneManager.LoadScene("Login");
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
