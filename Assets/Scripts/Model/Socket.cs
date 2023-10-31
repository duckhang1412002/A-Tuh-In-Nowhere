using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class Socket : MonoBehaviour
{
    [SerializeField]
    public string Color;

    public bool IsConnect { get; set; }
    public bool IsEndPoint { get; set; }
    private GameManager gameManager;
    void Start()
    {
        IsConnect = false;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public bool CheckSocketStartPoint(Player player)
    {
        if (this.IsConnect == false && !player.IsHandleWire)
            return true;
        else
            return false;
    }

    public bool CheckSocketEndPoint(Player player)
    {
        if (player.HandleWireColor == this.Color && !this.IsConnect)
            return true;
        else
            return false;
    }

    private void Update()
    {
        GameObject playerM = gameManager.PlayerM;
        GameObject playerF = gameManager.PlayerF;
        if (playerM != null && (Vector2)playerM.transform.position == (Vector2)this.transform.position)
        {
            ChangePlayerColor(playerM.GetComponent<Player>());
        }
        if (playerF != null && (Vector2)playerF.transform.position == (Vector2)this.transform.position)
        {
            ChangePlayerColor(playerF.GetComponent<Player>());
        }
    }
    public void UpdateSocket(Player player)
    {
        player.IsNotPickWire = player.HandleWireColor == this.Color ? true : false;
        this.IsConnect = true;
        player.IsHandleWire = player.HandleWireColor == this.Color ? false : true;
        if (player.HandleWireColor == this.Color) //endpoint
        {
            player.IsHandleWire = false;
            //player.HandleWireSteps = -1;
            //player.HandleWireColor = "Default";
            this.IsEndPoint = true;
        }
        else
        {
            player.IsHandleWire = true;
            player.HandleWireSteps = 0;
            player.HandleWireColor = this.Color;
        }

        player.IsAtSocket = true;
    }

    public void ChangePlayerColor(Player player)
    {
        //Default
        // Accessing the child object by name
        Transform childTransform = player.transform.Find("WholePlayerObject").transform.Find("Body");
        if (childTransform != null)
        {
            GameObject body = childTransform.gameObject;
            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, (this.IsEndPoint) ? "Default" : player.HandleWireColor);
        }
    }

    public void ChangePlayerAttrStartPoint(Player player)
    {
        player.IsNotPickWire = false;
        player.IsAtSocket = true;
        this.IsConnect = true;
        player.HandleWireColor = this.Color;
        Debug.Log("Is start point --- " + player.HandleWireColor);

        // Accessing the child object by name
        Transform childTransform = player.transform.Find("WholePlayerObject").transform.Find("Body");
        if (childTransform != null)
        {
            GameObject body = childTransform.gameObject;
            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, player.HandleWireColor);
        }
    }


    public void ChangePlayerAttrEndPoint(Player player)
    {
        player.IsNotPickWire = true;
        player.IsAtSocket = true;
        this.IsConnect = true;
        Debug.Log("Is end point --- " + player.HandleWireColor);

        // Accessing the child object by name
        Transform childTransform = player.transform.Find("WholePlayerObject").transform.Find("Body");
        if (childTransform != null)
        {
            GameObject body = childTransform.gameObject;
            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, "Default");
        }
        //activatePipeEffect = true;
        //gameManager.GetComponent<GameManager>().Score++;
    }
}
