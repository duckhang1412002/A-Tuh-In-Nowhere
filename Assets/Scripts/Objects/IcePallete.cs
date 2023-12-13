using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePallete : MonoBehaviour
{
    void Start(){

    }

    public bool CheckNextStep(Player player, Dictionary<Vector2,bool> wireMap){
        if(wireMap.ContainsKey(player.TempTargetPosition) && !player.IsNotPickWire){
            return false;
        }
        else{
            return true;
        }
    }
}
