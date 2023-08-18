using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Step : MonoBehaviourPun
{
    private static GameManager gameManager;
    private static Wire wireSpawner; 
    bool totalCheck = true;
    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    private GameObject player;
    public Player playerScript, otherPlayerScript;
    private bool enableMove = true;
    private bool isPauseGame = false;
    private bool activatePipeEffect = false;
    private bool isStepOnIce = false;
    private bool isStepOnEscalator = false;

    int currentMap;
    int xCurrent;
    int yCurrent;
    int xTarget;
    int yTarget;

    [SerializeField]
    private PhotonView view;

    private int photonViewID;
    private RPCManager RPC;

    // Start is called before the first frame update
    void Start()
    {
        RPC = GameObject.FindGameObjectWithTag("GameController").GetComponent<RPCManager>();
        wireSpawner = GameObject.Find("WireSpawner").GetComponent<Wire>();
        Debug.Log(wireSpawner + " is found!");
        photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        Debug.Log("Photon View ID: " + photonViewID);
        if (view.IsMine)
        {
            if (photonViewID == 1) player = gameManager.PlayerM;
            else player = gameManager.PlayerF;
            playerScript = player.GetComponent<Player>();
            playerScript.HandleWireColor = "Default";

            currentMap = (int)playerScript.CurrentPosition.x / 100;
            xCurrent = (int)(playerScript.CurrentPosition.x % 100);
            yCurrent = (int)(playerScript.CurrentPosition.y);
            xTarget = (int)(playerScript.TargetPosition.x % 100);
            yTarget = (int)(playerScript.TargetPosition.y);

        }
    }

    //check if 2 player get the same function
    [PunRPC]
    private void LogMove()
    {
        Debug.Log(photonViewID + " move!");
    }

    [PunRPC]
    void CallChangePlayerAttrStartPoint(int currentMap, int xTarget, int yTarget, int photonTargetID)
    {
        Socket socket = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();
        //socket.ChangePlayerAttrStartPoint(playerScript);
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }
        socket.ChangePlayerAttrStartPoint(targetP);
        //Debug.Log("Socket found: " + socket + " - " + socket.transform.position);
    }

    [PunRPC]
    void CallChangePlayerAttrEndPoint(int currentMap, int xTarget, int yTarget, int photonTargetID)
    {
        Socket socket = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();
        //socket.ChangePlayerAttrStartPoint(playerScript);
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }
        socket.ChangePlayerAttrEndPoint(targetP);
        //Debug.Log("Socket found: " + socket + " - " + socket.transform.position);
    }

    [PunRPC]
    private void SetPreviousMove(int photonTargetID, string targetPreviousMove)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }
        targetP.PreviousMove = targetPreviousMove;
    }
    private void Update()
    {
        currentMap = (int)playerScript.CurrentPosition.x / 100;
        xCurrent = (int)(playerScript.CurrentPosition.x % 100);
        yCurrent = (int)(playerScript.CurrentPosition.y);
        xTarget = (int)(playerScript.TargetPosition.x % 100);
        yTarget = (int)(playerScript.TargetPosition.y);

        if (view.IsMine && player != null)
        {
            //check player move
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                view.RPC("LogMove", RpcTarget.Others);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPauseGame = !isPauseGame;
            }

            if (!isStepOnIce && Input.GetKeyDown(KeyCode.UpArrow) && enableMove && !isPauseGame)
            {
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Up");

                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Up";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Up");
                }
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.DownArrow) && enableMove && !isPauseGame)
            {
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Down");
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Down";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Down");
                }
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.LeftArrow) && enableMove && !isPauseGame)
            {
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Left");
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Left";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Left");
                }
                this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.RightArrow) && enableMove && !isPauseGame)
            {
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Right");
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Right";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Right");
                }
                this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (isStepOnEscalator && enableMove)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);

                if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Escalator")
                {
                    Escalator escalator = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Escalator>();
                    playerScript.TempTargetPosition = escalator.GetNextPosition(playerScript);
                    playerScript.TempNextKey = escalator.Direction;
                }
                else
                {
                    StopStepOnEscalator();
                    return;
                }

                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.PreviousMove))
                {
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, playerScript.TempNextKey);
                }
            }
            else if (isStepOnIce && enableMove)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
                if (playerScript.PreviousMove == "Left")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
                }
                else if (playerScript.PreviousMove == "Right")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
                }
                else if (playerScript.PreviousMove == "Up")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
                }
                else if (playerScript.PreviousMove == "Down")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
                }

                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.PreviousMove))
                {
                    //playerScript.PreviousMove = playerScript.PreviousMove;
                }
                else
                {
                    StopStepOnIce();
                }
            }
            StepMove();
        }
    }

    [PunRPC]
    private void SetTempTargetMove(int photonTargetID, string tempNextKey)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        if (tempNextKey == "Up")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
            targetP.TempNextKey = "Up";
        }
        else if (tempNextKey == "Left")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
            targetP.TempNextKey = "Left";
        }
        else if (tempNextKey == "Down")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
            targetP.TempNextKey = "Down";
        }
        else if (tempNextKey == "Right")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
            targetP.TempNextKey = "Right";
        }
    }

