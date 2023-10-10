using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CreativeUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject mapItem;
    [SerializeField]
    public Transform parentTransform;

    private FirebaseAuthentication auth;
    DatabaseReference dataRef;
    List<Map> creativeMaps;
    void Start()
    {
        auth = FirebaseAuthentication.GetInstance();
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(GetListCreativeMap());
    }

    public void DuplicateObject()
    {
        // Instantiate a new copy of the object
        GameObject newObject = Instantiate(mapItem);

        // Set the new object's parent to the specified parentTransform
        newObject.transform.parent = parentTransform;

        // Set the position of the new object (you can adjust this as needed)
        newObject.transform.localPosition = Vector3.zero;

        Destroy(mapItem);
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
                        string _MapThumbnail = mapSnapShot.Child("Mapthumbnail").Value.ToString();
                        string _Description = mapSnapShot.Child("Description").Value.ToString();
                        bool _IsDeleted = Convert.ToBoolean(mapSnapShot.Child("IsDeleted").GetValue(false));
                        string _StatusID = mapSnapShot.Child("StatusID").Value.ToString();
                        string _CreatedDate = mapSnapShot.Child("Createddate").Value.ToString();

                        if (_MapType == "creative" && _StatusID == "map_approved" && _AccountID == auth.currentAccountID)
                            creativeMaps.Add(new Map(_AccountID, int.Parse(_MapID), _MapName, _MapType, _MapThumbnail, _Description, DateTime.Parse(_CreatedDate), DateTime.Parse(_CreatedDate), _IsDeleted));
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
        foreach (Map map in creativeMaps)
        {
            Debug.Log(map.MapName);
        }
    }
}
