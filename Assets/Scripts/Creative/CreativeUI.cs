using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreativeUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject mapItem;
    [SerializeField]
    private GameObject mapPopUp;
    [SerializeField]
    private Transform parentTransform;

    private FirebaseAuthentication auth;
    DatabaseReference dataRef;
    List<Map> creativeMaps;

    [SerializeField]
    Text mapName;
    [SerializeField]
    Text authorName;
    [SerializeField]
    Button playBtn, backBtn;
    [SerializeField]
    Button globalBtn, myBtn;

    Dictionary<Map, GameObject> mapItemList = new Dictionary<Map, GameObject>();

    private string mapMode;
    void Start()
    {
        auth = FirebaseAuthentication.GetInstance();
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
        backBtn.onClick.AddListener(hideMapInfo);
        StartCoroutine(GetListCreativeMap());
        globalBtn.interactable = false;
        mapMode = "Global";
        myBtn.onClick.AddListener(switchToMyMaps);
        globalBtn.onClick.AddListener(switchToGlobalMaps); ;
    }

    private void switchToGlobalMaps()
    {
        globalBtn.interactable = false;
        myBtn.interactable = true;
        foreach (var x in mapItemList)
        {
            if (x.Key.AccountID != auth.currentAccount.AccountID)
            {
                x.Value.SetActive(true);
            }
        }
    }

    private void switchToMyMaps()
    {
        globalBtn.interactable = true;
        myBtn.interactable = false;
        foreach (var x in mapItemList)
        {
            if (x.Key.AccountID != auth.currentAccount.AccountID)
            {
                x.Value.SetActive(false);
            }
        }
    }

    private void DuplicateObject(Map map)
    {
        Debug.Log("Duplicating...");
        // Instantiate a new copy of the object
        GameObject newObject = Instantiate(mapItem);

        Debug.Log("Duplicate: " + newObject);

        // Set the new object's parent to the specified parentTransform
        newObject.transform.parent = parentTransform;

        // Set the position of the new object (you can adjust this as needed)
        newObject.transform.localPosition = Vector3.zero;

        newObject.transform.localScale = Vector3.one;

        RawImage imageComponent = newObject.transform.Find("Mask/Map Image").GetComponent<RawImage>();

        if (imageComponent != null)
        {
            // Construct the full file path to the image in Application.persistentDataPath
            string imagePath = $"{Application.persistentDataPath}/Thumbs/{map.MapID}.png";

            if (File.Exists(imagePath))
            {
                // Load the image from the specified file path and assign it to the Image component
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(1, 1);
                if (texture.LoadImage(imageBytes))
                {
                    imageComponent.texture = texture;
                }
            }
            else
            {
                Debug.LogError("Image file not found: " + imagePath);
            }
        }
        newObject.GetComponent<Button>().onClick.AddListener(() => showMapInfo(map));
        newObject.gameObject.SetActive(true);
        mapItemList.Add(map, newObject);
    }

    private void hideMapInfo()
    {
        mapPopUp.SetActive(false);
    }

    private void showMapInfo(Map map)
    {
        RawImage imageComponent = mapPopUp.transform.Find("Frame/Mask/Map Image").GetComponent<RawImage>();
        if (imageComponent != null)
        {
            string imagePath = $"{Application.persistentDataPath}/Thumbs/{map.MapID}.png";

            if (File.Exists(imagePath))
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(1, 1);
                if (texture.LoadImage(imageBytes))
                {
                    imageComponent.texture = texture;
                }
            }
            else
            {
                Debug.LogError("Image file not found: " + imagePath);
            }
        }

        playBtn.onClick.AddListener(() => LoadGameByID(map.MapID));
        mapName.text = map.MapName;
        authorName.text = $"Made by ID: {map.AccountID}";
        mapPopUp.SetActive(true);
    }

    private void LoadGameByID(int mapID)
    {
        InputManager.fileName = mapID + ".txt";
        SceneManager.LoadScene("Game");

    }

    private IEnumerator GetListCreativeMap()
    {
        creativeMaps = new List<Map>();

        // Load data
        bool isDataLoaded = false;
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
                    foreach (var mapSnapShot in snapshot.Children)
                    {
                        int _AccountID = int.Parse(mapSnapShot.Child("AccountID").Value.ToString());
                        string _MapID = mapSnapShot.Child("MapID").Value.ToString();
                        string _MapName = mapSnapShot.Child("Mapname").Value.ToString();
                        string _MapType = mapSnapShot.Child("Maptype").Value.ToString();
                        string _Description = mapSnapShot.Child("Description").Value.ToString();
                        bool _IsDeleted = Convert.ToBoolean(mapSnapShot.Child("IsDeleted").GetValue(false));
                        string _StatusID = mapSnapShot.Child("StatusID").Value.ToString();
                        string _CreatedDate = mapSnapShot.Child("Createddate").Value.ToString();

                        if (_MapType == "creative" && _StatusID == "map_approved")
                        {
                            Debug.Log("Creative map: " + _MapID);
                            //DuplicateObject(_MapID);
                            creativeMaps.Add(new Map(_AccountID, int.Parse(_MapID), _MapName, _MapType, _Description, DateTime.Parse(_CreatedDate), DateTime.Parse(_CreatedDate), _IsDeleted));

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

        yield return new WaitUntil(() => isDataLoaded);
        Debug.Log("All creative maps");
        foreach (Map map in creativeMaps)
        {
            DuplicateObject(map);
        }
        //Destroy(mapItem); //destroy the prototype
    }
}
