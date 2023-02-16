using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePoint : MonoBehaviour
{
    [SerializeField] private string color;   
    // Start is called before the first frame update
    public string GetColorType(){
        return color.Trim();
    }
}
