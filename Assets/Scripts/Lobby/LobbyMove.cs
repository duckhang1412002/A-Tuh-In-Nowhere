using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;


public class LobbyMove : MonoBehaviourPunCallbacks
{
    Camera worldCamera;
    Camera playerCamera;

    Canvas canvas_screen, canvas_board_idle, canvas_board_sing, canvas_board_mult, canvas_board_prof;
    private Dictionary<Vector2,GameObject> objectList = new Dictionary<Vector2,GameObject>();


    /*MoveController*/
    private bool isPauseGame, enableMove, isMoving;
    private Player player;
    private bool allowInput = true;
    private float inputDelay = 0.3f;
    private float inputDelayTimer = 0.0f;
    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;

    [SerializeField] private GameObject playerMapController;

    private GameObject playerHost;
    private GameObject playerClient;
    private static RPCManager rpcManager;


    // Start is called before the first frame update
    void Start()
    {
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
            canvas_board_idle.enabled = true;
            canvas_board_sing.enabled = false;
            canvas_board_mult.enabled = false;
            canvas_board_prof.enabled = false;
        }

        /*Init 2 type cameras*/
        worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCamera = this.gameObject.transform.Find("Camera").gameObject.GetComponent<Camera>();

        worldCamera.enabled = true;
        playerCamera.enabled = false;

        isPauseGame = isMoving = false;
        enableMove = true;

        /*Player part*/
        player = this.GetComponent<Player>();
        rpcManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<RPCManager>();
    }

    private void Update()
    {
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
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection = Vector2.up;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection = Vector2.down;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection += Vector2.left;
                this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection += Vector2.right;
                this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

            if (moveDirection != Vector2.zero)
            {     
                if (moveDirection == Vector2.left) this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
                if (moveDirection == Vector2.right) this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

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
        
        player.transform.position = Vector3.MoveTowards(transform.position, newTargetPosition, moveSpeed * Time.deltaTime);

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
            GameObject item = GetItemAtPosition((Vector2)player.transform.position);

            if(item.name.Contains("Info")){
                if(SceneManager.GetActiveScene().name == "GameMode"){
                    if(item.name.Contains("Sing")){
                        canvas_board_idle.enabled = false;
                        canvas_board_sing.enabled = true;
                        canvas_board_mult.enabled = false;
                        canvas_board_prof.enabled = false;
                    } else if(item.name.Contains("Mult")){
                        canvas_board_idle.enabled = false;
                        canvas_board_sing.enabled = false;
                        canvas_board_mult.enabled = true;
                        canvas_board_prof.enabled = false;
                    } else {
                        canvas_board_idle.enabled = false;
                        canvas_board_sing.enabled = false;
                        canvas_board_mult.enabled = false;
                        canvas_board_prof.enabled = true;
                    }
                }
                if(item.name.Contains("Cut")){
                    StartCoroutine(LoadCutScene());
                }              
            }

            if(item.name.Contains("Entrance")){
                if(item.name.Contains("Sing")){
                    SceneManager.LoadScene("SingleLobby");
                } else if(item.name.Contains("Mult")){
                    SceneManager.LoadScene("CreativeLobby");
                } else {

                }     
            } else if(item.name.Contains("Outrance")){
                if(item.name.Contains("Sing")){
                    SceneManager.LoadScene("GameMode");
                }  
            } else if(item.name.Contains("Info")){
                if(SceneManager.GetActiveScene().name == "SingleLobby"){
                    if(item.name.Contains("Map")){
                        PlayerMapController.MapID = int.Parse(SplitText(item.name, 3));
                        PlayerMapController.RestartNumber = -1;
                        PlayerMapController.StepNumber = 0;

                        playerMapController.GetComponent<PlayerMapController>().ShowConfirmMapUI();           
                    }  
                } else if(SceneManager.GetActiveScene().name == "MultiplayerLobby"){
                    if(item.name.Contains("Map")){
                        PlayerMapController.MapID = int.Parse(SplitText(item.name, 3));
                        PlayerMapController.RestartNumber = -1;
                        PlayerMapController.StepNumber = 0;

                        playerMapController.GetComponent<PlayerMapController>().ShowConfirmMapUI();           
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

    private IEnumerator LoadCutScene(){
        Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        Camera cutScene_1 = GameObject.Find("Camera_Cut_1").GetComponent<Camera>();
        mainCamera.enabled = false;
        cutScene_1.enabled = true;

        PlayableDirector timeline = GameObject.Find("Camera_Cut_1").GetComponent<PlayableDirector>();
        timeline.Play();

        yield return new WaitForSeconds(6f);

        mainCamera.enabled = true;
        cutScene_1.enabled = false;
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
        string itemTag = item.name;
        Debug.Log(item.transform.position);
        //Debug.Log(targetPos + " is a " + itemTag);
        //if found wire return false
        
        //if (HaveOtherPlayer(targetPos)) return false;

        if (itemTag.Contains("Wall")) {
            return false;        
        }
        else if (itemTag.Contains("MapBlock")) {
            return false;        
        }
        else if(!itemTag.Contains("Info")){
            if(SceneManager.GetActiveScene().name == "GameMode"){
                canvas_board_idle.enabled = true;
                canvas_board_sing.enabled = false;
                canvas_board_mult.enabled = false;
                canvas_board_prof.enabled = false;
            } else if (SceneManager.GetActiveScene().name == "SingleLobby"){
                GameObject.Find("UIManager").GetComponent<UIManager>().HideConfirmMapUI();
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

    GameObject[] FindObjectsWithNameContaining(string partialName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Use LINQ to filter objects by name
        GameObject[] matchingObjects = allObjects.Where(obj => obj.name.Contains(partialName)).ToArray();

        return matchingObjects;
    }

    // private bool HaveOtherPlayer(Vector2 targetPos)
    // {
    //     //get other player
    //     if (gameManager.PlayerF == null) return false;
    //     GameObject player = (photonViewID == 1) ? gameManager.PlayerF : gameManager.PlayerM;
    //     return (Vector2)player.transform.position == targetPos;
    // }
}
