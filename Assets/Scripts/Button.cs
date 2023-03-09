using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField]
    private Door door;

    [SerializeField]
    private Sprite unableButton;

    [SerializeField]
    private Sprite enableButton;
    public bool IsActive{get; set;}
    public bool HasPipeOn{get; set;}
    // Start is called before the first frame update
    void Start()
    {
        IsActive = false;
        HasPipeOn = false;
        this.GetComponent<SpriteRenderer>().sprite = unableButton;
    }

    // Update is called once per frame
    void Update()
    {
        if(HasPipeOn){
            IsActive = true;          
        }

        if(IsActive){
            this.GetComponent<SpriteRenderer>().sprite = enableButton;
        } else{
            this.GetComponent<SpriteRenderer>().sprite = unableButton;
        }
    }

    public Door GetDoor(){
        return door;
    }
}
