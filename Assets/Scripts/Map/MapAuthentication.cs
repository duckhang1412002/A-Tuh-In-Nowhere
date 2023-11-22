using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using Firebase.Database;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Threading.Tasks;

public class MapAuthentication : MonoBehaviourPunCallbacks
{
    // Firebase variable
    [Header("Firebase")]
    public static MapAuthentication Instance;
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference accountsRef;

    [SerializeField]
    FirebaseAuthentication firebaseAuth;

    public Account currentAccount = null;

    public static MapAuthentication GetInstance()
    {
        return Instance;
    }

    private void Start()
    {
        this.gameObject.AddComponent<MapController>();
        firebaseAuth = FirebaseAuthentication.GetInstance();
        if (firebaseAuth != null)
        {
            firebaseAuth.InitializeFirebase();
            auth = firebaseAuth.auth;
            user = firebaseAuth.user;
            accountsRef = firebaseAuth.accountsRef;
            currentAccount = firebaseAuth.currentAccount;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            //Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private async Task<List<Map>> GetMapList(DatabaseReference db)
    {
        List<Map> maps = new List<Map>();

        // Create a task to fetch data from the "accounts" node
        Task<DataSnapshot> task = db.Child("Map").GetValueAsync();

        // Wait for the task to complete
        await task;

        if (task.IsCompleted)
        {
            // Retrieve the data snapshot
            DataSnapshot snapshot = task.Result;

            // Loop through the children of the node
            foreach (DataSnapshot s in snapshot.Children)
            {
                int _AccountID = int.Parse(s.Child("AccountID").Value.ToString());
                string _MapID = s.Child("MapID").Value.ToString();
                string _MapName = s.Child("Mapname").Value.ToString();
                string _MapType = s.Child("Maptype").Value.ToString();
                string _Description = s.Child("Description").Value.ToString();
                bool _IsDeleted = Convert.ToBoolean(s.Child("IsDeleted").GetValue(false));
                string _StatusID = s.Child("StatusID").Value.ToString();
                string _CreatedDate = s.Child("Createddate").Value.ToString();

                maps.Add(new Map(_AccountID, int.Parse(_MapID), _MapName, _MapType, _Description, DateTime.Parse(_CreatedDate), DateTime.Parse(_CreatedDate), _IsDeleted));
            }          
        }
        else
        {
            Debug.LogError("Failed to get accounts: " + task.Exception);
        }

        maps = maps.OrderBy(obj => obj.MapID).ToList();
        return maps;
    }

    public async Task<List<Map>> GetSingleMapList(){
        List<Map> mapList = await GetMapList(accountsRef);
        List<Map> singleMapList = mapList.Where(m => m.MapType == "single").ToList();
        return singleMapList;
    }

    public async Task<List<Map>> GetMultiplayerMapList(){
        List<Map> mapList = await GetMapList(accountsRef);
        List<Map> multiplayerMapList = mapList.Where(m => m.MapType == "multiple").ToList();
        return multiplayerMapList;
    }

    public async Task<List<Map>> GetCreativeMapList(){
        List<Map> mapList = await GetMapList(accountsRef);
        List<Map> multiplayerMapList = mapList.Where(m => m.MapType == "creative").ToList();
        return multiplayerMapList;
    }
}