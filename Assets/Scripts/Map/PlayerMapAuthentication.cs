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

public class PlayerMapAuthentication : MonoBehaviourPunCallbacks
{
    // Firebase variable
    [Header("Firebase")]
    public static PlayerMapAuthentication Instance;
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference accountsRef;

    [SerializeField]
    FirebaseAuthentication firebaseAuth;

    public int? currentAccountID = null;

    public static PlayerMapAuthentication GetInstance()
    {
        return Instance;
    }

    private void Start()
    {
        firebaseAuth = FirebaseAuthentication.GetInstance();
        if (firebaseAuth != null)
        {
            firebaseAuth.InitializeFirebase();
            auth = firebaseAuth.auth;
            user = firebaseAuth.user;
            accountsRef = firebaseAuth.accountsRef;
            currentAccountID = firebaseAuth.currentAccountID;
        }

        //FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);

        InitializeFirebase();
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

    public async void InitializeFirebase()
    {
        Task fetchTask = GetListPlayerMap(accountsRef);  
        await fetchTask;
    }

    private async Task<List<PlayerMap>> GetListPlayerMap(DatabaseReference db)
    {
        List<PlayerMap> playerMaps = new List<PlayerMap>();

        // Create a task to fetch data from the "accounts" node
        Task<DataSnapshot> task = db.Child("PlayerMap").GetValueAsync();

        // Wait for the task to complete
        await task;

        if (task.IsCompleted)
        {
            // Retrieve the data snapshot
            DataSnapshot snapshot = task.Result;

            // Loop through the children of the "accounts" node
            foreach (DataSnapshot accountSnapshot in snapshot.Children)
            {
                // Parse and use the account data
                string _AccountID = accountSnapshot.Child("AccountID").Value.ToString();
                string _MapID = accountSnapshot.Child("MapID").Value.ToString();
                string _StepNumber = accountSnapshot.Child("Stepnum").Value.ToString();
                string _RestartNumber = accountSnapshot.Child("Restartnum").Value.ToString();
                bool _IsVoted = Convert.ToBoolean(accountSnapshot.Child("IsVoted").GetValue(false));
                bool _IsDeleted = Convert.ToBoolean(accountSnapshot.Child("IsDeleted").GetValue(false));
                //string _DeletedDate = accountSnapshot.Child("DeletedDate").Value.ToString();

                playerMaps.Add(new PlayerMap(int.Parse(_AccountID), int.Parse(_MapID), int.Parse(_StepNumber), int.Parse(_RestartNumber), _IsVoted, _IsDeleted));
            }          
        }
        else
        {
            Debug.LogError("Failed to get accounts: " + task.Exception);
        }

        return playerMaps;
    }

    public async void UpdatePlayerMap(List<PlayerMap> playerMaps, int mapID, int restartNum, int stepNum)
    {
        //List<PlayerMap> playerMaps = await GetListPlayerMap(accountsRef);

        //Call the coroutine
        StartCoroutine(UpdatePlayerMapAsync(playerMaps, mapID, restartNum, stepNum));       
    }

    private IEnumerator UpdatePlayerMapAsync(List<PlayerMap> playerMaps, int mapID, int restartNum, int stepNum)
    {
        if(currentAccountID != null){
            int accountID = (int)currentAccountID;
            PlayerMap newPlayerMap = new PlayerMap(accountID, mapID, restartNum, stepNum, false, false){};
            UpdateInfoPlayerMap(newPlayerMap);
        } else yield break;
    }

    private async void UpdateInfoPlayerMap(PlayerMap map)
    {
        string Node = "PlayerMap";

        StartCoroutine(UpdateData(Node, "AccountID", map.AccountID, map.MapID, currentAccountID.ToString()));
        StartCoroutine(UpdateData(Node, "MapID", map.AccountID, map.MapID,  map.MapID.ToString()));
        StartCoroutine(UpdateData(Node, "Stepnum", map.AccountID, map.MapID, map.StepNumber.ToString()));
        StartCoroutine(UpdateData(Node, "Restartnum", map.AccountID, map.MapID, map.RestartNumber.ToString()));
        StartCoroutine(UpdateData(Node, "IsVoted", map.AccountID, map.MapID, map.IsVoted.ToString()));
        StartCoroutine(UpdateData(Node, "IsDeleted", map.AccountID, map.MapID, map.IsDeleted.ToString()));
        StartCoroutine(UpdateData(Node, "DeletedDate", map.AccountID, map.MapID, map.DeletedDate.ToString()));
    }

    public IEnumerator UpdateData(string Node, string dataName, int accountID, int mapID, string data)
    {
        //Set the currently logged in user deaths
        var DBTask = accountsRef.Child(Node).Child(accountID.ToString() + "_" + mapID.ToString()).Child(dataName).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    public async Task<List<PlayerMap>> GetCurrentPlayerMaps(){
        List<PlayerMap> playerMaps = await GetListPlayerMap(accountsRef);

        List<PlayerMap> currentPlayerMaps = playerMaps.Where(m => m.AccountID == currentAccountID).ToList();
        return currentPlayerMaps;
    }
}