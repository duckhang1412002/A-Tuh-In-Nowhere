using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Photon.Pun;
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


    //Player Map variable
    public List<PlayerMap> player_maps;
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

        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        player_maps = new List<PlayerMap>();
        player_maps.Clear();

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

    public void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Debug.Log("Firebase dependencies are available.");
                // // Initialize the Firebase app
                // FirebaseApp app = FirebaseApp.DefaultInstance;

                // // Initialize the FirebaseAuth instance
                // auth = FirebaseAuth.DefaultInstance;
                // // Get the reference to the "Accounts" node
                // accountsRef = FirebaseDatabase.DefaultInstance.RootReference;
                GetPlayerMaps(accountsRef);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    private void GetPlayerMaps(DatabaseReference db)
    {
        player_maps.Clear();
        // Read the data for each account
        db.Child("PlayerMap").GetValueAsync().ContinueWith(readTask =>
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
                    foreach (var accountSnapshot in snapshot.Children)
                    {
                        string accountID = accountSnapshot.Child("AccountID").Value.ToString();
                        string mapID = accountSnapshot.Child("MapID").Value.ToString();
                        string stepNumber = accountSnapshot.Child("Stepnum").Value.ToString();
                        string restartNumber = accountSnapshot.Child("Restartnum").Value.ToString();
                        bool isVoted = Convert.ToBoolean(accountSnapshot.Child("IsVoted").GetValue(false));

                        player_maps.Add(
                            new PlayerMap{
                                AccountID = int.Parse(accountID),
                                MapID = int.Parse(mapID),
                                StepNumber = int.Parse(stepNumber),
                                RestartNumber = int.Parse(restartNumber),
                                IsVoted = isVoted
                            }
                        );
                    }
                }
                else
                {
                    Debug.Log("No player maps data found.");
                }
            }
        });
    }

    private void InitPlayerMapController(){
        if(currentAccountID != null){
            List<PlayerMap> current_player_maps = player_maps.Where(m => m.AccountID == currentAccountID).ToList();

            //PlayerMapController playerMapController = new PlayerMapController(acc_id, current_player_maps);

            Debug.Log("CHECK------------------------------------------------" + current_player_maps.Count);
        }
    }
}