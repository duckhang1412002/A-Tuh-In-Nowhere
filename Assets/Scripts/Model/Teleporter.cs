using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public int ID { get; set; }
    public bool HasLeftEntrance{get; set;}
    public bool HasTopEntrance{get; set;}
    public bool HasRightEntrance{get; set;}
    public bool HasBottomEntrance{get; set;}
    public Teleporter TargetTeleporter { get; set; }

    void Start()
    {
        HasLeftEntrance = true;
        HasTopEntrance = false;
        HasRightEntrance = false;
        HasBottomEntrance = false;
    }

    public Vector3 GetNextPositionOutside(Player player)
    {
        Vector3 entrancePosition = new Vector3();
        if (player.TempNextKey == "Right" && HasLeftEntrance
        || player.TempNextKey == "Down" && HasTopEntrance
        || player.TempNextKey == "Left" && HasRightEntrance
        || player.TempNextKey == "Up" && HasBottomEntrance)
        {          
            entrancePosition = new Vector3(TargetTeleporter.transform.position.x, TargetTeleporter.transform.position.y, player.transform.position.z);
        }
        else{
            entrancePosition = new Vector3(this.transform.position.x, this.transform.position.y, player.transform.position.z);
        }

        return entrancePosition;
    }
    public Vector3 GetNextPositionInside(Player player)
    {
        Vector3 entrancePosition = new Vector3();
        if (player.TempNextKey == "Right" && HasRightEntrance){          
            entrancePosition = new Vector3(TargetTeleporter.transform.position.x+1, TargetTeleporter.transform.position.y, player.transform.position.z);
        } 
        else if(player.TempNextKey == "Down" && HasBottomEntrance){
            entrancePosition = new Vector3(TargetTeleporter.transform.position.x, TargetTeleporter.transform.position.y-1, player.transform.position.z);
        } 
        else if(player.TempNextKey == "Left" && HasLeftEntrance){
            entrancePosition = new Vector3(TargetTeleporter.transform.position.x-1, TargetTeleporter.transform.position.y, player.transform.position.z);
        } 
        else if(player.TempNextKey == "Up" && HasTopEntrance){
            entrancePosition = new Vector3(TargetTeleporter.transform.position.x, TargetTeleporter.transform.position.y+1, player.transform.position.z);
        }
        else{
            entrancePosition = new Vector3(this.transform.position.x, this.transform.position.y, player.transform.position.z);
        }

        return entrancePosition;
    }

    public bool CheckNextStepOutside(Player player, Dictionary<Vector2,bool> wireMap){
        bool totalCheck = true;
        if(wireMap.ContainsKey(this.GetNextPositionOutside(player)) && !player.IsNotPickWire){
            totalCheck = false;
        }
        Debug.Log(totalCheck);

        return totalCheck;
    }
    public bool CheckNextStepInside(Player player, GameObject nextStepObject, Dictionary<Vector2,bool> wireMap){
        bool totalCheck = true;
        if(wireMap.ContainsKey(nextStepObject.transform.position) && !player.IsNotPickWire){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Socket" && !player.IsNotPickWire 
        && nextStepObject.GetComponent<Socket>().Color != player.HandleWireColor
        && nextStepObject.GetComponent<Socket>().IsConnect == false){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Wall"){
            totalCheck = false;
        }

        return totalCheck;
    }
    public bool IsTeleport(Vector3 teleporterPosition){
        Vector2 currentTeleporter = this.transform.position;
        Vector2 targetTeleporter = new Vector2(teleporterPosition.x, teleporterPosition.y);
        if(currentTeleporter != targetTeleporter) 
            return true;
        else
            return false;
    }
}
