using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int ID { get; set; }
    private GameManager gameManager;

    public DoorButton Button { get; set; }

    public string DoorOpenDirection { get; set; }

    public bool isReverseDoor {get; set;}

    public bool IsActive{get; set;}  

    public bool HasPipeAtDoorPosition{get; set;}
    public bool HasPlayerAtDoorPosition{get; set;}

    private float moveSpeed = 5f;
    private Vector2 previousPosition; 
    private Vector2 targetPosition; 
    private Vector2 openAxis;
    private Vector2 doorPosition;

    // Start is called before the first frame update
    public void Start()
    {
        HasPlayerAtDoorPosition = false;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public void Init()
    {
        previousPosition = doorPosition = this.transform.position;

        if (DoorOpenDirection == "Up") targetPosition = new Vector2(previousPosition.x, previousPosition.y + 1);
        else if (DoorOpenDirection == "Down") targetPosition = new Vector2(previousPosition.x, previousPosition.y - 1);
        else if (DoorOpenDirection == "Left") targetPosition = new Vector2(previousPosition.x - 1, previousPosition.y);
        else if (DoorOpenDirection == "Right") targetPosition = new Vector2(previousPosition.x + 1, previousPosition.y);

        if (isReverseDoor)
        {
            IsActive = true;
            this.transform.position = new Vector3(targetPosition.x, targetPosition.y, 9);

            //swap
            Vector2 tmp = previousPosition;
            previousPosition = targetPosition;
            targetPosition = tmp;
        }
        else
        {
            IsActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HasPlayerAtDoorPosition = false;
        GameObject playerM = gameManager.PlayerM;
        GameObject playerF = gameManager.PlayerF;
        if (playerM != null && (Vector2)playerM.transform.position == (Vector2)doorPosition)
        {
            HasPlayerAtDoorPosition = true;
            if (playerM.GetComponent<Player>().IsHandleWire) gameManager.WireMap[(Vector2)doorPosition] = true;
        }
        if (playerF != null && (Vector2)playerF.transform.position == (Vector2)doorPosition)
        {
            HasPlayerAtDoorPosition = true;
            if (playerF.GetComponent<Player>().IsHandleWire) gameManager.WireMap[(Vector2)doorPosition] = true;
        }
        HasPipeAtDoorPosition = gameManager.WireMap.ContainsKey(doorPosition);
        if (Button.IsActive)
        {
            if (!HasPlayerAtDoorPosition && !HasPipeAtDoorPosition)
            {
                openAxis = targetPosition;
                IsActive = !isReverseDoor;
            }
        }
        else
        {
            if (!HasPlayerAtDoorPosition && !HasPipeAtDoorPosition)
            {
                openAxis = previousPosition;
                IsActive = isReverseDoor;
            }
        }
        DoorTransition();

        
    }

    public void DebugPosition()
    {
        Debug.Log(this.transform.position + " : " + previousPosition + " - " + targetPosition);
    }

    public void DoorTransition(){
        if (!HasPipeAtDoorPosition && !HasPlayerAtDoorPosition){
            this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(openAxis.x, openAxis.y, 9), moveSpeed * Time.deltaTime);            
        }       
    }

    public Vector2 GetReverseDoorLocation(){
        return new Vector2(targetPosition.x, targetPosition.y);
    }

    public bool CheckReverseDoor(){
        return isReverseDoor;
    }

    public bool CheckNextStep(Player player){
        bool totalCheck = false;

        if(this.IsActive){
            totalCheck = true;
            if (!player.IsNotPickWire) this.HasPipeAtDoorPosition = true;           

        } else{
            totalCheck = false;
        }

        if(totalCheck){
            HasPlayerAtDoorPosition = true;
        }

        return totalCheck;
    }

    public bool IsValidPosition()
    {
        return this.IsActive;
    }
}
