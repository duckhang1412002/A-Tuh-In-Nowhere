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
using UnityEditor.Experimental.GraphView;

public class FirebaseAuthentication : MonoBehaviourPunCallbacks
{
    [Header("Panel")]
    public GameObject loginPanel;
    public GameObject RegisterPanel;

    // Firebase variable
    [Header("Firebase")]
    public static FirebaseAuthentication Instance;
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference accountsRef;
    public int currentAccountID = -1; //I change from int? to -1 for null

    // Login Variables
    [Space]
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public TMP_InputField nameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;
    public ErrorPopup errorPopup;

    private float autoUpdateDelay = 10f; //30 s
    private float autoUpdateDelayTimer = 10f;
    private bool autoUpdate = false;

    public static FirebaseAuthentication GetInstance()
    {
        return Instance;
    }

    private void Start()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        ClearFields();
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

    private void Update()
    {
        if (currentAccountID != -1)
        {
            autoUpdateDelayTimer += Time.deltaTime;
            if (autoUpdateDelayTimer >= autoUpdateDelay)
            {
                autoUpdateDelayTimer = 0.0f;
                //autoUpdate = true; // Enable input after the delay
                                   //Start update last active

                string Node = "Account";
                StartCoroutine(UpdateData(Node, "Lastactive", currentAccountID, DateTime.Now.ToString()));
            }
        }
    }

    public void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(async task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies are available.");
                // Initialize the Firebase app
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Initialize the FirebaseAuth instance
                auth = FirebaseAuth.DefaultInstance;
                // Get the reference to the "Accounts" node
                accountsRef = FirebaseDatabase.DefaultInstance.RootReference;

                Task fetchTask = GetListAccount(accountsRef);  
                await fetchTask;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    private async Task<List<Account>> GetListAccount(DatabaseReference db)
    {
        List<Account> accounts = new List<Account>();

        // Create a task to fetch data from the "accounts" node
        Task<DataSnapshot> task = db.Child("Account").GetValueAsync();

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
                string _Email = accountSnapshot.Child("Email").Value.ToString();
                string _Pwd = accountSnapshot.Child("Pwd").Value.ToString();
                string _Fullname = accountSnapshot.Child("Fullname").Value.ToString();
                bool _IsOnlined = Convert.ToBoolean(accountSnapshot.Child("IsOnlined").GetValue(false));
                string _RoleID = accountSnapshot.Child("RoleID").Value.ToString();
                string _Nickname = accountSnapshot.Child("Nickname").Value.ToString();
                string _AvtLink = accountSnapshot.Child("Avatarlink").Value.ToString();
                string _Key = accountSnapshot.Child("Key").Value.ToString();
                string _Ribbon = accountSnapshot.Child("Ribbon").Value.ToString();
                string _LastActive = accountSnapshot.Child("Lastactive").Value.ToString();

                accounts.Add(new Account(int.Parse(_AccountID), _Email, _Pwd, _Fullname, _IsOnlined, int.Parse(_RoleID), _Nickname, _AvtLink, int.Parse(_Ribbon), int.Parse(_Key), DateTime.Parse(_LastActive)));
            }
        }
        else
        {
            Debug.LogError("Failed to get accounts: " + task.Exception);
        }

