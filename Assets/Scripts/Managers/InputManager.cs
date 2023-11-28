using Firebase.Extensions;
using Firebase.Storage;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.Demo.SlotRacer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    private void Awake()
    {
        //fileName = PhotonNetwork.LocalPlayer.CustomProperties["MapID"].ToString();
        filePath = $"{Application.persistentDataPath}/Maps/{fileName}";
        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://atuhinnowhere-testing.appspot.com");

        mapInfo = storageReference.Child(fileName);
        gameManager = this.gameObject.GetComponent<GameManager>();

    }
    public void DownloadFile(UnityEvent onComplete)
    {
        string path = filePath;      //GetFilePath(url); 
        
        if (File.Exists(path))
        {
            Debug.Log("Found the same file locally, Loading!!!");

            //StartCoroutine(LoadModelAsync(path));
            LoadModel();
            onComplete.Invoke();
            return;
        }

       /* string url = $"https://firebasestorage.googleapis.com/v0/b/atuhinnowhere-testing.appspot.com/o/{fileName}?alt=media";
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
                LoadModel();
                Debug.Log("I end download here!");
                onComplete.Invoke();
                //StartCoroutine(LoadModelAsync(path));

            }
        }

        ));*/
        //this.onComplete = onComplete;
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

    public void LoadModel()
    {
        string[] lines = File.ReadAllLines(filePath);
        gameManager.inputList = ConvertLinesToListMap(lines);
    }

    public void LoadModelAsync(UnityEvent onComplete)
    {
        fileName = PhotonNetwork.LocalPlayer.CustomProperties["MapID"].ToString()+".txt";
        filePath = $"{Application.persistentDataPath}/Maps/{fileName}";
        string[] lines = File.ReadAllLines(filePath);
        gameManager.inputList = ConvertLinesToListMap(lines);
        onComplete.Invoke();
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
                    if (grid[i, j] == "Null") grid[i, j] = "Wall";
                    btnCnt += values[j].Contains("Door") ? 1 : 0;
                }
            }
            /* ------------------------------------ */
            int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
            int[] dy = { -1, 1, 0, 0, 1, -1, 1, -1 };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (grid[i, j] == "Wall")
                    {
                        int cnt = 0;
                        bool haveGround = false;
                        for (int k = 0; k < 8; ++k)
                        {
                            int x = i + dx[k];
                            int y = j + dy[k];
                            if (x < 0 || x >= rows || y < 0 || y >= columns)
                            {
                                ++cnt;
                                continue;
                            }
                            if (grid[x, y] == "Wall" || grid[x, y] == "Null")
                            {
                                ++cnt;
                            }
                            else
                            {
                                haveGround = true;
                            }
                        }
                        if (cnt >= 4 && !haveGround) grid[i, j] = "Null";
                    }
                }
            }
                    // Assuming rows and columns are defined earlier in your code
                    // Create a grid that is larger by 2 rows and 2 columns
                    /*            string[,] grid = new string[rows + 2, columns + 2];

                                // Fill the entire grid with "Wall" initially
                                for (int i = 0; i < rows + 2; i++)
                                {
                                    for (int j = 0; j < columns + 2; j++)
                                    {
                                        grid[i, j] = "Wall";
                                    }
                                }

                                for (int i = 0; i < rows; i++)
                                {
                                    string[] values = lines[currentLineIndex + 1 + i].Split(' ');
                                    for (int j = 0; j < columns; j++)
                                    {
                                        if (values[j] == "Null")
                                        {
                                            // Replace "." with "Null"
                                            grid[i + 1, j + 1] = "Wall";
                                        }
                                        else
                                        {
                                            // Place the actual item from your input
                                            grid[i + 1, j + 1] = values[j];
                                        }

                                        btnCnt += values[j].Contains("Door") ? 1 : 0;
                                    }
                                }
                                // Now, your grid contains "#" in the outer boundary, walls generated around main items, and the actual data in the inner grid.

                                // Verify 
                                int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
                                int[] dy = { -1, 1, 0, 0, 1, -1, 1, 1 };
                                for (int i = 0; i < rows + 2; ++i)
                                {
                                    for (int j = 0; j < columns + 2; ++j)
                                    {
                                        if (grid[i, j] == "Wall")
                                        {
                                            // Check all 8 adjacent cells
                                            bool allWallsOrNull = true;
                                            int cnt = 0;
                                            for (int k = 0; k < 4; ++k)
                                            {
                                                int x = i + dx[k];
                                                int y = j + dy[k];
                                                if (x < 0 || x > rows + 1 || y < 0 || y > columns + 1)
                                                {
                                                    ++cnt;
                                                    continue;
                                                }
                                                if (grid[x, y] == "Wall" || grid[x, y] == "Null")
                                                {
                                                    ++cnt;
                                                }
                                            }
                                            if (cnt == 4) grid[i, j] = "Null";
                                        }
                                    }
                                }
                    */



                    /* ------------------------------------ */
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