/*    [PunRPC]
    private void UpdateLocation(int photonTargetID)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1)
            {
                targetP = gameManager.PlayerM.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerM.transform.position.x), (float)Math.Round(gameManager.PlayerM.transform.position.y));
                targetP.TargetPosition = targetP.TempTargetPosition;
            }
            else
            {
                targetP = gameManager.PlayerF.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerF.transform.position.x), (float)Math.Round(gameManager.PlayerF.transform.position.y));
                targetP.TargetPosition = targetP.TempTargetPosition;
            }
        }
        if (view.IsMine)
        {
            playerScript.CurrentPosition = this.transform.position;
            playerScript.TargetPosition = playerScript.TempTargetPosition;
            currentMap = (int)playerScript.CurrentPosition.x / 100;
            xCurrent = (int)(playerScript.CurrentPosition.x % 100);
            yCurrent = (int)(playerScript.CurrentPosition.y);
            xTarget = (int)(playerScript.TargetPosition.x % 100);
            yTarget = (int)(playerScript.TargetPosition.y);
        }
    }*/

    [PunRPC]
    private void DimensionOutUpdateLocation(float tempTargetPositionX, float tempTargetPositionY, float objX, float objY, int photonTargetID){
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1)
            {
                targetP = gameManager.PlayerM.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerM.transform.position.x), (float)Math.Round(gameManager.PlayerM.transform.position.y));
                targetP.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.transform.position = new Vector3(objX, objY, targetP.DefaultZAxis);
            }
            else
            {
                targetP = gameManager.PlayerF.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerF.transform.position.x), (float)Math.Round(gameManager.PlayerF.transform.position.y));
                targetP.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.transform.position = new Vector3(objX, objY, targetP.DefaultZAxis);
            }
        }
        if (view.IsMine)
        {
        playerScript.CurrentPosition = this.transform.position;
        playerScript.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
        playerScript.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
        playerScript.transform.position = new Vector3(objX, objY, playerScript.DefaultZAxis);

        GameObject mainCamera = GameObject.Find("Main Camera");
        mainCamera.transform.position = playerScript.TargetPosition;

        int tempTargetMap = (int)playerScript.TargetPosition.x / 100;
        xCurrent = (int)(playerScript.CurrentPosition.x % 100);
        yCurrent = (int)(playerScript.CurrentPosition.y);
        xTarget = (int)(playerScript.TargetPosition.x % 100);
        yTarget = (int)(playerScript.TargetPosition.y);


        if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
        {
            //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
            RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
        }
        else
        {
            //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
            RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
        }
        currentMap = tempTargetMap;       
        }       
    }

    [PunRPC]
    private void DimensionInUpdateLocation(float tempTargetPositionX, float tempTargetPositionY, float objX, float objY, int photonTargetID){
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1)
            {
                targetP = gameManager.PlayerM.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerM.transform.position.x), (float)Math.Round(gameManager.PlayerM.transform.position.y));
                targetP.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.transform.position = new Vector3(objX, objY, targetP.DefaultZAxis);
            }
            else
            {
                targetP = gameManager.PlayerF.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerF.transform.position.x), (float)Math.Round(gameManager.PlayerF.transform.position.y));
                targetP.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                targetP.transform.position = new Vector3(objX, objY, targetP.DefaultZAxis);
            }
        }
        if (view.IsMine)
        {
            // playerScript.CurrentPosition = playerScript.transform.position;
            // playerScript.TempTargetPosition = tempTargetPosition;
            // playerScript.TargetPosition = tempTargetPosition;
            // playerScript.transform.position = new Vector3(dOut.transform.position.x, dOut.transform.position.y, playerScript.DefaultZAxis);

            playerScript.CurrentPosition = this.transform.position;
            playerScript.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
            playerScript.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
            playerScript.transform.position = new Vector3(objX, objY, playerScript.DefaultZAxis);

            GameObject mainCamera = GameObject.Find("Main Camera");
            mainCamera.transform.position = playerScript.TargetPosition;

            int tempTargetMap = (int)playerScript.TargetPosition.x / 100;
            xCurrent = (int)(playerScript.CurrentPosition.x % 100);
            yCurrent = (int)(playerScript.CurrentPosition.y);
            xTarget = (int)(playerScript.TargetPosition.x % 100);
            yTarget = (int)(playerScript.TargetPosition.y);
    

            if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
            {
                //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
            }
            else
            {
                //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
            }           
            currentMap = tempTargetMap;
        }
    }

    [PunRPC]
    void CallBridgeCheckNextStep(int mapIndex, int xAxis, int yAxis, int photonTargetID){
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        Bridge bridge = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<Bridge>();
        totalCheck = bridge.CheckNextStep(targetP);
    }

    [PunRPC]
    void CallBridgeCheckCurrentStep(int mapIndex, int xAxis, int yAxis, int photonTargetID){
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        Bridge bridge = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<Bridge>();
        totalCheck = bridge.CheckCurrentStep(targetP, targetP.PreviousMove);
    }

    [PunRPC]
    void CallDoorButtonCheckCurrentStep(int currentMap, int xCurrent, int yCurrent, int mapIndex, int xAxis, int yAxis, int photonTargetID)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        DoorButton button = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<DoorButton>();
        button.CheckCurrentStep(targetP, gameManager.PlayGridList[mapIndex][xAxis, yAxis], gameManager.WireMap);
        //totalCheck = bridge.CheckCurrentStep(targetP, targetP.PreviousMove);
    }

    [PunRPC]
    void CallDoorButtonCheckNextStep(int mapIndex, int xAxis, int yAxis, int photonTargetID)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        DoorButton button = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<DoorButton>();
        button.IsActive = true;
        totalCheck = button.CheckNextStep(targetP);
        //totalCheck = bridge.CheckCurrentStep(targetP, targetP.PreviousMove);
    }

    [PunRPC]
    void CallDoorCheckNextStep(int mapIndex, int xAxis, int yAxis, int photonTargetID)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        Door door = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<Door>();
        totalCheck = door.CheckNextStep(targetP);
    }

    [PunRPC]
    private void CallChangeDoorAttr(int mapIndex, int xAxis, int yAxis, int photonTargetID){
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        Door door = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<Door>();
        if(door.HasPlayerAtDoorPosition) door.HasPlayerAtDoorPosition = false;
    }

    [PunRPC]
    private void CallPlusScore(){
        gameManager.Score++;
    }

    private bool CanStepToPosition(Vector2 currentPosition, Vector2 targetPosition, string tempNextKey)
    {    
        totalCheck = true;   
        currentMap = (int)currentPosition.x / 100;
        xCurrent = (int)(currentPosition.x % 100);
        yCurrent = (int)(currentPosition.y);
        xTarget = (int)(targetPosition.x % 100);
        yTarget = (int)(targetPosition.y);

        //Debug.Log(xTarget + " ---- " + yTarget + gameManager.PlayGridList[currentMap][xTarget, yTarget].tag);     

        //check current position
        if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
        {
            Bridge bridge = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<Bridge>();
            view.RPC("CallBridgeCheckCurrentStep", RpcTarget.All, currentMap, xCurrent, yCurrent, photonViewID);
            if (!totalCheck)
            {
                return false;
            }
        }
        else if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "DoorButton")
        {
            int tempCurrentMap = (int)targetPosition.x / 100;
            int tempXTarget = (int)(targetPosition.x % 100);
            int tempYTarget = (int)(targetPosition.y);
            //DoorButton button = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<DoorButton>();
            //button.CheckCurrentStep(playerScript, gameManager.PlayGridList[tempCurrentMap][tempXTarget, tempYTarget], gameManager.WireMap);
            view.RPC("CallDoorButtonCheckCurrentStep", RpcTarget.All, currentMap, xCurrent, yCurrent, tempCurrentMap, tempXTarget, tempYTarget, photonViewID);
        }
        else if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Teleporter")
        {
            Teleporter teleporter = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<Teleporter>();

            Vector3 tempTargetPosition = teleporter.GetNextPositionInside(playerScript);
            int tempCurrentMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = teleporter.CheckNextStepInside(playerScript, gameManager.PlayGridList[tempCurrentMap][tempXTarget, tempYTarget], gameManager.WireMap);

            if (totalCheck)
            {
                if (teleporter.IsTeleport(tempTargetPosition))
                {
                    playerScript.CurrentPosition = playerScript.transform.position;
                    playerScript.TargetPosition = tempTargetPosition;
                    playerScript.transform.position = tempTargetPosition;

                    tempCurrentMap = (int)playerScript.TargetPosition.x / 100;
                    xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                    yCurrent = (int)(playerScript.CurrentPosition.y);
                    xTarget = (int)(playerScript.TargetPosition.x % 100);
                    yTarget = (int)(playerScript.TargetPosition.y);

                    if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                        //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                        RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    }
                    else
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                        //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                        RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    }

                    if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                    {
                        Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                        if (socket.CheckSocketEndPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrEndPoint(playerScript);
                            view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                            //GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                            //view.RPC("GenerateWire", RpcTarget.All, tempCurrentMap, xTarget, yTarget, "Wire", photonViewID);
                            RPC.CallGenerateWire(tempCurrentMap, xTarget, yTarget, "Wire", photonViewID);
                            view.RPC("CallPlusScore", RpcTarget.All);
                        }
                        else if (socket.CheckSocketStartPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrStartPoint(playerScript);    
                            view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                        }
                    }
                    return true;
                }
            }
        }

        if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DimensionIn")
        {
            DimensionIn dIn = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<DimensionIn>();
            GameObject dOut = dIn.GetDimensionOut(playerScript);

            Vector3 tempTargetPosition = dIn.GetNextPosition(playerScript);

            if(tempTargetPosition == gameManager.PlayGridList[currentMap][xTarget, yTarget].transform.position) 
                return false;

            int tempTargetMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = dIn.CheckNextStep(playerScript, gameManager.PlayGridList[tempTargetMap][tempXTarget, tempYTarget], gameManager.WireMap);

            if (totalCheck)
            {
                view.RPC("DimensionInUpdateLocation", RpcTarget.All, tempTargetPosition.x, tempTargetPosition.y, dOut.transform.position.x, dOut.transform.position.y, photonViewID);
                view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, tempNextKey);
            } else{
                return false;
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DimensionOut")
        {
            DimensionOut dOut = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<DimensionOut>();
            DimensionIn dIn = dOut.BaseDimension;
            int objCurrentMap = currentMap;
            int objX = xTarget;
            int objY = yTarget;

            Vector3 tempTargetPosition = dOut.GetNextPosition(playerScript);
            int tempTargetMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = dOut.CheckNextStep(playerScript, gameManager.PlayGridList[tempTargetMap][tempXTarget, tempYTarget], gameManager.WireMap);

            if (totalCheck)
            {           
                view.RPC("DimensionOutUpdateLocation", RpcTarget.All, tempTargetPosition.x, tempTargetPosition.y, dIn.transform.position.x, dIn.transform.position.y, photonViewID);
                //view.RPC("GenerateWire", RpcTarget.All, objCurrentMap, objX, objY, "Dimension", photonViewID);
                RPC.CallGenerateWire(objCurrentMap, objX, objY, "Dimension", photonViewID);
                view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, tempNextKey);    
            }
            else{
                return false;
            }
        }
  
        //check target posotion
        if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Bridge")
        {
            Bridge bridge = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Bridge>();
            view.RPC("CallBridgeCheckNextStep", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
            if (totalCheck)
            {
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                }
                else
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                }

                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, playerScript.DefaultZAxis);

            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Socket")
        {
            totalCheck = false;
            Socket socket = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();

            if (playerScript.IsNotPickWire && socket.IsConnect)
            {
                totalCheck = true;
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
            }
            else if (socket.CheckSocketEndPoint(playerScript))
            {
                totalCheck = true;
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                }
                else
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                }
                //socket.ChangePlayerAttrEndPoint(playerScript);
                view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                //GenerateWire(currentMap, xTarget, yTarget, "Wire", null);
                //view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Wire", photonViewID);
                RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                view.RPC("CallPlusScore", RpcTarget.All);
            }
            else if (socket.CheckSocketStartPoint(playerScript))
            {
                totalCheck = true;
                //socket.ChangePlayerAttrStartPoint(playerScript);
                view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Wall")
        {
            totalCheck = false;
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DoorButton")
        {
/*            DoorButton button = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<DoorButton>();
            button.IsActive = true;

            totalCheck = button.CheckNextStep(playerScript);*/
            view.RPC("CallDoorButtonCheckNextStep", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
            if (totalCheck)
            {
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
                //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Ice")
        {
            IcePallete icePallete = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<IcePallete>();
            totalCheck = icePallete.CheckNextStep(playerScript, gameManager.WireMap);

            if (totalCheck)
            {
                moveSpeed = 7f;
                isStepOnIce = true;
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
                //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
            }
        }
        else if (gameManager.WireMap.ContainsKey(targetPosition) && !playerScript.IsNotPickWire)
        {
            if (!playerScript.IsNotPickWire) totalCheck = false;
        }
/*        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DoorButton")
        {
            DoorButton button = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<DoorButton>();
            button.IsActive = true;
            view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
            //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
            view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
        }*/
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Door")
        {
            view.RPC("CallDoorCheckNextStep", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);

            if (totalCheck)
            {
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
                //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Teleporter")
        {
            Teleporter teleporter = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Teleporter>();

            Vector3 tempTargetPosition = teleporter.GetNextPositionOutside(playerScript);
            int tempCurrentMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = teleporter.CheckNextStepOutside(playerScript, gameManager.WireMap);

            if (totalCheck)
            {
                if (teleporter.IsTeleport(tempTargetPosition))
                {
                    playerScript.CurrentPosition = playerScript.transform.position;
                    playerScript.TargetPosition = tempTargetPosition;
                    playerScript.transform.position = tempTargetPosition;

                    tempCurrentMap = (int)playerScript.TargetPosition.x / 100;
                    xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                    yCurrent = (int)(playerScript.CurrentPosition.y);
                    xTarget = (int)(playerScript.TargetPosition.x % 100);
                    yTarget = (int)(playerScript.TargetPosition.y);

                    if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                        //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                        RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    }
                    else
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                        //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                        RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    }

                    if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                    {
                        Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                        if (socket.CheckSocketEndPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrEndPoint(playerScript);
                            view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                            //GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                            //view.RPC("GenerateWire", RpcTarget.All, tempCurrentMap, xCurrent, yCurrent, "Wire", photonViewID);
                            RPC.CallGenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", photonViewID);
                            view.RPC("CallPlusScore", RpcTarget.All);
                        }
                        else if (socket.CheckSocketStartPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrStartPoint(playerScript);
                            view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                        }
                    }
                }
                else
                {
                    //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                    RPC.CallUpdateLocation(photonViewID);
                    if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                        //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                        RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    }
                    else
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                        //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                        RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    }
                }
            }
        }
        else
        {
            if (totalCheck)
            {
                //view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                RPC.CallUpdateLocation(photonViewID);
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                }
                else
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                }               
            }
        }

        if (!totalCheck)
        {
            if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
            {
                Bridge bridge = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<Bridge>();
                bridge.CheckOpacity(playerScript, playerScript.PreviousMove);
            }
        } else{
            if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Door")
            {
                //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                view.RPC("CallChangeDoorAttr", RpcTarget.All, currentMap, xCurrent, yCurrent, photonViewID);
            }
        }
        return totalCheck;
    }

    void StepMove()
    {
        player.transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerScript.TargetPosition.x, playerScript.TargetPosition.y, playerScript.DefaultZAxis), moveSpeed * Time.deltaTime);
        if (new Vector2(player.transform.position.x, player.transform.position.y) != playerScript.TargetPosition)
        {
            enableMove = false;
        }
        else
        {
            enableMove = true;
        }
    }

    private void StopStepOnIce()
    {
        isStepOnIce = false;
        moveSpeed = 5f;
    }

    private void StopStepOnEscalator()
    {
        isStepOnEscalator = false;
        if (isStepOnIce)
            moveSpeed = 7f;
        else
            moveSpeed = 5f;
    }
}