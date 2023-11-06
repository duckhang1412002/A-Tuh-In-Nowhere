using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourPun
{
    public int ID;
    public string[] CompletedMapID {get; set;}


    public string HandleWireColor{get; set;}
    public bool IsNotPickWire{get; set;}
    public bool IsHandleWire { get; set; }
    public int HandleWireSteps { get; set; }
    public string TempNextKey { get; set; }
    public string PreviousMove { get; set; }
    public Vector2 PreviousDirection { get; set; }
    public float DefaultZAxis { get; set; }
    public bool IsAtSocket { get; set; }

    [SerializeField]
    private Vector2 currentPos;
    [SerializeField]
    private Vector2 targetPos;
    [SerializeField]
    private Vector2 previousPos;
    private Vector2 tempCurrentPos;
    private Vector2 tempTargetPos;
    public Vector2 CurrentPosition
    {
        get => currentPos;
        set
        {
            currentPos = value;
            if(SceneManager.GetActiveScene().name != "SingleLobby"
            && SceneManager.GetActiveScene().name != "GameMode") 
                photonView.RPC("SetCurrentPosition", RpcTarget.Others, currentPos.x, currentPos.y);
        }
    }

    public Vector2 PreviousPosition
    {
        get => previousPos;
        set
        {
            previousPos = value;
            if(SceneManager.GetActiveScene().name != "SingleLobby"
            && SceneManager.GetActiveScene().name != "GameMode")
                photonView.RPC("SetPreviousPosition", RpcTarget.Others, previousPos.x, previousPos.y);
        }
    }

    public Vector2 TargetPosition
    {
        get => targetPos;
        set
        {
            targetPos = value;
            if(SceneManager.GetActiveScene().name != "SingleLobby"
            && SceneManager.GetActiveScene().name != "GameMode") 
                photonView.RPC("SetTargetPosition", RpcTarget.Others, targetPos.x, targetPos.y);
        }
    }

    public Vector2 TempCurrentPosition
    {
        get; set;
    }

    public Vector2 TempTargetPosition
    {
        get; set;
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

    private void Awake()
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

/*    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Serialize data to send over the network
            stream.SendNext(CurrentPosition);
            stream.SendNext(PreviousPosition);
            stream.SendNext(TargetPosition);
            // Serialize other properties if needed...
        }
        else
        {
            // Deserialize data received from the network
            CurrentPosition = (Vector2)stream.ReceiveNext();
            PreviousPosition = (Vector2)stream.ReceiveNext();
            TargetPosition = (Vector2)stream.ReceiveNext();
            // Deserialize other properties if needed...
        }
    }*/
}
