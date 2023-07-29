using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escalator : MonoBehaviour
{
    public string Direction{get; set;}
    private static string[] directionArr = {"Left", "Down", "Right", "Up"};
    public bool IsRotateByClock{get; set;}
    private static float[] escalatorRotation = { 0f, 90.0f, 180.0f, 270.0f };
    int rotationIndex;
    private bool blockRotate;

    public EscButton button;

    public void Start(){
        IsRotateByClock = true;
        blockRotate = false;

        if(Direction == "Left"){
            rotationIndex = 0;
        } else if (Direction == "Up"){
           rotationIndex = 3;
        } else if (Direction == "Right"){
            rotationIndex = 2;
        } else if (Direction == "Down"){
            rotationIndex = 1;
        }
    }

    void Update(){
        if(button != null){
            if(button.IsActive && !blockRotate){
                button.IsActive = false;
                if(button.HasPipeOn) blockRotate = true;
                RotateObject();          
            }
        }
    }

    private void RotateObject(){
        if(IsRotateByClock){
            this.transform.Rotate(0f, 0f, -90f);
            --rotationIndex;
        } else {
            this.transform.Rotate(0f, 0f, 90f);
            ++rotationIndex;
        }

        rotationIndex = rotationIndex % 4;
        if(rotationIndex < 0) rotationIndex = 3; 
        Debug.Log("Check " + rotationIndex + directionArr[rotationIndex]);

        Direction = directionArr[rotationIndex];
    }

    public void RenderSprite(){
        if(Direction == "Left"){
            this.transform.Rotate(0f, 0f, escalatorRotation[0]);
        } else if (Direction == "Up"){
           this.transform.Rotate(0f, 0f, escalatorRotation[3]);;
        } else if (Direction == "Right"){
            this.transform.Rotate(0f, 0f, escalatorRotation[2]);
        } else if (Direction == "Down"){
            this.transform.Rotate(0f, 0f, escalatorRotation[1]);
        }
    }

    public Vector3 GetNextPosition(Player player)
    {
        Vector3 targetPosition = new Vector3();
        if (Direction == "Up")
        {          
            targetPosition = new Vector3(this.transform.position.x, this.transform.position.y+1, player.transform.position.z);
        } else if(Direction == "Right"){
            targetPosition = new Vector3(this.transform.position.x+1, this.transform.position.y, player.transform.position.z);
        } else if(Direction == "Down"){
            targetPosition = new Vector3(this.transform.position.x, this.transform.position.y-1, player.transform.position.z);
        } else if(Direction == "Left"){
            targetPosition = new Vector3(this.transform.position.x-1, this.transform.position.y, player.transform.position.z);
        }
       
        return targetPosition;
    }


    public bool CheckNextStep(Player player, Dictionary<Vector2,Wire> wireMap){
        if(wireMap.ContainsKey(player.TempTargetPosition) && !player.IsNotPickWire){
            return false;
        }
        else{
            return true;
        }
    }
}
