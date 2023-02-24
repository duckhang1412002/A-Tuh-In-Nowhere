using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private string bridgeType;

    public bool HasPipeOnBridge{get;set;}
    public bool HasPipeUnderBridge{get;set;}

    void Start(){
        HasPipeOnBridge = false;
        HasPipeUnderBridge = false;
    }

    public string GetBridgeType(){
        return bridgeType.Trim();
    }
}
