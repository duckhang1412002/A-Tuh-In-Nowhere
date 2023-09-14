using Firebase.Extensions;
using Firebase.Storage;
using Photon.Pun.Demo.SlotRacer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PlayerMove playerMove;
    public static string fileName { get; set; }

    public string[,] inputMap;
    private List<string[,]> listMap;

    public List<string>[] listDimensionIn { get; set; }
    public List<int>[] ListDoor { get; set; }


    private bool downloadComplete;
    public async Task<List<string[,]>> LoadGridFromFile()
    {
        listMap = new List<string[,]>();
        downloadComplete = false;
        await Task.Run(() => DownloadAndProcessFile());
        while (!downloadComplete)
        {

        }
        return listMap;
    }

    private void DownloadAndProcessFile()
    {
        string[] lines = null;
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference storageRef = storage.GetReferenceFromUrl("gs://atuhinnowhere-testing.appspot.com");
        storageRef.Child(fileName).GetBytesAsync(long.MaxValue).ContinueWithOnMainThread((task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to download file");
                return;
            }
            Debug.Log("I downloaded the file");
            byte[] fileData = task.Result;
            Debug.Log("fileData: " + fileData);
            string fileContent = System.Text.Encoding.UTF8.GetString(fileData);
            Debug.Log("fileContent : " + fileContent);
            // Now you have the file content in 'fileContent'
            // You can split it into lines if needed
            lines = fileContent.Split('\n');
            listMap = ConvertLinesToListMap(lines);

            // Do something with 'lines'...
            downloadComplete = true;
        });


    }

    private List<string[,]> ConvertLinesToListMap(string[] lines)
    {
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

    /*public List<string[,]> LoadGridFromFile()
    {
        //fileName = "input.txt";
        listMap = new List<string[,]>();
        *//*        string folderPath = Path.Combine(Application.dataPath, "..", "CustomMap");
                string filePath = Path.Combine(folderPath, fileName);
                string[] lines = File.ReadAllLines(filePath); *//*

        //try to get from firebase 
        string[] lines = null;
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference storageRef = storage.GetReferenceFromUrl("gs://atuhinnowhere-testing.appspot.com");
        storageRef.Child(fileName).GetBytesAsync(long.MaxValue).ContinueWithOnMainThread((task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to download file");
                return;
            }
            Debug.Log("I downloaded the file");
            byte[] fileData = task.Result;
            Debug.Log("fileData: " + fileData);
            string fileContent = System.Text.Encoding.UTF8.GetString(fileData);
            Debug.Log("fileContent : " + fileContent);
            // Now you have the file content in 'fileContent'
            // You can split it into lines if needed
            lines = fileContent.Split('\n');

            // Do something with 'lines'...
        });
        //
        Debug.Log("Lines: " + lines);
        //Count number of connections
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
            for (int i = 0; i < rows; i++)  {
                string[] values = lines[currentLineIndex + 1 + i].Split(' ');
                for (int j = 0; j < columns; j++) {
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
    }*/
}
