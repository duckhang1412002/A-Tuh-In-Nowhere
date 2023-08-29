using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using static UnityEngine.GraphicsBuffer;
using System;
using Unity.VisualScripting;

public class RPCManager : MonoBehaviourPunCallbacks
{
    private PhotonView view;
    private GameManager gameManager;
    private Wire wireManager;
    private int photonViewID;
    GameObject newWire;
    public void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        gameManager= this.gameObject.GetComponent<GameManager>();
        photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        wireManager = this.gameObject.GetComponent<Wire>();
        
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

    private Player GetPlayerByPhotonID(int photonViewID)
    {
        GameObject player = (photonViewID == 1) ? gameManager.PlayerM : gameManager.PlayerF;
        return player.GetComponent<Player>();
    }

    public void CallAddScore()
    {
        view.RPC("AddScore", RpcTarget.All);
    }

    [PunRPC]
    private void AddScore()
    {
        gameManager.Score++;
    }

    public GameObject CallRenderWire(Vector2 playerPosition, float z, int spriteIndex, int rotationIndex, string color)
    {
        newWire = null;
        view.RPC("RenderWire", RpcTarget.All, playerPosition.x, playerPosition.y, z, spriteIndex, rotationIndex, color);
        if (newWire != null)
        {
            view.RPC("UpdateMapWire", RpcTarget.All);
        }
        return newWire;
    }

    [PunRPC]
    private void RenderWire(float x, float y, float z, int spriteIndex, int rotationIndex, string color)
    {
        newWire = wireManager.RenderWire(new Vector3(x, y, z), spriteIndex, rotationIndex, color);
    }

    [PunRPC]
    private void UpdateMapWire()
    {
        gameManager.WireMap[(Vector2)newWire.transform.position] = true;
    }

    public void CallChangePlayerColor(int photonViewID, Vector2 socketPos)
    {
        view.RPC("ChangePlayerColor", RpcTarget.All, photonViewID, socketPos.x, socketPos.y);
    }

    [PunRPC]
    private void ChangePlayerColor(int photonViewId, float x, float y) {
        Player player = GetPlayerByPhotonID(photonViewId);
        Socket socket = GetItemAtPosition(new Vector2(x, y)).GetComponent<Socket>();
        socket.ChangePlayerColor(player);
    }

    public void CallUpdateDefaultZAxis(int photonViewID, float z)
    {
        view.RPC("UpdateDefaultZAxis", RpcTarget.All, photonViewID, z);
    }


