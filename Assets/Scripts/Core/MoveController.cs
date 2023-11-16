using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviourPun
{
    private static GameManager gameManager;
    private static RPCManager rpcManager;
    private static Wire wireSpawner;
    bool totalCheck = true;
    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    private GameObject playerObject;
    private Player player;

    private int photonViewID;
    private bool isPauseGame, enableMove, isMoving;

    DimensionIn dimensionIn;
    DimensionOut dimensionOut;

    private bool allowInput = true;
    private float inputDelay = 0.3f; //adjust this for delay input
    private float inputDelayTimer = 0.0f;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;
    private float minSwipeDistance = 50f; // Adjust this threshold to your preference
    public int stepCount;

    // Start is called before the first frame update
    void Start()
    {
        stepCount = 0;
        dimensionIn = null;
        dimensionOut = null;
        isPauseGame = isMoving = false;
        enableMove = true;
        wireSpawner = GameObject.Find("WireSpawner").GetComponent<Wire>();
        photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        rpcManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<RPCManager>();
        player = this.GetComponent<Player>();
        Debug.Log("Player MoveController ID : " + photonViewID + " LocalActor Gender: " + PhotonNetwork.LocalPlayer.CustomProperties["Gender"]);
        Debug.Log($"PlayerM: {gameManager.PlayerM} + PlayerF: {gameManager.PlayerF}");
    }

    private void MovePlayer()
    {
        Vector3 newTargetPosition = new Vector3(player.TargetPosition.x, player.TargetPosition.y, player.DefaultZAxis);

        //update z
        if (player.transform.position.z != player.DefaultZAxis) player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.DefaultZAxis);
        
        player.transform.position = Vector3.MoveTowards(transform.position, newTargetPosition, moveSpeed * Time.deltaTime);

        if (player.transform.position == newTargetPosition && !enableMove)
        {
            //Debug.Log("Test call: " + player.transform.position + " == " + newTargetPosition);
            enableMove = true; // Re-enable movement once the target position is reached
            if (player.IsAtSocket)
            {
                Vector2 socketPos = newTargetPosition;
                if (dimensionIn != null) socketPos = dimensionIn.GetEntrancePosition(player.PreviousDirection);
                if (dimensionOut != null) socketPos = dimensionOut.GetExitPosition(player.PreviousDirection);
                rpcManager.CallChangePlayerColor(photonViewID, socketPos);
            }
            if (player.IsHandleWire || player.IsAtSocket) RenderWire();
            player.PreviousPosition = player.CurrentPosition;
            player.CurrentPosition = newTargetPosition;
            if (dimensionIn != null || dimensionOut != null)
            {
                TeleportPlayer();
            }
            GameObject item = GetItemAtPosition((Vector2)player.transform.position);
            if (item.tag != "Bridge") {
                player.DefaultZAxis = 6f;
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.DefaultZAxis);
            }
        }
    }

    private void TeleportPlayer()
    {
        if (dimensionIn != null)
        {
            Vector2 dimPos = dimensionIn.GetEntrancePosition(player.PreviousDirection);
            player.transform.position = new Vector3(dimPos.x, dimPos.y, player.transform.position.z);
            //dimensionIn = null;
            DimensionOut dOut = dimensionIn.GetDimensionOut(player.PreviousDirection).GetComponent<DimensionOut>();
            if (dOut.WireOnColor != null)
            {
                int rotationIndex = (dOut.OutDirection == "Top" || dOut.OutDirection == "Bottom") ? 1 : 2;
                rpcManager.CallRenderWire(dOut.transform.position, 7f, 0, rotationIndex, dOut.WireOnColor);
                Debug.Log("In color: " + dOut.WireOnColor);
            } //is handle wire

        } else
        {
            //player.transform.position = dimensionOut.GetExitPosition(player.PreviousDirection);
            Vector2 dimPos = dimensionOut.GetExitPosition(player.PreviousDirection);
            player.transform.position = new Vector3(dimPos.x, dimPos.y, player.transform.position.z);
            //dimensionOut = null;

            if (dimensionOut.WireOnColor != null)
            {
                int rotationIndex = (dimensionOut.OutDirection == "Top" || dimensionOut.OutDirection == "Bottom") ? 1 : 2;
                rpcManager.CallRenderWire(dimensionOut.transform.position, 7f, 0, rotationIndex, dimensionOut.WireOnColor);
                Debug.Log("Out color: " + dimensionOut.WireOnColor);
            } //is Handle wire
            
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

    private int CalculateRotationIndex(Vector2 dir)
    {
        if (dir == Vector2.up) //up
        {
            return 3;
        }
        else if (dir == Vector2.down) //down
        {
            return 1;
        }
        else if (dir == Vector2.left) //left
        {
            return 0;
        }
        else
            return 2;
    }

    private void RenderWire()
    {
        //Debug.Log("Prev Pos 1st: " + player.PreviousPosition);
        if (player.PreviousDirection == Vector2.zero) return;
        //Debug.Log("Wire is rendering at " + player.transform.position);
        string wireColor = player.HandleWireColor;
        Vector2 renderPosition = player.CurrentPosition;
        int rotationIndex = 0;

        Vector2 offset = player.TargetPosition - (Vector2)player.PreviousPosition;
        Vector2 offsetPrev = player.CurrentPosition - (Vector2)player.PreviousPosition;
        Vector2 offsetNext = player.TargetPosition - player.CurrentPosition;
        //Debug.Log("Prev Pos: " + player.PreviousPosition);
        int spriteIndex; // Default to straight wire sprite
        float wireZ = 7f; // Default z for wire
        /* check case render at bridge */
        if (player.DefaultZAxis != 6f)
        {
            GameObject targetItem = GetItemAtPosition(renderPosition);
            if (targetItem.tag == "Bridge")
            {
               wireZ = targetItem.GetComponent<Bridge>().GetWireZ(player.PreviousDirection);
            }
        }

        spriteIndex = 0; // Default to straight wire sprite
        if (offset == new Vector2(0, 2) || offset == Vector2.up) //up
        {
            rotationIndex = 1;
        }
        else if (offset == new Vector2(0, -2) || offset == Vector2.down) //down
        {
            rotationIndex = 3;
        }
        else if (offset == new Vector2(-2, 0) || offset == Vector2.left) //left
        {
            rotationIndex = 0; 
        }
        else if (offset == new Vector2(2, 0) || offset == Vector2.right) //right
        {
            rotationIndex = 2;
        }
        else
        {
            spriteIndex = 1; // Curve wire sprite
            if (offsetPrev == Vector2.left)
            {
                rotationIndex = offsetNext == Vector2.up ? 0 : 3;
            } else if (offsetPrev == Vector2.right)
            {
                rotationIndex = offsetNext == Vector2.up ? 1 : 2;
            } else if (offsetPrev == Vector2.up)
            {
                rotationIndex = offsetNext == Vector2.left ? 2 : 3;
            } else if (offsetPrev == Vector2.down)
            {
                rotationIndex = offsetNext == Vector2.left ? 1 : 0;
            }
            else
            {
                //Debug.Log("I can not render!" + offset + " - " + offsetPrev + " - " + offsetNext);
            }
        }

        if (player.HandleWireSteps == 1) //first step after pick wire
        {
            spriteIndex = 2;
            if (offsetNext == Vector2.up) //up
            {
                rotationIndex = 1;
            }
            else if (offsetNext == Vector2.down) //down
            {
                rotationIndex = 3;
            }
            else if (offsetNext == Vector2.left) //left
            {
                rotationIndex = 2;
            }
            else if (offsetNext == Vector2.right) //right
            {
                rotationIndex = 0;
            }
            //Debug.Log("It's after startpoint");
        }
        if (player.HandleWireSteps != 0) //if not start point
        {
            GameObject newWire = rpcManager.CallRenderWire(renderPosition, wireZ, spriteIndex, rotationIndex, wireColor);
        }
        /* ---- */
        if (player.HandleWireSteps != 0 && player.IsAtSocket) //endpoint
        {
            Vector2 socketPos = player.TargetPosition;
            if (dimensionIn != null) socketPos = dimensionIn.GetEntrancePosition(player.PreviousDirection);
            if (dimensionOut != null) socketPos = dimensionOut.GetExitPosition(player.PreviousDirection);
            GameObject itemCurrent = GetItemAtPosition(socketPos);
            Socket s = itemCurrent.GetComponent<Socket>();
            if (offsetNext == Vector2.up) //up
            {
                rotationIndex = 3;
            }
            else if (offsetNext == Vector2.down) //down
            {
                rotationIndex = 1;
            }
            else if (offsetNext == Vector2.left) //left
            {
                rotationIndex = 0;
            }
            else if (offsetNext == Vector2.right) //right
            {
                rotationIndex = 2;
            }
            wireColor = s.Color;
            //sprite index = 2
            //default z = 7
            rpcManager.CallRenderWire(socketPos, 7f, 2, rotationIndex, wireColor);
            //Debug.Log("It's endpoint");
            /* reset wire */
            player.HandleWireSteps = 0;
            player.HandleWireColor = "Default";
        }
        player.IsAtSocket = false;
        ++player.HandleWireSteps; //increase steps
        //Debug.Log("Step: " + player.HandleWireSteps);
    }

    private void Update()
    {
        if (photonView.IsMine)
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
                    this.transform.Find("PlayerInner").localScale = new Vector3(-1f, 1f, 1f);                 
                    
                }
                else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    moveDirection += Vector2.right;
                    this.transform.Find("PlayerInner").localScale = new Vector3(1f, 1f, 1f); 
                }

                if (moveDirection != Vector2.zero)
                {
                    // Debug.Log("Current: " + player.CurrentPosition);
                    if (moveDirection == Vector2.left) this.transform.Find("PlayerInner").localScale = new Vector3(-1f, 1f, 1f); 
                    if (moveDirection == Vector2.right) this.transform.Find("PlayerInner").localScale = new Vector3(1f, 1f, 1f); 
                    if (item.GetComponent<Bridge>() != null)
                    {
                        Bridge bridge = item.GetComponent<Bridge>();
                        bool isHorizontal = bridge.IsHorizontal();
                        bool isVertical = bridge.IsVertical();

                        if ((isHorizontal && player.DefaultZAxis == 2f && moveDirection.y != 0) ||
                            (isVertical && player.DefaultZAxis == 2f && moveDirection.x != 0) ||
                            (isHorizontal && player.DefaultZAxis == 5f && moveDirection.x != 0) ||
                            (isVertical && player.DefaultZAxis == 5f && moveDirection.y != 0))
                            return;

                    }
                    moveDirection.Normalize();
                    Vector2 newPosition = player.CurrentPosition + moveDirection * moveSteps;
                    //check if the next position is valid to move in or else it will return here
                    if (!IsPositionValid(newPosition, moveDirection)) return;
                    ++stepCount;
                    player.PreviousDirection = moveDirection;
                    player.TargetPosition = newPosition;
                    enableMove = false; // Disable movement until the target position is reached
                    allowInput = false; // Disable input for the delay periods
                }
            }
            MovePlayer();
        }
    }

    private bool HaveOtherPlayer(Vector2 targetPos)
    {
        //get other player
        if (gameManager.PlayerF == null || gameManager.PlayerM == null) return false;
        GameObject playerGO;
        if (PhotonNetwork.OfflineMode == true || gameManager.PlayerM.GetComponent<Player>().ID == photonViewID) playerGO = gameManager.PlayerM;
        else playerGO = gameManager.PlayerF;
        Player targetPlayer = playerGO.GetComponent<Player>();
        return ((Vector2)targetPlayer.transform.position == targetPos || targetPlayer.TargetPosition == targetPos);
    }

    private GameObject GetItemAtPosition(Vector2 pos)
    {
        int map = (int)pos.x / 100;
        int n = gameManager.PlayGridList[map].GetLength(0);
        int m = gameManager.PlayGridList[map].GetLength(1);
        int x = (int)pos.x % 100;
        int y = (int)pos.y;
        if (x < 0 || x >= n || y < 0 || y >= m) return null;
        return gameManager.PlayGridList[map][x, y];
    }

    private bool IsPositionValid(Vector2 targetPos, Vector2 moveDirection)
    {
        GameObject item = GetItemAtPosition(targetPos);
        if (item == null || item.tag == null) return false;
        string itemTag = item.tag;
        //Debug.Log(targetPos + " is a " + itemTag);
        //if found wire return false
        if (gameManager.WireMap.ContainsKey(targetPos) && player.IsHandleWire && itemTag != "Bridge") return false;
        if (HaveOtherPlayer(targetPos)) return false;

        if (itemTag == "Wall") {
            return false;
        }
        else if (itemTag == "Socket") {
            Socket socket = item.GetComponent<Socket>();
            bool ok = false;
            if (socket.CheckSocketStartPoint(player)) {
                rpcManager.CallUpdateSocket(photonViewID, targetPos);
                //rpcManager.CallChangePlayerColor(photonViewID, targetPos);
                ok = true;
            }
            else if (socket.CheckSocketEndPoint(player)) {
                rpcManager.CallUpdateSocket(photonViewID, targetPos);
                //rpcManager.CallChangePlayerColor(photonViewID, targetPos);
                rpcManager.CallAddScore();
                ok = true;
            }
            return (ok == true || !player.IsHandleWire);
        }
        else if (itemTag == "Bridge") {
            Bridge bridge = item.GetComponent<Bridge>();
            float newZ = bridge.GetZAxisToMove(player, moveDirection);
            if (newZ == -1f)
            {
                return false;
            }
            rpcManager.CallUpdateDefaultZAxis(photonViewID, newZ);
            return true;
        }
        else if (itemTag == "Ice") {
            // Wrong render
            isMoving = true;
            return true;
        }
        else if (itemTag == "DimensionIn") {
            DimensionIn dIn = item.GetComponent<DimensionIn>();
            Vector2 telePos = dIn.GetEntrancePosition(moveDirection);
            if (telePos == Vector2.zero)
            {
                return false;
            }
            if (player.IsHandleWire)
            {
                GameObject dOutGO = dIn.GetDimensionOut(moveDirection);
                dOutGO.GetComponent<DimensionOut>().WireOnColor = player.HandleWireColor;
            }
            if (IsPositionValid(telePos, moveDirection))
            {
                dimensionIn = dIn;
                return true;
            }
            return false;
        }
        else if (itemTag == "DimensionOut") {
            DimensionOut dOut = item.GetComponent<DimensionOut>();
            Vector2 telePos = dOut.GetExitPosition(moveDirection);
            if (telePos == Vector2.zero)
            {
                return false;
            }
            if (player.IsHandleWire)
            {
                dOut.WireOnColor = player.HandleWireColor;
            }
            if (IsPositionValid(telePos, moveDirection))
            {
                dimensionOut = dOut;
                return true;
            }
            return false;
        }
        else if (itemTag == "Door") {
            Door door = item.GetComponent<Door>();
            return door.IsValidPosition();
        }
        else if (itemTag == "DoorButton") {
            DoorButton doorButton = item.GetComponent<DoorButton>();
            return true;
        }
        else // Just ground
        {
            // Handle the default case for "Ground" here if needed
            // ...
        }

        return true;
    }
}