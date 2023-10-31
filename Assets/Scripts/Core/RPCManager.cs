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
        int x = (int)pos.x % 100;
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
        Debug.Log("Score: " + gameManager.Score);
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


    public void CallUpdateSocket(int photonViewID, Vector2 socketPos)
    {
        view.RPC("UpdateSocket", RpcTarget.All, photonViewID, socketPos.x, socketPos.y);
    }

    [PunRPC]
    private void UpdateSocket(int photonViewId, float x, float y)
    {
        Player player = GetPlayerByPhotonID(photonViewId);
        Socket socket = GetItemAtPosition(new Vector2(x, y)).GetComponent<Socket>();
        socket.UpdateSocket(player);
    }

    public void CallChangePlayerColor(int photonViewID, Vector2 socketPos)
    {
        //Debug.Log("PhotonViewID " + photonViewID);
        view.RPC("ChangePlayerColor", RpcTarget.All, photonViewID, socketPos.x, socketPos.y);
    }

    [PunRPC]
    private void ChangePlayerColor(int photonViewId, float x, float y) {
        Player player = GetPlayerByPhotonID(photonViewId);
        //Debug.Log("Change cl of " + player);
        Socket socket = GetItemAtPosition(new Vector2(x, y)).GetComponent<Socket>();
        //Debug.Log("Get the socket at " + socket);
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
}