    [PunRPC]
    private void UpdateDefaultZAxis(int photonViewId, float z)
    {
        Player player = GetPlayerByPhotonID(photonViewId);
        player.DefaultZAxis = z;
    }
    /*

        public void CallGenerateWire(int mapIndex, int xAxis, int yAxis, string type, int photonTargetID)
        {
            view.RPC("GenerateWire", RpcTarget.All, mapIndex, xAxis, yAxis, type, photonTargetID);
        }

        [PunRPC]
        private void GenerateWire(int mapIndex, int xAxis, int yAxis, string type, int photonTargetID)
        {
            Player targetP = GetPlayerByPhotonID(photonTargetID);

            Wire wireSpawner = GameObject.Find("WireSpawner").GetComponent<Wire>();

            if (type == "Bridge" && !targetP.IsNotPickWire)
            {
                wireSpawner.GetComponent<Wire>().wireZAxis = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<Bridge>().GetZAxisWire(targetP.PreviousMove);
                GameObject w = wireSpawner.GenerateWire(targetP);
                wireSpawner.GetComponent<Wire>().wireZAxis = 7f;
            }
            else if (type == "Wire" && !targetP.IsNotPickWire || targetP.IsAtSocket)
            {
                GameObject w = wireSpawner.GenerateWire(targetP);
                Vector2 wirePosition = new Vector2(w.transform.position.x, w.transform.position.y);
                gameManager.WireMap[wirePosition] = true;
            }
            else if (type == "Dimension" && !targetP.IsNotPickWire)
            {
                int wireRotation = (targetP.TempNextKey == "Up" || targetP.TempNextKey == "Down") ? 1 : 0;

                Vector2 renderPosition = gameManager.PlayGridList[mapIndex][xAxis, yAxis].transform.position;
                GameObject w = wireSpawner.RenderWire(renderPosition, 0, wireRotation, targetP.HandleWireColor);
            }
        }

        public void CallUpdateLocation(int photonTargetID)
        {
            view.RPC("UpdateLocation", RpcTarget.All, photonTargetID);
        }

        [PunRPC]
        private void UpdateLocation(int photonTargetID)
        {
            Player targetP = GetPlayerByPhotonID(photonTargetID);
            if (photonTargetID == 1)
            {
                //targetP = gameManager.PlayerM.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerM.transform.position.x), (float)Math.Round(gameManager.PlayerM.transform.position.y));
                targetP.TargetPosition = targetP.TempTargetPosition;
            }
            else
            {
                //targetP = gameManager.PlayerF.GetComponent<Player>();
                targetP.CurrentPosition = new Vector2((float)Math.Round(gameManager.PlayerF.transform.position.x), (float)Math.Round(gameManager.PlayerF.transform.position.y));
                targetP.TargetPosition = targetP.TempTargetPosition;
            }
        }

        public void CallDimensionOutUpdateLocation(float tempTargetPositionX, float tempTargetPositionY, float objX, float objY, int photonTargetID)
        {
            view.RPC("DimensionOutUpdateLocation", RpcTarget.All, tempTargetPositionX, tempTargetPositionY, objX, objY, photonTargetID);
        }

        [PunRPC]
        private void DimensionOutUpdateLocation(float tempTargetPositionX, float tempTargetPositionY, float objX, float objY, int photonTargetID)
        {
            Player targetP = GetPlayerByPhotonID(photonTargetID);
            int currentMap = (int)targetP.CurrentPosition.x / 100;
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
                Player playerScript = GetPlayerByPhotonID(photonViewID);
                playerScript.CurrentPosition = playerScript.transform.position;
                playerScript.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                playerScript.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                playerScript.transform.position = new Vector3(objX, objY, playerScript.DefaultZAxis);

                GameObject mainCamera = GameObject.Find("Main Camera");
                mainCamera.transform.position = playerScript.TargetPosition;

                int xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                int yCurrent = (int)(playerScript.CurrentPosition.y);
                int xTarget = (int)(playerScript.TargetPosition.x % 100);
                int yTarget = (int)(playerScript.TargetPosition.y);

                Debug.Log(currentMap + " - " + xCurrent + " - " + yCurrent);

                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    //RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonTargetID);
                }
                else
                {
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    //RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonTargetID);
                }
                //currentMap = tempTargetMap;
            }
        }

        public void CallDimensionInUpdateLocation(float tempTargetPositionX, float tempTargetPositionY, float objX, float objY, int photonTargetID)
        {
            view.RPC("DimensionInUpdateLocation", RpcTarget.All, tempTargetPositionX, tempTargetPositionY, objX, objY, photonTargetID);
        }

        [PunRPC]
        private void DimensionInUpdateLocation(float tempTargetPositionX, float tempTargetPositionY, float objX, float objY, int photonTargetID)
        {
            *//* bug F vao ma M bi di theo *//*
            Player targetP = GetPlayerByPhotonID(photonTargetID);
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
                Player playerScript = GetPlayerByPhotonID(photonViewID);
                playerScript.CurrentPosition = playerScript.transform.position;
                playerScript.TempTargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                playerScript.TargetPosition = new Vector2(tempTargetPositionX, tempTargetPositionY);
                playerScript.transform.position = new Vector3(objX, objY, playerScript.DefaultZAxis);

                GameObject mainCamera = GameObject.Find("Main Camera");
                mainCamera.transform.position = playerScript.TargetPosition;

                int tempTargetMap = (int)playerScript.TargetPosition.x / 100;
                int currentMap = (int)playerScript.CurrentPosition.x / 100;
                int xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                int yCurrent = (int)(playerScript.CurrentPosition.y);
                int xTarget = (int)(playerScript.TargetPosition.x % 100);
                int yTarget = (int)(playerScript.TargetPosition.y);


                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    //RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                }
                else
                {
                    //view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    //RPC.CallGenerateWire(currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                }
                //currentMap = tempTargetMap;
            }
        }*/

}
