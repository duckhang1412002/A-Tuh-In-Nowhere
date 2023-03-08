using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePoint : MonoBehaviour
{
    [SerializeField] private string color;   

    public bool IsConnect{get; set;}

    void Start(){
        IsConnect = false;
    }

    public string GetColorType(){
        return color;
    }
}
