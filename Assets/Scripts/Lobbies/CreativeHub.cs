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

public class CreativeHub : MonoBehaviour
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
            if (x.Key.AccountID != FirebaseAuthentication.currentAccount.AccountID)
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
            if (x.Key.AccountID != FirebaseAuthentication.currentAccount.AccountID)
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
        Debug.Log("Render map: " + map.MapID);
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
        //authorName.text = $"Made by ID: {map.AccountID}";
        authorName.text = $"Made by: {map.AccountName}";
        mapPopUp.SetActive(true);
    }

    private void LoadGameByID(int mapID)
    {
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.CreateRoom("single", new RoomOptions(), TypedLobby.Default);
        PhotonNetwork.LocalPlayer.CustomProperties["MapID"] = mapID;
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
                    foreach (var s in snapshot.Children)
                    {
                        int _AccountID = int.Parse(s.Child("AccountID").Value.ToString());
                        string _MapID = s.Child("MapID").Value.ToString();
                        string _MapName = s.Child("Mapname").Value.ToString();
                        string _MapType = s.Child("Maptype").Value.ToString();
                        string _Description = s.Child("Description").Value.ToString();
                        bool _IsDeleted = Convert.ToBoolean(s.Child("IsDeleted").GetValue(false));
                        string _StatusID = s.Child("StatusID").Value.ToString();
                        string _CreatedDate = s.Child("Createddate").Value.ToString();

                        if (_MapType == "creative" && _StatusID == "map_approved")
                        {
                            Debug.Log("Creative map: " + _MapID);
                            //DuplicateObject(_MapID);
                            //creativeMaps.Add(new Map(_AccountID, int.Parse(_MapID), _MapName, _MapType, _Description, DateTime.Parse(_CreatedDate), DateTime.Parse(_CreatedDate), _IsDeleted));

                            // Fetch AccountName for the given AccountID
                            DatabaseReference accountRef = dataRef.Child("Account").Child(_AccountID.ToString());
                            accountRef.GetValueAsync().ContinueWith(accountTask =>
                            {
                                if (accountTask.IsFaulted)
                                {
                                    Debug.LogError("Failed to read account data: " + accountTask.Exception);
                                }
                                else if (accountTask.IsCompleted)
                                {
                                    DataSnapshot accountSnapshot = accountTask.Result;

                                    if (accountSnapshot != null && accountSnapshot.HasChildren)
                                    {
                                        string _AccountName = accountSnapshot.Child("Fullname").Value.ToString();

                                        // Now you have both _MapID and _AccountName

                                        creativeMaps.Add(new Map(_AccountID, _AccountName, int.Parse(_MapID), _MapName, _MapType, _Description, DateTime.Parse(_CreatedDate), DateTime.Parse(_CreatedDate), _IsDeleted));
                                    }
                                    else
                                    {
                                        Debug.Log("No account data found for AccountID: " + _AccountID);
                                    }
                                    // Set the data loaded flag to true
                                    isDataLoaded = true;
                                }
                            });

                        }
                    }

                    // Set the data loaded flag to true
                    //isDataLoaded = true;
                }
                else
                {
                    Debug.Log("No accounts data found.");
                }
            }
        });

        yield return new WaitUntil(() => isDataLoaded);
        Debug.Log("All creative maps");
        for (int i = 0; i <  creativeMaps.Count; ++i)
        {
            Map map = creativeMaps[i];
            DuplicateObject(map);
        }
        //Destroy(mapItem); //destroy the prototype
    }
}
