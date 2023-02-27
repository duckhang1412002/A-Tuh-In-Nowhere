using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField]
    private Door door;
    public bool IsActive{get; set;}
    public bool HasPipeOn{get; set;}
    // Start is called before the first frame update
    void Start()
    {
        IsActive = false;
        HasPipeOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(HasPipeOn){
            IsActive = true;
        }
    }

    public Door GetDoor(){
        return door;
    }
}
