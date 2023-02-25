using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePoint : MonoBehaviour
{
    [SerializeField] private string color;   

    public string GetColorType(){
        return color.Trim();
    }
}