        return accounts;
    }


    //Clear the login feilds
    public void ClearFields()
    {
        nameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        confirmPasswordRegisterField.text = "";
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    // Track state changes of the auth object.
    /*void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                StartCoroutine(UpdateStatus(user.UserId, false));
                auth.SignOut();
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }*/

    public async void Login()
    {
        List<Account> accounts = await GetListAccount(accountsRef);

        //Call the login coroutine passing the email and password
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text, accounts));
    }

    private IEnumerator LoginAsync(string email, string password, List<Account> accounts)
    {
        if (CheckLogIn(email, accounts))
        {
            errorPopup.ShowPopup("This account is already logged in!");
            yield break; // Exit the coroutine
        }
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Login Failed! Because ";
            
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    errorPopup.ShowPopup("Email is invalid! ");
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    errorPopup.ShowPopup("Wrong Password! ");
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    errorPopup.ShowPopup("Email is missing! ");
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    errorPopup.ShowPopup("Password is missing! ");
                    break;
                case AuthError.UserNotFound:
                    failedMessage = "Account does not exist";
                    errorPopup.ShowPopup("User not found ");
                    break;
                default:
                    errorPopup.ShowPopup("Something was wrong... ");
                    break;
            }
            //Debug.Log(failedMessage);
        }
        else
        {
            int matchingAccID = FindAccount(email, accounts).AccountID;

            // User is logged in now
            user = loginTask.Result.User;
            UpdateUserID(matchingAccID);
            StartCoroutine(UpdateStatus(currentAccountID, true));
            PhotonNetwork.NickName = user.DisplayName;

            Debug.LogFormat("{0} with {1} You Are Successfully Logged In", user.DisplayName, currentAccountID);

            ClearFields();
            SceneManager.LoadScene("GameMode");
        }
    }

    public async void Register()
    {
        List<Account> accounts = await GetListAccount(accountsRef);

        //Call the register coroutine passing the email and password
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text, accounts));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword, List<Account> accounts)
    {

        if (email == "")
        {
            //Debug.LogError("Email field is empty");
            errorPopup.ShowPopup("Email field is empty! ");

            yield break;
        }
        else if (name == "")
        {
            //Debug.LogError("User Name is empty");
            errorPopup.ShowPopup("The nickname is empty! ");
            yield break;
        }
        else if (!CheckNicknameAvailability(name, accounts))
        {
            //Debug.LogError("Nickname is already taken");
            errorPopup.ShowPopup("Nickname is already taken! ");
            yield break; // Exit the registration coroutine
        }
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            //Debug.LogError("Password does not match");
            errorPopup.ShowPopup("Password does not match! ");
            yield break;
        }

        else if (confirmPasswordRegisterField.text != confirmPasswordRegisterField.text)
        {
            //Debug.LogError("Password does not match");
            errorPopup.ShowPopup("Password does not match! ");
            yield break;
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Because ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        errorPopup.ShowPopup("Email is invalid! ");
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        errorPopup.ShowPopup("Password must be longer than 6 characters! ");
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        errorPopup.ShowPopup("Email is missing! ");
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        errorPopup.ShowPopup("Password is missing! ");
                        break;
                    case AuthError.EmailAlreadyInUse:
                        failedMessage = "Email Already In Use";
                        errorPopup.ShowPopup("Email Already In Use! ");
                        break;
                    default:
                        errorPopup.ShowPopup("Password must be more than 6 characters! ");
                        break;
                }

                //Debug.Log(failedMessage);
            }
            else
            {
                // Get The User After Registration Success
                user = registerTask.Result.User;

                UserProfile userProfile = new UserProfile { DisplayName = name };

                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Delete the user if user update failed
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    string failedMessage = "Profile update Failed! Because ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            errorPopup.ShowPopup("Email is invalid! ");
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            errorPopup.ShowPopup("Password must be longer than 6 characters! ");
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            errorPopup.ShowPopup("Email is missing! ");
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            errorPopup.ShowPopup("Password is missing! ");
                            break;
                        case AuthError.EmailAlreadyInUse:
                            failedMessage = "Email Already In Use";
                            errorPopup.ShowPopup("Email Already In Use! ");
                            break;
                        default:
                            errorPopup.ShowPopup("Password must be more than 6 characters! ");
                            break;
                    }

                    //Debug.Log(failedMessage);
                }
                else
                {
                    Account newAccount = new Account(0, email, password, name, false, 2, "", "", 0, 0, DateTime.Parse("1/1/1999")){};

                    UpdateInfoAccount(newAccount);         
                    ClearFields();              
                    RegisterPanel.SetActive(false);
                    loginPanel.SetActive(true);
                }
            }
        }
    }

    private bool CheckNicknameAvailability(string nickname, List<Account> accounts)
    {
        foreach (var account in accounts)
        {
            if (account.Fullname == nickname)
            {
                // Nickname is already taken
                return false;
            }
        }
        // Nickname is available
        return true;
    }

    private bool CheckLogIn(string email, List<Account> accounts)
    {
        Debug.Log(accounts[0].Fullname);
        Account account = FindAccount(email, accounts);
        if (account != null)
        {
            TimeSpan timeDifference = DateTime.Now - account.Lastactive;
            return timeDifference.TotalSeconds < 10;
        }
        /*        if (account != null && account.IsOnlined)
                {
                    return true;
                }*/
        // Account is not logged in
        return false;
    }

    private Account FindAccount(string email, List<Account> accounts)
    {
        foreach (Account account in accounts)
        {
            if (account.Email == email)
            {
                return account;
            }
        }

        return null; // Account not found
    }

    private async void UpdateInfoAccount(Account acc)
    {
        int newAccountID = await GetLastAccountIDAsync() + 1;

        string Node = "Account";
        StartCoroutine(UpdateData(Node, "AccountID", newAccountID, newAccountID.ToString()));
        StartCoroutine(UpdateData(Node, "StatusID", newAccountID, acc.StatusID.ToString()));
        StartCoroutine(UpdateData(Node, "Email", newAccountID, acc.Email.ToString()));
        StartCoroutine(UpdateData(Node, "Pwd", newAccountID, acc.Pwd.ToString()));
        StartCoroutine(UpdateData(Node, "Fullname", newAccountID, acc.Fullname.ToString()));
        StartCoroutine(UpdateData(Node, "Nickname", newAccountID, acc.Nickname.ToString()));
        StartCoroutine(UpdateData(Node, "Avatarlink", newAccountID, acc.Avatarlink.ToString()));
        StartCoroutine(UpdateData(Node, "Ribbon", newAccountID, acc.Ribbon.ToString()));
        StartCoroutine(UpdateData(Node, "Key", newAccountID, acc.Key.ToString()));
        StartCoroutine(UpdateData(Node, "Createddate", newAccountID, acc.Createddate.ToString()));
        StartCoroutine(UpdateData(Node, "Lastactive", newAccountID, acc.Lastactive.ToString()));
        StartCoroutine(UpdateData(Node, "Deleteddate", newAccountID, acc.Deleteddate.ToString()));
        StartCoroutine(UpdateData(Node, "IsDeleted", newAccountID, acc.IsDeleted.ToString()));
        StartCoroutine(UpdateData(Node, "RoleID", newAccountID, acc.RoleID.ToString()));
        StartCoroutine(UpdateStatus(newAccountID, acc.IsOnlined));
    }

    private void UpdateUserID(int userID)
    {
        currentAccountID = userID;
    }

    public IEnumerator UpdateData(string Node, string dataName, int newAccountID, string data)
    {
        //Set the currently logged in user deaths
        var DBTask = accountsRef.Child(Node).Child(newAccountID.ToString()).Child(dataName).SetValueAsync(data);

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

    public IEnumerator UpdateStatus(int? userId, bool data)
    {
        //Set the currently logged in user deaths
        var DBTask = accountsRef.Child("Account").Child(userId.ToString()).Child("IsOnlined").SetValueAsync(data);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("User status updated successfully.");
        }
    }

    public void LogOut()
    {
        Application.Quit();
    }

    public int? GetPlayerID(){
        return currentAccountID;
    }

    public async Task<int> GetLastAccountIDAsync()
    {
        try
        {
            DataSnapshot snapshot = await accountsRef.Child("Account")
                .OrderByKey()
                .LimitToLast(1)
                .GetValueAsync();

            if (snapshot.HasChildren)
            {
                // Since we used LimitToLast(1), there should be only one child.
                foreach (var childSnapshot in snapshot.Children)
                {
                    int lastAccountId = int.Parse(childSnapshot.Key);
                    return lastAccountId;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting last account ID: {ex.Message}");
            Debug.Log("");
        }

        // Return a default value (e.g., -1) if there was an error or no data found.
        return 0;
    }
}
