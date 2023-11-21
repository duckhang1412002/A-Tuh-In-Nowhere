using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public class DoorButton : MonoBehaviour
{
    public int ID { get; set; }
    public bool IsActive{get; set;}
    public bool HasPipeOn{get; set;}
    private GameManager gameManager;
    // Start is called before the first frame update
    public void Start()
    {
        IsActive = false;
        HasPipeOn = false;
        this.gameObject.transform.Find("Inner").gameObject.GetComponent<SpriteGlowEffect>().GlowColor = this.gameObject.GetComponent<ChangeColor>().GetColor("Red");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(HasPipeOn){
            IsActive = true;          
        }
        GameObject playerM = gameManager.PlayerM;
        GameObject playerF = gameManager.PlayerF;
        if (playerM != null && (Vector2)playerM.transform.position == (Vector2)this.transform.position)
        {
            IsActive = true;
            if (playerM.GetComponent<Player>().IsHandleWire) gameManager.WireMap[(Vector2)this.transform.position] = true;
        }
        else if (playerF != null && (Vector2)playerF.transform.position == (Vector2)this.transform.position)
        {
            IsActive = true;
            if (playerF.GetComponent<Player>().IsHandleWire) gameManager.WireMap[(Vector2)this.transform.position] = true;
        }
        else IsActive = gameManager.WireMap.ContainsKey(this.transform.position);

        if (IsActive)
        {
            this.gameObject.transform.Find("Inner").gameObject.GetComponent<SpriteGlowEffect>().GlowColor = this.gameObject.GetComponent<ChangeColor>().GetColor("Green");
        }
        else
        {
            this.gameObject.transform.Find("Inner").gameObject.GetComponent<SpriteGlowEffect>().GlowColor = this.gameObject.GetComponent<ChangeColor>().GetColor("Red");
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
            if(!player.IsNotPickWire || player.IsHandleWire) this.HasPipeOn = true;
            return true;
        }          
    }
}
