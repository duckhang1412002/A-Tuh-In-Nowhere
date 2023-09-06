using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPun
{
    public int ID { get; set; }
    public string HandleWireColor{get; set;}
    public bool IsNotPickWire{get; set;}
    public bool IsHandleWire { get; set; }
    public int HandleWireSteps { get; set; }
    public string TempNextKey { get; set; }
    public string PreviousMove { get; set; }
    public Vector2 PreviousDirection { get; set; }
    public float DefaultZAxis { get; set; }
    public bool IsAtSocket { get; set; }

    private Vector2 currentPos;
    private Vector2 targetPos;
    private Vector2 previousPos;
    private Vector2 tempCurrentPos;
    private Vector2 tempTargetPos;
    public Vector2 CurrentPosition
    {
        get => currentPos;
        set
        {
            currentPos = value;
            photonView.RPC("SetCurrentPosition", RpcTarget.Others, currentPos.x, currentPos.y);
        }
    }

    public Vector2 PreviousPosition
    {
        get => previousPos;
        set
        {
            previousPos = value;
            photonView.RPC("SetPreviousPosition", RpcTarget.Others, previousPos.x, previousPos.y);
        }
    }

    public Vector2 TargetPosition
    {
        get => targetPos;
        set
        {
            targetPos = value;
            photonView.RPC("SetTargetPosition", RpcTarget.Others, targetPos.x, targetPos.y);
        }
    }

    public Vector2 TempCurrentPosition
    {
        get => tempCurrentPos;
        set
        {
            tempCurrentPos = value;
            photonView.RPC("SetTempCurrentPosition", RpcTarget.Others, tempCurrentPos.x, tempCurrentPos.y);
        }
    }

    public Vector2 TempTargetPosition
    {
        get => tempTargetPos;
        set
        {
            tempTargetPos = value;
            photonView.RPC("SetTempTargetPosition", RpcTarget.Others, tempTargetPos.x, tempTargetPos.y);
        }
    }

    [PunRPC]
    private void SetCurrentPosition(float x, float y)
    {
        currentPos = new Vector2(x, y);
    }

    [PunRPC]
    private void SetPreviousPosition(float x, float y)
    {
        currentPos = new Vector2(x, y);
    }

    [PunRPC]
    private void SetTargetPosition(float x, float y)
    {
        targetPos = new Vector2(x, y);
    }

    [PunRPC]
    private void SetTempCurrentPosition(float x, float y)
    {
        tempCurrentPos = new Vector2(x, y);
    }

    [PunRPC]
    private void SetTempTargetPosition(float x, float y)
    {
        tempTargetPos = new Vector2(x, y);
    }

    // Start is called before the first frame update
    void Start()
    {
        IsNotPickWire = true;
        IsAtSocket = false;
        IsHandleWire = false;
        CurrentPosition = this.transform.position;
        TargetPosition = new Vector2((float)Math.Ceiling(this.transform.position.x), (float)Math.Ceiling(this.transform.position.y));
        TempCurrentPosition = this.transform.position;
        TempTargetPosition = this.transform.position;
        DefaultZAxis = 6f;
        TempNextKey = "";
        PreviousMove = "";
        HandleWireColor = "Default";
        PreviousDirection = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Serialize data to send over the network
            stream.SendNext(CurrentPosition);
            stream.SendNext(PreviousPosition);
            stream.SendNext(TargetPosition);
            stream.SendNext(TempCurrentPosition);
            stream.SendNext(TempTargetPosition);
            // Serialize other properties if needed...
        }
        else
        {
            // Deserialize data received from the network
            CurrentPosition = (Vector2)stream.ReceiveNext();
            PreviousPosition = (Vector2)stream.ReceiveNext();
            TargetPosition = (Vector2)stream.ReceiveNext();
            TempCurrentPosition = (Vector2)stream.ReceiveNext();
            TempTargetPosition = (Vector2)stream.ReceiveNext();
            // Deserialize other properties if needed...
        }
    }
}
