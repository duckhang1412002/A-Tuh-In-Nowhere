using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using TMPro;


public class LobbyMove : MonoBehaviourPunCallbacks
{
    private UIManager uiManager;
    Camera worldCamera;
    Camera playerCamera;

    Canvas canvas_screen, canvas_board_idle, canvas_board_sing, canvas_board_mult, canvas_board_crea, canvas_board_prof;
    private Dictionary<Vector2,GameObject> objectList = new Dictionary<Vector2,GameObject>();


    /*MoveController*/
    private bool isPauseGame, enableMove, isMoving;
    private Player player;
    private bool allowInput = true;
    private float inputDelay = 0.3f;
    private float inputDelayTimer = 0.0f;
    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;

    private GameObject playerHost;
    private GameObject playerClient;
    private static RPCManager rpcManager;
    private GameObject playerMapController;

    public static Vector2 PositionBeforePlay{get; set;}
    public static Vector2 PositionPlayMap{get; set;}
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;
    private float minSwipeDistance = 50f; // Adjust this threshold to your preference

    DimensionIn dimensionIn;
    DimensionOut dimensionOut;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        dimensionIn = null;
        dimensionOut = null;
        /*Player part*/
        player = this.gameObject.GetComponent<Player>();
        rpcManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<RPCManager>();
        playerMapController = GameObject.Find("PlayerMapController");
        if(SceneManager.GetActiveScene().name == "SingleLobby" || SceneManager.GetActiveScene().name == "MultiplayerLobby" ){
            if(PlayerMapController.MapID != -1){
                player.DefaultZAxis = 6f;
                player.transform.position = new Vector3(PositionPlayMap.x, PositionPlayMap.y, player.DefaultZAxis);
                player.CurrentPosition = PositionBeforePlay;
                player.TargetPosition = PositionBeforePlay;
                if(player.transform.position.x > player.TargetPosition.x){
                    player.transform.Find("PlayerInner").localScale = new Vector3(-1f, 1f, 1f);
                }
            }
        }
        if(SceneManager.GetActiveScene().name == "GameMode" && PlayerMapController.CurrentGameMode == "Single Mode"){
            player.DefaultZAxis = 6f;
            player.transform.position = new Vector3(-4f, -1f, player.DefaultZAxis);
            player.CurrentPosition = new Vector2(-4f,-1f);
            player.TargetPosition = new Vector2(-3f,-1f);
            player.transform.Find("PlayerInner").localScale = new Vector3(1f, 1f, 1f);
        } else if(SceneManager.GetActiveScene().name == "GameMode" && PlayerMapController.CurrentGameMode == "Multiplayer Mode"){
            player.DefaultZAxis = 6f;
            player.transform.position = new Vector3(4f, -1f, player.DefaultZAxis);
            player.CurrentPosition = new Vector2(4f,-1f);
            player.TargetPosition = new Vector2(3f,-1f);
            player.transform.Find("PlayerInner").localScale = new Vector3(-1f, 1f, 1f);
        } else if(SceneManager.GetActiveScene().name == "GameMode" && PlayerMapController.CurrentGameMode == "Creative Mode"){
            player.DefaultZAxis = 6f;
            player.transform.position = new Vector3(0f, -2f, player.DefaultZAxis);
            player.CurrentPosition = new Vector2(0f,-2f);
            player.TargetPosition = new Vector2(0f,-2f);
            player.transform.Find("PlayerInner").localScale = new Vector3(1f, 1f, 1f);
        }

        /*Add in-game interact object to Dictionary*/
        GameObject[] foundObjects = FindObjectsWithNameContaining("GameObj_Ground");
        foreach(GameObject i in foundObjects){
            objectList.Add(i.transform.position, i);
        }

        foundObjects = null;
        foundObjects = FindObjectsWithNameContaining("GameObj");
        foreach(GameObject i in foundObjects){
            if(i.name == "GameObj_Ground") continue;

            if(objectList.ContainsKey(i.transform.position)){
                objectList[i.transform.position] = i;
            } else {
                objectList.Add(i.transform.position, i);
            }
        }

