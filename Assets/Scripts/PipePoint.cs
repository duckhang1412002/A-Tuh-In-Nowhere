using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePoint : MonoBehaviour
{
    [SerializeField] private string color;   
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetColorType(){
        return color.Trim();
    }
}
