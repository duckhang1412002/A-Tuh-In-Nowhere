using TMPro;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Button = UnityEngine.UI.Button;

public class ChangeRoom : MonoBehaviourPunCallbacks
{
    [Header("Waiting canvas")]
    [SerializeField] private TMP_Text loading;
    [SerializeField] private TMP_Text numberPlayer;
    [SerializeField] private TMP_Text percentText;
    [SerializeField] private GameObject waitCanvas;

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Header("Screens")]
    [SerializeField]
    private GameObject optionSelectScreen; // screen to select create or join a room

    [SerializeField]
    private GameObject joinRoomScreen; // canvas screen to click the room number and enter the room

    [Header("Create Room Panel")]
    [SerializeField]
    private GameObject openPanel;
    [SerializeField]
    private TMP_Text roomNameTxt;
    private string afterRandom = null;
    [SerializeField]
    private TMP_Text roomMasterTxt;
    [SerializeField]
    private TMP_Text MapChosenTxt;

    [Header("Room")]
    [SerializeField]
    private TMP_InputField roomNameJoin; // input-field of text of the room to join

    [SerializeField]
    private TMP_Text errorText; // error text to display if the room exist or not

    // use for display the error text to be flicker
    public float flickerDuration = 2f;
    public float flickerInterval = 0.05f;
    private bool isFlickering = false;

    [SerializeField]
    private TMP_Text errorChoosePanelTxt; // error text to display if the room exist or not

    [Header("Event System")]
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private GameObject choosePanelButton;
    [SerializeField]
    private GameObject joinPanelButton;

    public RoomOptions roomOptions = new RoomOptions(); // new RoomOption to create a room

    [Header("PLayer")]
    [SerializeField]
    private TMP_Text playerName;

    [Header("Scale Selected Button")]
    private Button currSelectButton;
    private Vector2 selectedScale = new Vector2(1.2f, 1.2f);
    private Vector2 defaultScale = new Vector2(1f, 1f);

    private void Start()
    {
        if(!PhotonNetwork.IsConnected) 
            PhotonNetwork.ConnectUsingSettings();

        StopFlickering(); // refresh the flickering

        PhotonNetwork.JoinLobby(); // auto join lobby as the scene load
        eventSystem.firstSelectedGameObject = choosePanelButton;
        currSelectButton = choosePanelButton.GetComponent<Button>();

        // display player name
        playerName.text = "";

        //lock the cursor 
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/

        errorChoosePanelTxt.text = null;
        roomNameJoin.onValueChanged.AddListener(ValidateInput);

        //waitCanvas.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    private void Update()
    {
        if (currSelectButton != null)
        {
            ScaleButton(currSelectButton, defaultScale);
        }

        // Update currently selected button
        currSelectButton = GetCurrentlySelectedButton();

        // Scale the currently selected button
        if (currSelectButton != null)
        {
            ScaleButton(currSelectButton, selectedScale);
        }
    }

    private Button GetCurrentlySelectedButton()
    {
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;

        // Check if the selected GameObject has a Button component
        if (selectedGameObject != null)
        {
            Button selectedButton = selectedGameObject.GetComponent<Button>();
            if (selectedButton != null)
            {
                return selectedButton;
            }
        }

        return null;
    }

    
    // Click join to open the canvas enter the room code
    public void OnClickJoinRoom()
    {
        // set size of btn to default
        currSelectButton = GetCurrentlySelectedButton();
        ScaleButton(currSelectButton, defaultScale);

        optionSelectScreen.SetActive(false);
        joinRoomScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(joinPanelButton);
    }

    // after enter the text in canvas joinRoom, click enter to enter the room
    public void EnterRoomNum()
    {
        string roomEnter = roomNameJoin.text;
        char map = roomEnter[roomEnter.Length - 1];

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (string.IsNullOrEmpty(roomEnter) || roomEnter.Trim().Length < 4)
            {
                DisplayErrorText("Please enter a valid room with 4 digits.");
            }
            else 
            {
                InputManager.fileName = "mul-"+ map + ".txt";
                PhotonNetwork.JoinRoom(roomEnter);
            }   
        }
        else
        {
            DisplayErrorText("Not connected to the server!");
        }

    }

    public void JoinLobby(string lobbyCode)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (string.IsNullOrEmpty(lobbyCode) || lobbyCode.Trim().Length != 6)
            {
                DisplayErrorText("Please enter a valid room with 6 digits.");
            }
            else 
            {
                PhotonNetwork.JoinRoom(lobbyCode);
            }   
        }
        else
        {
            DisplayErrorText("Not connected to the server!");
        }

    }

    private bool IsValidCharacter(char character)
    {
        return character >= '1' && character <= '5';
    }

    private void ValidateInput(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            if (input.Length > 4)
            {
                DisplayErrorText("Please enter a valid room with 4 digits.");
                roomNameJoin.text = "";
                return;
            }

            foreach (char character in input)
            {
                if (!IsValidCharacter(character))
                {
                    DisplayErrorText("Invalid room number! Please enter a number from 1 to 5.");
                    roomNameJoin.text = "";
                    return;
                }
            }
        }
    }

    // Call if enter a room fail
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            DisplayErrorText("The room does not exist!");
        }
        else
        {
            // Handle other join room failure cases
            DisplayErrorText("Failed to join the room: " + message);
        }
    }


    // this is for choosing number for the room code to enter 
    public void ClickRoomNumber(TMP_Text buttonText)
    {
        if (roomNameJoin.text.Length < 4)
        {
            roomNameJoin.text += buttonText.text;
        }
    }

    // this is for deleting if the player enter wrong number
    public void DeleteRoomNumber()
    {
        // Get the current text from the input field
        string currentText = roomNameJoin.text;

        // Check if the text is not empty
        if (!string.IsNullOrEmpty(currentText))
        {
            // Remove the last character from the text
            roomNameJoin.text = currentText.Substring(0, 0);
        }
    }

    public void OnBacModeScene()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("PlayMode");
    }

    // return the chooseOptionPanel
    public void OnClickBackPanel()
    {
        // refresh data
        roomNameJoin.text = "";
        errorText.text = "";

        // change scene
        optionSelectScreen.SetActive(true);
        joinRoomScreen.SetActive(false);

        // set the zoom btn
        eventSystem.SetSelectedGameObject(choosePanelButton);
        currSelectButton = choosePanelButton.GetComponent<Button>();
    }


    // fuction for the flicker of error text
    public void DisplayErrorText(string errorMessage)
    {
        errorText.text = errorMessage;

        if (!isFlickering)
        {
            StartFlickering();
        }
    }

    private void StartFlickering()
    {
        isFlickering = true;
        StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            errorText.enabled = !errorText.enabled;
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        errorText.enabled = true; // Ensure the error text is visible after flickering stops
        errorText.text = ""; // set it back to null when the flicker stop
        isFlickering = false;
    }

    // call in start to refresh 
    public void StopFlickering()
    {
        StopAllCoroutines();
        errorText.enabled = true; // Ensure the error text is permanently visible
        isFlickering = false;
    }

    // scale the button
    private void ScaleButton(Button button, Vector2 scale)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.localScale = scale;
    }

    /* For create panel */
    // Create a room with maximum 2 player
    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            roomOptions.MaxPlayers = 2;
            roomOptions.IsOpen = true;
            string roomNum = roomNameTxt.text; // after random a room code
            //Debug.Log("Creating room: " + roomNum);
            InputManager.fileName = "mul-" + MapChosenTxt.text + ".txt";
            PhotonNetwork.CreateRoom(roomNum, roomOptions, TypedLobby.Default); // enter the room

            // Display the wait canvas
            //waitCanvas.SetActive(true);
        }
        else
        {
            errorChoosePanelTxt.text = "There are some errors with the server!";
        }
    }

    // Open Create Room Panel
    public void OnClickOpenCreatePanel()
    { 
        openPanel.SetActive(true);
        afterRandom = roomRandom();
        roomNameTxt.text = afterRandom;
        roomMasterTxt.text = PhotonNetwork.NickName;
        MapChosenTxt.text = "None is choosing!";
    }

    public void OnClickMapChosen(TMP_Text map)
    {
        roomNameTxt.text = afterRandom + map.text; // the last digit is for choosing map
        MapChosenTxt.text = map.text;
    }

    // Close Create Room Panel
    public void OnClickBackCreatePanel()
    {
        openPanel.SetActive(false);
    }

    // Random a room number (dont let player to create a room name), it is a room with xxxx (0<x<5)
    public string roomRandom()
    {
        string roomNumber = "";
        Random random = new Random();
        for (int i = 0; i < 3; i++)
        {
            int digit = random.Next(1, 6);
            roomNumber += digit.ToString();
        }

        return roomNumber;
    }

    // waiting for another destiny
    /*public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        numberPlayer.text = PhotonNetwork.CurrentRoom.Players.Count.ToString() + "/2";
        StartCoroutine(LoadText());
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // Start loading the game scene
            StartCoroutine(LoadGameSceneAsync("Game"));
        } else
        {
            percentText.text = "Loading 0%";
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
    {
        numberPlayer.text = PhotonNetwork.CurrentRoom.Players.Count.ToString() + "/2";
    }*/

    // check the time for loading
    private IEnumerator LoadGameSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Normalize progress to 0-1 range

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                // Complete the loading when the other player enters
                progress = 1f;
            }
            else
            {
                // Limit the progress to 90% if the other player has not entered
                progress = Mathf.Clamp01(progress * 0.9f);
            }

            // Convert progress to percentage
            int percentage = Mathf.RoundToInt(progress * 100f);
            percentText.text = + percentage + "%";

            yield return null;
        }
    }

    private IEnumerator LoadText()
    {

        int dotCount = 0;
        string loadingTextBase = "Waiting";
        string dots = "";

        while (true)
        {
            if (dotCount < 3)
            {
                dots += ".";
                dotCount++;
            }
            else
            {
                dots = "";
                dotCount = 0;
            }

            loading.text = loadingTextBase + dots;

            yield return new WaitForSeconds(0.5f);
        }
    }

    // enter the lobby_dual after create a room successfully
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        PhotonNetwork.LoadLevel("MultiplayerLobby");
    }

    public void OnClickLeftRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        waitCanvas.SetActive(false);
    }


}