        /*Get canvas*/
        canvas_screen = GameObject.Find("Canvas_Screen").GetComponent<Canvas>();
        canvas_screen.enabled = true;

        /*GameMode Canvas*/
        if(SceneManager.GetActiveScene().name == "GameMode"){
            canvas_board_idle = GameObject.Find("Canvas_Board_Idle").GetComponent<Canvas>();
            canvas_board_sing = GameObject.Find("Canvas_Board_Sing").GetComponent<Canvas>();
            canvas_board_mult = GameObject.Find("Canvas_Board_Mult").GetComponent<Canvas>();
            canvas_board_prof = GameObject.Find("Canvas_Board_Prof").GetComponent<Canvas>();
            canvas_board_crea = GameObject.Find("Canvas_Board_Crea").GetComponent<Canvas>();
            canvas_board_idle.enabled = true;
            canvas_board_sing.enabled = false;
            canvas_board_mult.enabled = false;
            canvas_board_crea.enabled = false;
            canvas_board_prof.enabled = false;

            canvas_board_prof.gameObject.transform.Find("Txt_PlayerName").gameObject.GetComponent<TextMeshProUGUI>().text = FirebaseAuthentication.currentAccount.Fullname;
        }

        /*Init 2 type cameras*/
        if(SceneManager.GetActiveScene().name == "GameMode"){
            worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            playerCamera = this.gameObject.transform.Find("Camera").gameObject.GetComponent<Camera>();

            worldCamera.enabled = true;
            playerCamera.enabled = false;
        }

        isPauseGame = isMoving = false;
        enableMove = true;

        /*Cutscene check part*/
        if(PlayerMapController.MapID == 1 && GameMode.ShowCutSceneMultiplayerMode && SceneManager.GetActiveScene().name == "SingleLobby"){
            GameMode.ShowCutSceneMultiplayerMode = false;
            StartCoroutine(LoadCutScene_1());
        } else if(PlayerMapController.MapID == 2 && GameMode.ShowCutSceneCreativeMode && SceneManager.GetActiveScene().name == "SingleLobby"){
            GameMode.ShowCutSceneCreativeMode = false;
            StartCoroutine(LoadCutScene_2());
        }

