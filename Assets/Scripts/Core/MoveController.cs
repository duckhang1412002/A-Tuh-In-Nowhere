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
using static UnityEngine.GraphicsBuffer;

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

    private bool allowInput = true;
    private float inputDelay = 0.35f;
    private float inputDelayTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        isPauseGame = isMoving = false;
        enableMove = true;
        wireSpawner = GameObject.Find("WireSpawner").GetComponent<Wire>();
        photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        rpcManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<RPCManager>();
        player = this.GetComponent<Player>();
        Debug.Log("Player : " + player);

        /*
                    currentMap = (int)playerScript.CurrentPosition.x / 100;
                    xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                    yCurrent = (int)(playerScript.CurrentPosition.y);
                    xTarget = (int)(playerScript.TargetPosition.x % 100);
                    yTarget = (int)(playerScript.TargetPosition.y);*/
    }

    private Player GetTargetPlayer(int photonTargetID)
    {
        if (photonTargetID == photonViewID)
        {
            return player;
        }
        else if (photonTargetID == 1)
        {
            return gameManager.PlayerM.GetComponent<Player>();
        }
        else
        {
            return gameManager.PlayerF.GetComponent<Player>();
        }
    }
    private void MovePlayer()
    {
        Vector3 newTargetPosition = new Vector3(player.TargetPosition.x, player.TargetPosition.y, player.DefaultZAxis);

        player.transform.position = Vector3.MoveTowards(transform.position, newTargetPosition, moveSpeed * Time.deltaTime);

        if (player.transform.position == newTargetPosition && !enableMove)
        {
            enableMove = true; // Re-enable movement once the target position is reached
            if (player.IsHandleWire || player.IsAtSocket) RenderWire();
            player.PreviousPosition = player.CurrentPosition;
            player.CurrentPosition = newTargetPosition;
        }
    }

    private void RenderWire()
    {
        if (player.PreviousDirection == Vector2.zero) return;
        player.IsAtSocket = false;
        string wireColor = player.HandleWireColor;
        Vector2 renderPosition = player.CurrentPosition;
        int rotationIndex = 0;

        Vector2 offset = player.TargetPosition - player.PreviousPosition;
        Vector2 offsetPrev = player.CurrentPosition - player.PreviousPosition;
        Vector2 offsetNext = player.TargetPosition - player.CurrentPosition;

        int spriteIndex = 0; // Default to straight wire sprite
        /* do render for socket */
        if (player.HandleWireSteps == -1) //endpoint
        {
            GameObject itemCurrent = GetItemAtPosition(player.TargetPosition);
            Socket s = itemCurrent.GetComponent<Socket>();
            spriteIndex = 2;
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
            rpcManager.CallRenderWire(player.TargetPosition, player.DefaultZAxis+1, spriteIndex, rotationIndex, wireColor);
            Debug.Log("It's endpoint");
        }

        spriteIndex = 0; // Default to straight wire sprite
        if (offset == new Vector2(0, 2)) //up
        {
            rotationIndex = 1;
        }
        else if (offset == new Vector2(0, -2)) //down
        {
            rotationIndex = 3;
        }
        else if (offset == new Vector2(-2, 0)) //left
        {
            rotationIndex = 0; 
        }
        else if (offset == new Vector2(2, 0)) //right
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
                Debug.Log("I can not render!");
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
            Debug.Log("It's after startpoint");
        } 
        /* ---- */

        if (player.HandleWireSteps != 0) //if not start point
        {
            GameObject newWire = rpcManager.CallRenderWire(renderPosition, player.DefaultZAxis+1, spriteIndex, rotationIndex, wireColor);
        }    
        ++player.HandleWireSteps; //increase steps
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
        if (enableMove && allowInput)
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
                player.PreviousDirection = moveDirection;
                player.TargetPosition = newPosition;
                enableMove = false; // Disable movement until the target position is reached
                allowInput = false; // Disable input for the delay periods
            }
        }
        MovePlayer();
    }

    private GameObject GetItemAtPosition(Vector2 pos)
    {
        int map = (int)pos.x / 100;
        int n = gameManager.PlayGridList[map].GetLength(0);
        int m = gameManager.PlayGridList[map].GetLength(1);
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (x < 0 || x >= n || y < 0 || y >= m) return null;
        return gameManager.PlayGridList[map][x, y];
    }

    private bool IsPositionValid(Vector2 targetPos, Vector2 moveDirection)
    {
        GameObject item = GetItemAtPosition(targetPos);
        if (item == null || item.tag == null) return false;
        string itemTag = item.tag;

        //if found wire return false
        if (gameManager.WireMap.ContainsKey(targetPos) && player.IsHandleWire && itemTag != "Bridge") return false;

        switch (itemTag)
        {
            case "Wall":
                return false;
            case "Socket":
                Socket socket = item.GetComponent<Socket>();
                if (socket.CheckSocketStartPoint(player)) {
                    rpcManager.CallChangePlayerColor(photonViewID, targetPos);
                    return true;
                } else if (socket.CheckSocketEndPoint(player)) {
                    rpcManager.CallChangePlayerColor(photonViewID, targetPos);
                    rpcManager.CallAddScore();
                    return true;
                }
                return true;
            case "Bridge":
                Bridge bridge = item.GetComponent<Bridge>();
                float newZ = bridge.GetZAxisToMove(player, moveDirection);
                if (newZ == -1f) return false;
                rpcManager.CallUpdateDefaultZAxis(photonViewID, newZ);
                return true;
            default: //just ground
                break;
        }
        //check if there is a wire

        return true;
    }
}