using Firebase.Extensions;
using Firebase.Storage;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.SlotRacer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class InputManager : MonoBehaviour
{
    public static string fileName { get; set; }

    public string[,] inputMap;
    private List<string[,]> listMap;

    public List<string>[] listDimensionIn { get; set; }
    public List<int>[] ListDoor { get; set; }

    GameManager gameManager;
    private bool downloadComplete;

    string filePath;
    FirebaseStorage storage;
    StorageReference storageReference;
    StorageReference mapInfo;
    UnityEvent onComplete;

    void Start()
    {
        //Application.persistDataPath stores the data at runtime to a specific location as per the platform
        //More details at:- https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html

        filePath = $"{Application.persistentDataPath}/Maps/{fileName}";
        Debug.Log("File path:" + filePath);

        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://atuhinnowhere-testing.appspot.com");

        mapInfo = storageReference.Child(fileName);
        gameManager = this.gameObject.GetComponent<GameManager>();

    }

    public void DownloadFile(UnityEvent onComplete)
    {
        string path = filePath;      //GetFilePath(url); 
        string url = $"https://firebasestorage.googleapis.com/v0/b/atuhinnowhere-testing.appspot.com/o/{fileName}?alt=media";
        if (File.Exists(path))
        {
            Debug.Log("Found the same file locally, Loading!!!");

            //StartCoroutine(LoadModelAsync(path));
            LoadModel(path);

            return;
        }

        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                //Logging any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }

            else
            {
                //Save the model fetched from firebase into spaceShip 
                LoadModel(path);
                Debug.Log("I end download here!");
                //StartCoroutine(LoadModelAsync(path));

            }
        }

        ));
        this.onComplete = onComplete;
    }

    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(filePath);

            yield return req.SendWebRequest();
            Debug.Log("I'm downloading!");
            callback(req);

        }
    }

    void LoadModel(string path)
    {
        string[] lines = File.ReadAllLines(filePath);
        gameManager.inputList = ConvertLinesToListMap(lines);
        onComplete.Invoke();
    }

    IEnumerator LoadModelAsync(string path)
    {
        using (var streamReader = new StreamReader(path))
        {
            string fileContent = streamReader.ReadToEnd();
            string[] lines = fileContent.Split('\n');
            gameManager.inputList = ConvertLinesToListMap(lines);
        }

        // Yield a frame to allow other Unity tasks to run
        yield return null;
    }
    private List<string[,]> ConvertLinesToListMap(string[] lines)
    {
        listMap = new List<string[,]>();
        int currentLineIndex = 0;
        int mapCnt = int.Parse(lines[currentLineIndex++]);
        int btnCnt = 0;
        //read all Maps
        for (int _ = 0; _ < mapCnt; ++_)
        {
            // Read the dimensions of the current map
            string[] dimensions = lines[currentLineIndex].Split(' ');
            //Debug.Log(dimensions.Length);
            int rows = int.Parse(dimensions[0]);
            int columns = int.Parse(dimensions[1]);
            //Debug.Log(rows + " - " + columns);
            string[,] grid = new string[rows, columns];

            // Populate the grid with values from the remaining lines
            for (int i = 0; i < rows; i++)
            {
                string[] values = lines[currentLineIndex + 1 + i].Split(' ');
                for (int j = 0; j < columns; j++)
                {
                    grid[i, j] = values[j];
                    btnCnt += values[j].Contains("Door") ? 1 : 0;
                }
            }
            listMap.Add(grid);
            // Move to the next map's data
            currentLineIndex += 1 + rows;
        }


        //Create Array of Connections
        //Button and Door
        ListDoor = new List<int>[btnCnt];
        for (int _ = 0; _ < btnCnt; _++)
            ListDoor[_] = new List<int>();
        //Dimension In
        listDimensionIn = new List<string>[mapCnt];
        for (int _ = 0; _ < listDimensionIn.Length; _++)
            listDimensionIn[_] = new List<string>();

        //Check connections attributes
        while (currentLineIndex < lines.Length)
        {
            //Debug.Log(lines[currentLineIndex]);
            string attribute = lines[currentLineIndex].Split("---")[1];
            //number of attribute
            ++currentLineIndex;
            int cnt = int.Parse(lines[currentLineIndex]);
            while (cnt-- > 0)
            {
                ++currentLineIndex;
                string[] description = lines[currentLineIndex].Split(' ');
                if (attribute == "DimensionIn")
                {
                    int dimensionIn = int.Parse(description[0]);
                    string direction = description[1];
                    listDimensionIn[dimensionIn].Add(direction);
                }
                else if (attribute == "DoorButton")
                {
                    int btn = int.Parse(description[0]);
                    int door = int.Parse(description[1]);
                    ListDoor[door].Add(btn);
                }
            }
            ++currentLineIndex;
        }

        return listMap;
    }
   
}