        //Reset the MapID to reset all effect
        PlayerMapController.MapID = -1;
    }

    private void Update()
    {
        /*Check if the PauseUI is activating, pause the game movement*/
        if (uiManager.GetPauseUI().activeSelf)
        {
            isPauseGame = true;
        }else{
            isPauseGame = false;
        }
        
        if (isPauseGame) return; // Disable movement game is paused
        if (!allowInput)
        {
            inputDelayTimer += Time.deltaTime;
            if (inputDelayTimer >= inputDelay)
            {
                inputDelayTimer = 0.0f;
                allowInput = true; // Enable input after the delay
            }
        }
        if (isMoving && enableMove) //case dashing in the same direction
        {
            Vector2 newPosition = player.CurrentPosition + player.PreviousDirection * moveSteps;
            //Debug.Log("is moving!");
            //check if the next position is valid to move in or else it will return here
            if (!IsPositionValid(newPosition, player.PreviousDirection))
            {
                //Debug.Log("not moving anymore!");
                isMoving = false;
                return;
            }
            enableMove = false;
            player.PreviousDirection = player.PreviousDirection;
            player.TargetPosition = newPosition;
        }
        else if (enableMove && allowInput && !isMoving)
        { // Enable move if player is allowed to move
            Vector2 moveDirection = Vector2.zero;
            GameObject item = GetItemAtPosition(player.CurrentPosition);

            // Check for touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // Record the start position of the touch
                        touchStartPos = touch.position;
                        isSwiping = true;
                        break;

                    case TouchPhase.Moved:
                        // Determine the direction of the swipe
                        touchEndPos = touch.position;
                        Vector2 swipeDirection = touchEndPos - touchStartPos;

                        // Check if the swipe distance exceeds the threshold
                        if (isSwiping && swipeDirection.magnitude > minSwipeDistance)
                        {
                            // Normalize the swipe direction to get a consistent movement speed
                            swipeDirection.Normalize();

                            // Determine the dominant axis of the swipe
                            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                            {
                                // Horizontal swipe
                                if (swipeDirection.x > 0)
                                {
                                    // Swipe right
                                    // Move your player right here
                                    moveDirection = Vector2.right;
                                }
                                else
                                {
                                    // Swipe left
                                    moveDirection = Vector2.left;
                                }
                            }
                            else
                            {
                                // Vertical swipe
                                if (swipeDirection.y > 0)
                                {
                                    // Swipe up
                                    moveDirection = Vector2.up;
                                }
                                else
                                {
                                    // Swipe down
                                    moveDirection = Vector2.down;
                                }
                            }

                            // Reset the isSwiping flag
                            isSwiping = false;
                        }
                        break;

                    case TouchPhase.Ended:
                        isSwiping = false;
                        break;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                moveDirection = Vector2.up;
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                moveDirection = Vector2.down;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                moveDirection += Vector2.left;
                player.transform.Find("PlayerInner").localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                moveDirection += Vector2.right;
                player.transform.Find("PlayerInner").localScale = new Vector3(1f, 1f, 1f);
            }

            if (moveDirection != Vector2.zero)
            {     
                if (moveDirection == Vector2.left) player.transform.Find("PlayerInner").localScale = new Vector3(-1f, 1f, 1f);
                if (moveDirection == Vector2.right) player.transform.Find("PlayerInner").localScale = new Vector3(1f, 1f, 1f);

                moveDirection.Normalize();
                Vector2 newPosition = player.CurrentPosition + moveDirection * moveSteps;
                //check if the next position is valid to move in or else it will return here
                if (!IsPositionValid(newPosition, moveDirection)) return;
                player.PreviousDirection = moveDirection;
                player.TargetPosition = newPosition;
                enableMove = false; // Disable movement until the target position is reached
                allowInput = false; // Disable input for the delay periods
            }
        }
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 newTargetPosition = new Vector3(player.TargetPosition.x, player.TargetPosition.y, player.DefaultZAxis);
        //Debug.Log(player.TargetPosition + "---NEW");
        
        player.transform.position = Vector3.MoveTowards(this.transform.position, newTargetPosition, moveSpeed * Time.deltaTime);

        if (player.transform.position == newTargetPosition && !enableMove)
        {
            //Debug.Log("Test call: " + player.transform.position + " == " + newTargetPosition);
            enableMove = true; // Re-enable movement once the target position is reached   

            player.PreviousPosition = player.CurrentPosition;
            player.CurrentPosition = newTargetPosition;
            // if (dimensionIn != null || dimensionOut != null)
            // {
            //     TeleportPlayer();
            // }

            if (dimensionIn != null || dimensionOut != null)
            {
                TeleportPlayer();
            }

            GameObject item = GetItemAtPosition((Vector2)player.transform.position);

            if(SceneManager.GetActiveScene().name == "GameMode" && item.name.Contains("Info")){
                if(item.name.Contains("Sing")){
                    canvas_board_idle.enabled = false;
                    canvas_board_sing.enabled = true;
                    canvas_board_mult.enabled = false;
                    canvas_board_prof.enabled = false;
                    canvas_board_crea.enabled = false;
                } else if(item.name.Contains("Mult")){
                    canvas_board_idle.enabled = false;
                    canvas_board_sing.enabled = false;
                    canvas_board_mult.enabled = true;
                    canvas_board_prof.enabled = false;
                    canvas_board_crea.enabled = false;
                } else if(item.name.Contains("Crea")){
                    canvas_board_idle.enabled = false;
                    canvas_board_sing.enabled = false;
                    canvas_board_mult.enabled = false;
                    canvas_board_prof.enabled = false;
                    canvas_board_crea.enabled = true;
                } else if(item.name.Contains("Prof")){
                    canvas_board_idle.enabled = false;
                    canvas_board_sing.enabled = false;
                    canvas_board_mult.enabled = false;
                    canvas_board_prof.enabled = true;
                    canvas_board_crea.enabled = false;
                } 
            }

            if(item.name.Contains("Entrance")){
                if(item.name.Contains("Sing")){
                    PlayerMapController.CurrentGameMode = "Single Mode";
                    SceneManager.LoadScene("SingleLobby");
                } else if(item.name.Contains("Mult")){
                    PlayerMapController.CurrentGameMode = "Multiplayer Mode";
                    SceneManager.LoadScene("Loading");
                } else {
                    PlayerMapController.CurrentGameMode = "Creative Mode";
                    SceneManager.LoadScene("CreativeLobby");
                }     
            } else if(item.name.Contains("Outrance")){
                if(item.name.Contains("Sing")){
                    SceneManager.LoadScene("GameMode");
                } else if(item.name.Contains("Mult")){
                    MultiplayerLobby.isFirstTimeJoinRoom = true;
                    PhotonNetwork.LeaveRoom();
                }
            } else if(item.name.Contains("Info")){
                if(SceneManager.GetActiveScene().name == "SingleLobby"){
                    if(item.name.Contains("Map")){
                        PlayerMapController.MapID = int.Parse(SplitText(item.name, 3));
                        PlayerMapController.RestartNumber = -1;
                        PlayerMapController.StepNumber = 0;
      
                        MapProjector currentProjector = GameObject.Find("GameObj_MapBlock_Map_" + PlayerMapController.MapID).GetComponent<MapProjector>();
                        uiManager.ShowConfirmMapUI(currentProjector);

                        PositionBeforePlay = player.PreviousPosition;    
                        PositionPlayMap = player.CurrentPosition;                
                    }  
                } else if(SceneManager.GetActiveScene().name == "MultiplayerLobby"){
                    if(item.name.Contains("GameObj_Info_Map")){
                        PlayerMapController.MapID = int.Parse(SplitText(item.name, 3));
                        PlayerMapController.RestartNumber = -1;
                        PlayerMapController.StepNumber = 0;
                        PlayerMapController.MapRole = SplitText(item.name, 4);
 
                        MapProjector currentProjector = GameObject.Find("GameObj_MapBlock_Map_" + PlayerMapController.MapID).GetComponent<MapProjector>();
                        uiManager.ShowConfirmMapUI(currentProjector);

                        PositionBeforePlay = player.PreviousPosition;
                        PositionPlayMap = player.CurrentPosition;   

                        /*Setup position for multiplayer mode when chosing previous play map >:*/
                        if(this.gameObject.name == "PlayerM(Clone)"){
                            MultiplayerLobby.playerM_Init_XPos = (int)PositionBeforePlay.x;
                            MultiplayerLobby.playerM_Init_YPos = (int)PositionBeforePlay.y;
                        } else {
                            MultiplayerLobby.playerF_Init_XPos = (int)PositionBeforePlay.x;
                            MultiplayerLobby.playerF_Init_YPos = (int)PositionBeforePlay.y;
                        }

                    }  else if(item.name.Contains("GameObj_Info_VSMap")) {
                        PlayerMapController.MapID = 0;
                        PlayerMapController.RestartNumber = -1;
                        PlayerMapController.StepNumber = 0;
                        PlayerMapController.MapRole = SplitText(item.name, 4);
 
                        MapProjector currentProjector = GameObject.Find("GameObj_MapBlock_VSMap_0").GetComponent<MapProjector>();
                        uiManager.ShowConfirmMapUI(currentProjector);

                        PositionBeforePlay = player.PreviousPosition;
                        PositionPlayMap = player.CurrentPosition;

                        /*Setup position for multiplayer mode when chosing previous play map >:*/
                        if(this.gameObject.name == "PlayerM(Clone)"){
                            MultiplayerLobby.playerM_Init_XPos = (int)PositionBeforePlay.x;
                            MultiplayerLobby.playerM_Init_YPos = (int)PositionBeforePlay.y;
                        } else {
                            MultiplayerLobby.playerF_Init_XPos = (int)PositionBeforePlay.x;
                            MultiplayerLobby.playerF_Init_YPos = (int)PositionBeforePlay.y;
                        }
                    }
                }
            }
        }
    }

    private string SplitText(string txt, int index){
        string[] splitArray = txt.Split('_');
        Debug.Log(splitArray[index]);
        return splitArray[index];
    }

    private IEnumerator LoadCutScene_1(){
        GameObject reactionObj = GameObject.Find("Reaction");
        reactionObj.GetComponent<Animator>().SetTrigger("Reaction-Popup");

        yield return new WaitForSeconds(1.5f);
        isPauseGame = true;
        Camera mainCamera = GameObject.Find("Camera_Single_1").GetComponent<Camera>();
        Camera cutScene_1 = GameObject.Find("Camera_Cut_1").GetComponent<Camera>();
        mainCamera.enabled = false;
        cutScene_1.enabled = true;

        PlayableDirector timeline = GameObject.Find("Camera_Cut_1").GetComponent<PlayableDirector>();
        timeline.Play();

        yield return new WaitForSeconds(6f);

        mainCamera.enabled = true;
        cutScene_1.enabled = false;
        isPauseGame = false;

        StartCoroutine(ShowNotification("Multiplayer Mode is unlocked!"));

        //TextMeshProUGUI txt_charMessage = GameObject.Find("Txt_CharMessage").GetComponent<TextMeshProUGUI>();
        //txt_charMessage.text = "Multiplayer Mode is unlocked!";
    }

    private IEnumerator LoadCutScene_2(){
        GameObject reactionObj = GameObject.Find("Reaction");
        reactionObj.GetComponent<Animator>().SetTrigger("Reaction-Popup");

        yield return new WaitForSeconds(1.5f);
        isPauseGame = true;
        Camera mainCamera = GameObject.Find("Camera_Single_2").GetComponent<Camera>();
        Camera cutScene_2 = GameObject.Find("Camera_Cut_2").GetComponent<Camera>();
        mainCamera.enabled = false;
        cutScene_2.enabled = true;

        PlayableDirector timeline = GameObject.Find("Camera_Cut_2").GetComponent<PlayableDirector>();
        timeline.Play();

        yield return new WaitForSeconds(7f);

        mainCamera.enabled = true;
        cutScene_2.enabled = false;
        isPauseGame = false;

        StartCoroutine(ShowNotification("Creative Mode is unlocked!"));

       // TextMeshProUGUI txt_charMessage = GameObject.Find("Txt_CharMessage").GetComponent<TextMeshProUGUI>();
        //txt_charMessage.text = "Creative Mode is unlocked!";
    }

    private IEnumerator ShowNotification(string content){
        GameObject notificationBoard = GameObject.Find("PlayerM").transform.Find("CharCanvas").gameObject;
        notificationBoard.SetActive(true);
        notificationBoard.transform.Find("CharBoard/Txt_CharMessage").GetComponent<TextMeshProUGUI>().text = content;

        yield return new WaitForSeconds(3f);
        notificationBoard.SetActive(false);
    }

    private GameObject GetItemAtPosition(Vector2 pos)
    {
        if(objectList.ContainsKey(pos)){
            return objectList[pos];
        } else 
            return null;
    }

    private bool IsPositionValid(Vector2 targetPos, Vector2 moveDirection)
    {
        GameObject item = GetItemAtPosition(targetPos);
        if(item == null) return false;
        string itemTag = item.name;
        //Debug.Log(targetPos + " is a " + itemTag);
        //if found wire return false
        
        //if (HaveOtherPlayer(targetPos)) return false;

        if (itemTag.Contains("Wall") || itemTag.Contains("Hidden")) {
            return false;        
        }
        else if (itemTag.Contains("MapBlock")) {
            return false;        
        }
        else if (itemTag.Contains("DimensionIn")){
            DimensionIn dIn = item.GetComponent<DimensionIn>();
            Vector2 telePos = dIn.GetEntrancePosition(moveDirection);
            if (telePos == Vector2.zero)
            {
                return false;
            }
            if (IsPositionValid(telePos, moveDirection))
            {
                dimensionIn = dIn;
                this.gameObject.transform.Find("PlayerInner").transform.Find("WholePlayerObject").gameObject.GetComponent<Animator>().SetTrigger("Smaller");
                return true;
            }
            return false;
        }
        else if (itemTag.Contains("DimensionOut")) {
            DimensionOut dOut = item.GetComponent<DimensionOut>();
            Vector2 telePos = dOut.GetExitPosition(moveDirection);
            if (telePos == Vector2.zero)
            {
                return false;
            }
            if (IsPositionValid(telePos, moveDirection))
            {
                dimensionOut = dOut;
                return true;
            }
            return false;
        } 
        else if(!itemTag.Contains("Info")){
            if(SceneManager.GetActiveScene().name == "GameMode"){
                canvas_board_idle.enabled = true;
                canvas_board_sing.enabled = false;
                canvas_board_mult.enabled = false;
                canvas_board_crea.enabled = false;
                canvas_board_prof.enabled = false;
            } else if (SceneManager.GetActiveScene().name == "SingleLobby" || SceneManager.GetActiveScene().name == "MultiplayerLobby"){
                uiManager.HideConfirmMapUI();
            }
        } 
        else // Just ground
        {
            // Handle the default case for "Ground" here if needed
            // ...
        }

        GameObject previousItem = GetItemAtPosition(player.transform.position);        
        if(SceneManager.GetActiveScene().name == "GameMode" && previousItem.name.Contains("CameraSwitch")){
            canvas_screen.renderMode = RenderMode.ScreenSpaceCamera;
            
            if(player.transform.position.y - item.transform.position.y < 0){
            /*Player go to entrance*/
            canvas_screen.worldCamera = playerCamera;
            playerCamera.enabled = true;
            worldCamera.enabled = false;

            } else{

            /*Player go back*/
            canvas_screen.worldCamera = worldCamera;
            playerCamera.enabled = false;
            worldCamera.enabled = true;
            }
        }

        return true;
    }

    private void TeleportPlayer()
    {
        CameraManager cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        if (dimensionIn != null)
        {
            Vector2 dimPos = dimensionIn.GetEntrancePosition(player.PreviousDirection);
            player.transform.position = new Vector3(dimPos.x, dimPos.y, player.transform.position.z);
            //dimensionIn = null;
            DimensionOut dOut = dimensionIn.GetDimensionOut(player.PreviousDirection).GetComponent<DimensionOut>();  
            cameraManager.SetupSingleplayerCamera(int.Parse(SplitText(dOut.gameObject.name, 2)), int.Parse(SplitText(dimensionIn.gameObject.name, 2)));    
        } else
        {
            //player.transform.position = dimensionOut.GetExitPosition(player.PreviousDirection);
            Vector2 dimPos = dimensionOut.GetExitPosition(player.PreviousDirection);
            player.transform.position = new Vector3(dimPos.x, dimPos.y, player.transform.position.z);
            //dimensionOut = null;  
            DimensionIn dIn = dimensionOut.GetDimensionIn();
            Debug.Log(int.Parse(SplitText(dimensionOut.gameObject.name, 2)) + " --- " + int.Parse(SplitText(dIn.gameObject.name, 2)));
            cameraManager.SetupSingleplayerCamera(int.Parse(SplitText(dIn.gameObject.name, 2)), int.Parse(SplitText(dimensionOut.gameObject.name, 2)));    
        }
        /* !!!!! CHECK HERE !!!!! */
        player.PreviousPosition =  CalculatePrevious(player.transform.position, player.PreviousDirection);
        
        //Debug.Log("Previous move after teleport:" + player.PreviousPosition);
        player.CurrentPosition = player.transform.position;
        player.TargetPosition = player.transform.position;
        //player.PreviousPosition = player.CurrentPosition = player.TargetPosition = player.transform.position;
        dimensionIn = null;
        dimensionOut = null;
    }

    private Vector2 CalculatePrevious(Vector2 pos, Vector2 dir)
    {
        if (dir == Vector2.up) return new Vector2(pos.x, pos.y - 1);
        if (dir == Vector2.down) return new Vector2(pos.x, pos.y + 1);
        if (dir == Vector2.left) return new Vector2(pos.x+1, pos.y);
        return new Vector2(pos.x-1, pos.y);
    }

    GameObject[] FindObjectsWithNameContaining(string partialName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Use LINQ to filter objects by name
        GameObject[] matchingObjects = allObjects.Where(obj => obj.name.Contains(partialName)).ToArray();

        return matchingObjects;
    }
}
