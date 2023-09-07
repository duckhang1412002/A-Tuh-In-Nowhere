using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Socket : MonoBehaviour
{
    public string Color { get; set; }

    public bool IsConnect { get; set; }
    public bool IsEndPoint { get; set; }

    void Start()
    {
        IsConnect = false;
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

    public void ChangePlayerColor(Player player)
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
        } else
        {
            player.IsHandleWire = true;
            player.HandleWireSteps = 0;
            player.HandleWireColor = this.Color;
        }

        player.IsAtSocket = true;
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
