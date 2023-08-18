using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using static UnityEngine.GraphicsBuffer;
using System;

public class RPCManager : MonoBehaviourPunCallbacks
{
    private PhotonView view;
    private GameManager gameManager;

    public void Start()
    {
        view = this.gameObject.GetComponent<PhotonView>();
        gameManager= this.gameObject.GetComponent<GameManager>();
        
    }
    private Player GetPlayerByPhotonID(int photonViewID)
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        GameObject player = (photonViewID == 1) ? gameManager.PlayerM : gameManager.PlayerF;
        return player.GetComponent<Player>();
    }

    public void CallGenerateWire(int mapIndex, int xAxis, int yAxis, string type, int photonTargetID)
    {
        //GenerateWire(mapIndex, xAxis, yAxis, type, photonTargetID);
        //Debug.Log("RPC View: " + view);
        view.RPC("GenerateWire", RpcTarget.All, mapIndex, xAxis, yAxis, type, photonTargetID);
    }

    [PunRPC]
    private void GenerateWire(int mapIndex, int xAxis, int yAxis, string type, int photonTargetID)
    {
        Player targetP = GetPlayerByPhotonID(photonTargetID);

        Debug.Log("Target P: " + targetP.ToString());

        Wire wireSpawner = GameObject.Find("WireSpawner").GetComponent<Wire>();
        //GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

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
/*        if (photonTargetID != photonViewID)
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
        }*/
    }
}
