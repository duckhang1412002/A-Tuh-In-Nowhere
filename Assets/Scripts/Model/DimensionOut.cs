using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WebSocketSharp;

public class DimensionOut : MonoBehaviour
{
    public DimensionIn BaseDimension { get; set; }
    public string OutDirection { get; set; }
    public string WireOnColor { get; set; }

    private static float[] dimensionOutRotation = { 0f, 90.0f, 180.0f, 270.0f };

    private void Start()
    {
        WireOnColor = null;
    }

    public Vector2 GetExitPosition(Vector2 moveDirection)
    {
        Vector2 exitPosition = Vector2.zero;
        if (this.BaseDimension != null)
        {
            if (moveDirection == Vector2.left)
            {
                exitPosition = new Vector3(BaseDimension.transform.position.x - 1, BaseDimension.transform.position.y);
            }
            else if (moveDirection == Vector2.down)
            {
                exitPosition = new Vector3(BaseDimension.transform.position.x, BaseDimension.transform.position.y - 1);
            }
            else if (moveDirection == Vector2.right)
            {
                exitPosition = new Vector3(BaseDimension.transform.position.x + 1, BaseDimension.transform.position.y);
            }
            else if (moveDirection == Vector2.up)
            {
                exitPosition = new Vector3(BaseDimension.transform.position.x, BaseDimension.transform.position.y + 1);
            }
        }
        return exitPosition;
    }
    public Vector3 GetNextPosition(Player player)
    {
        Vector3 exitPosition = new Vector3();
        if (player.TempNextKey == "Left" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x-1, BaseDimension.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Down" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x, BaseDimension.transform.position.y-1, player.transform.position.z);
        }
        else if (player.TempNextKey == "Right" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x+1, BaseDimension.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Up" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x, BaseDimension.transform.position.y+1, player.transform.position.z);
        }

        return exitPosition;
    }

    public bool CheckNextStep(Player player, GameObject nextStepObject, Dictionary<Vector2,bool> wireMap){
        bool totalCheck = true;
        if(wireMap.ContainsKey(GetNextPosition(player)) && !player.IsNotPickWire){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Socket" && !player.IsNotPickWire 
        && nextStepObject.GetComponent<Socket>().Color != player.HandleWireColor
        && nextStepObject.GetComponent<Socket>().IsConnect == false){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Bridge" && !nextStepObject.GetComponent<Bridge>().CheckNextStep(player)){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Wall"){
            totalCheck = false;
        }

        return totalCheck;
    }

    public void RenderSprite(string direction){
        if(direction == "Left"){
            transform.Rotate(0f, 0f, dimensionOutRotation[0]);
        } else if(direction == "Bottom"){
            transform.Rotate(0f, 0f, dimensionOutRotation[1]);
        } else if(direction == "Right"){
            transform.Rotate(0f, 0f, dimensionOutRotation[2]);
        } else if(direction == "Top"){
            transform.Rotate(0f, 0f, dimensionOutRotation[3]);
        }
    }
}
