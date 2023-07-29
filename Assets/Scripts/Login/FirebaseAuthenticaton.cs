using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using Firebase.Database;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class FirebaseAuthenticaton : MonoBehaviourPunCallbacks
{
    [Header("Panel")]
    public GameObject loginPanel;
    public GameObject RegisterPanel;

    // Firebase variable
    [Header("Firebase")]
    public static FirebaseAuthenticaton Instance;
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference accountsRef;
    public List<Account> accounts;
    public string acc_userID = null;

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

    public static FirebaseAuthenticaton GetInstance()
    {
        return Instance;
    }

    private void Start()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        ClearFields();
        accounts = new List<Account>();
        accounts.Clear();

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
                Debug.Log("Firebase dependencies are available.");
                // Initialize the Firebase app
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Initialize the FirebaseAuth instance
                auth = FirebaseAuth.DefaultInstance;
                // Get the reference to the "Accounts" node
                accountsRef = FirebaseDatabase.DefaultInstance.RootReference;
                GetListAccount(accountsRef);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    private void GetListAccount(DatabaseReference db)
    {
        accounts.Clear();
        // Read the data for each account
        db.Child("Accounts").GetValueAsync().ContinueWith(readTask =>
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
                        string email = accountSnapshot.Child("email").Value.ToString();
                        string nickname = accountSnapshot.Child("nickname").Value.ToString();
                        string password = accountSnapshot.Child("password").Value.ToString();
                        bool isActive = Convert.ToBoolean(accountSnapshot.Child("isActive").GetValue(false));

                        accounts.Add(new Account { acc_email = email, acc_nickname = nickname, acc_password = password, acc_isActive = isActive });

                        //Debug.Log(email + " " + nickname + " " + password + " " + isActive);
                    }
                }
                else
                {
                    Debug.Log("No accounts data found.");
                }
            }
        });
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

    public void Login()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        if (CheckLogIn(email))
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
            // User is logged in now
            user = loginTask.Result.User;
            UpdateUserID(user.UserId);
            StartCoroutine(UpdateStatus(acc_userID, true));
            PhotonNetwork.NickName = user.DisplayName;

            Debug.LogFormat("{0} with {1} You Are Successfully Logged In", user.DisplayName, acc_userID);
            //Debug.Log(PhotonNetwork.NickName);
            //Debug.Log(acc_userID);

            ClearFields();
            SceneManager.LoadScene("PlayMode");
        }
    }

    public void Register()
    {
        //Call the register coroutine passing the email and password
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
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
        else if (!CheckNicknameAvailability(name))
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


                    string failedMessage = "Profile update Failed! Becuase ";
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
                    Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                    Debug.Log("Registration Sucessful Welcome " + user.UserId);
                    Account newAccount = new Account
                    {
                        acc_email = email,
                        acc_nickname = name,
                        acc_password = password,
                        acc_isActive = false
                    };

                    UpdateInfoAccount(newAccount);
                    accounts.Add(newAccount);
                    ClearFields();
                    RegisterPanel.SetActive(false);
                    loginPanel.SetActive(true);
                }
            }
        }
    }

    private bool CheckNicknameAvailability(string nickname)
    {
        foreach (var account in accounts)
        {
            if (account.acc_nickname == nickname)
            {
                //Debug.Log("cannot use this " + nickname);
                // Nickname is already taken
                return false;
            }
        }
        //Debug.Log("can use this " + nickname);
        // Nickname is available
        return true;
    }

    private bool CheckLogIn(string email)
    {
        Account account = FindAccount(email);
        if (account != null && account.acc_isActive )
        {
            //Debug.Log("cannot login");
            // Account is already logged in
            return true;
        }
        //Debug.Log("can login");
        // Account is not logged in
        return false;
    }

    private Account FindAccount(string email)
    {
        foreach (Account account in accounts)
        {
            if (account.acc_email == email)
            {
                return account;
            }
        }

        return null; // Account not found
    }

    private void UpdateInfoAccount(Account acc)
    {
        string Node = "Accounts";
        StartCoroutine(UpdateData(Node, "email", acc.acc_email));
        StartCoroutine(UpdateData(Node, "password", acc.acc_password));
        StartCoroutine(UpdateData(Node, "nickname", acc.acc_nickname));
        StartCoroutine(UpdateStatus(user.UserId, acc.acc_isActive));
    }

    private void UpdateUserID(string userID)
    {
        acc_userID = userID;
    }

    public IEnumerator UpdateData(string Node, string dataName, string data)
    {
        //Set the currently logged in user deaths
        var DBTask = accountsRef.Child(Node).Child(user.UserId).Child(dataName).SetValueAsync(data);

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

    public IEnumerator UpdateStatus(string userId, bool data)
    {
        //Set the currently logged in user deaths
        var DBTask = accountsRef.Child("Accounts").Child(userId).Child("isActive").SetValueAsync(data);

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

}
