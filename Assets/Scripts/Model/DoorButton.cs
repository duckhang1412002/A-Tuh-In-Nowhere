using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public int ID { get; set; }

    [SerializeField]
    private Sprite unableButton;

    [SerializeField]
    private Sprite enableButton;
    public bool IsActive{get; set;}
    public bool HasPipeOn{get; set;}
    // Start is called before the first frame update
    public void Start()
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

    public void CheckCurrentStep(Player player, GameObject nextStepObject, Dictionary<Vector2, bool> wireMap){
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
        else if(nextStepObject.tag == "Door" && !nextStepObject.GetComponent<Door>().IsActive){
            totalCheck = false;
        }

        if(!totalCheck) 
            this.IsActive = true;
        else 
            this.IsActive = false;
    }

    public bool CheckNextStep(Player player){
        if(this.HasPipeOn && !player.IsNotPickWire) 
            return false;
        else{
            if(!player.IsNotPickWire) this.HasPipeOn = true;
            return true;
        }          
    }
}
