using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public string Direction { get; set; }
    public bool HasWireOnBridge { get; set; }
    public bool HasWireUnderBridge { get; set; }
    public bool HasPlayerOnBridge {get; set; }
    public bool HasPlayerUnderBridge {get; set; }
    ChangeColor color;
    private GameManager gameManager;

    void Start()
    {
        HasWireOnBridge = false;
        HasWireUnderBridge = false;
        HasPlayerOnBridge = false;
        HasPlayerUnderBridge = false;
        color = new ChangeColor();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Update(){
        if (color != null)
        {
            if (HasPlayerUnderBridge)
            {
                color.ChangeSpriteColor(this.gameObject, "Opacity");
            }
            else
            {
                color.ChangeSpriteColor(this.gameObject, "Default");
            }
        }
        this.HasPlayerUnderBridge = false;
        this.HasPlayerOnBridge = false;
        GameObject playerM = gameManager.PlayerM;
        GameObject playerF = gameManager.PlayerF;
        if (playerM != null && (Vector2)playerM.transform.position == (Vector2)this.transform.position)
        {
            Debug.Log("Found playerM at Bridge!");
            if (playerM.transform.position.z == 5f)
            {
                this.HasPlayerUnderBridge = true;
                if (playerM.GetComponent<Player>().IsHandleWire)
                {
                    Debug.Log("there is a wire underneath");
                    this.HasWireUnderBridge = true;
                }
            } else if (playerM.transform.position.z == 2f)
            {
                this.HasPlayerOnBridge = true;
                if (playerM.GetComponent<Player>().IsHandleWire) this.HasWireOnBridge = true;
            }
/*            this.HasPlayerUnderBridge = playerM.transform.position.z == 5f;
            this.HasPlayerOnBridge = playerM.transform.position.z == 2f;*/
        } 
        if (playerF != null && (Vector2)playerF.transform.position == (Vector2)this.transform.position)
        {
            if (playerF.transform.position.z == 5f)
            {
                this.HasPlayerUnderBridge = true;
                if (playerF.GetComponent<Player>().IsHandleWire)
                {
                    Debug.Log("there is a wire underneath");
                    this.HasWireUnderBridge = true;
                }
            }
            else if (playerF.transform.position.z == 2f)
            {
                this.HasPlayerOnBridge = true;
                if (playerF.GetComponent<Player>().IsHandleWire) this.HasWireOnBridge = true;
            }
            /*this.HasPlayerUnderBridge = playerF.transform.position.z == 5f;
            this.HasPlayerOnBridge = playerF.transform.position.z == 2f;*/
        }

    }

    public bool IsVertical()
    {
        return Direction == "Vertical";
    }
    public bool IsHorizontal()
    {
        return Direction == "Horizontal";
    }
    public void RenderSprite(){
        if(IsVertical()){
            this.transform.Rotate(0f,0f,90f);
        }
    }

    public bool CheckNextStep(Player player)
    {
        bool isOnBridge = false;

        if ((this.IsHorizontal() && (player.TempNextKey == "Left" || player.TempNextKey == "Right"))
        || (this.IsVertical() && (player.TempNextKey == "Up" || player.TempNextKey == "Down")))
            isOnBridge = true;

        if (isOnBridge)
        {
            if((HasWireOnBridge && !player.IsNotPickWire) || HasPlayerOnBridge) return false;
            player.DefaultZAxis = 2f;  
            this.HasPlayerOnBridge = true;        
        }
        else
        {    
            if(HasWireUnderBridge && !player.IsNotPickWire || HasPlayerUnderBridge) return false;   
            player.DefaultZAxis = 5f;
            this.HasPlayerUnderBridge = true;  
        }

        return true;
    }

    public bool CheckCurrentStep(Player player, string previousMove)
    {
        bool isOnBridge = false;

        if ((this.IsHorizontal() && (previousMove == "Left" || previousMove == "Right"))
        || (this.IsVertical() && (previousMove == "Up" || previousMove == "Down")))
            isOnBridge = true;

        if (isOnBridge)
        {
            if ((this.IsHorizontal() && (player.TempNextKey == "Up" || player.TempNextKey == "Down"))
            || (this.IsVertical() && (player.TempNextKey == "Left" || player.TempNextKey == "Right")))
                return false;

            if (!player.IsNotPickWire) this.HasWireOnBridge = true;
            this.HasPlayerOnBridge = false;
        }
        else
        {
            if ((this.IsHorizontal() && (player.TempNextKey == "Left" || player.TempNextKey == "Right"))
            || (this.IsVertical() && (player.TempNextKey == "Up" || player.TempNextKey == "Down")))
                return false;

            if (!player.IsNotPickWire) this.HasWireUnderBridge = true;
            this.HasPlayerUnderBridge = false;
        }
        return true;
    }

    public void CheckOpacity(Player player, string previousMove)
    {
        bool isOnBridge = false;

        if ((this.IsHorizontal() && (previousMove == "Left" || previousMove == "Right"))
        || (this.IsVertical() && (previousMove == "Up" || previousMove == "Down")))
            isOnBridge = true;

        if (!isOnBridge)
        {
            this.HasPlayerUnderBridge = true;
        }
    }

    public float GetZAxisWire(string previousMove)
    {
        float wireZAxis = 0f;

        if (this.IsVertical()
        && (previousMove == "Left" || previousMove == "Right")
        && this.HasWireUnderBridge)
        {
            wireZAxis = 6f;
        }
        else if (this.IsHorizontal()
        && (previousMove == "Up" || previousMove == "Down"
        && this.HasWireUnderBridge))
        {
            wireZAxis = 6f;
        }
        else if (this.IsVertical()
        && (previousMove == "Up" || previousMove == "Down"
        && this.HasWireOnBridge))
        {
            wireZAxis = 3f;
        }
        else if (this.IsHorizontal()
        && (previousMove == "Left" || previousMove == "Right"
        && this.HasWireOnBridge))
        {
            wireZAxis = 3f;
        }

        return wireZAxis;
    }

    public float GetZAxisToMove(Player player, Vector2 moveDirection)
    {
        if (this.IsVertical())
        {
            if (moveDirection == Vector2.up || moveDirection == Vector2.down && !HasPlayerOnBridge)
            {
                if (!player.IsHandleWire || (player.IsHandleWire && !HasWireOnBridge)) 
                    return 2f;
            }
            if (moveDirection == Vector2.left || moveDirection == Vector2.right && !HasPlayerUnderBridge)
            {
                if (!player.IsHandleWire || (player.IsHandleWire && !HasWireUnderBridge))
                    return 5f;
            }
            } else if (this.IsHorizontal())
        {
            if (moveDirection == Vector2.up || moveDirection == Vector2.down && !HasPlayerUnderBridge)
            {
                if (!player.IsHandleWire || (player.IsHandleWire && !HasWireUnderBridge))
                    return 5f;
            }
            if (moveDirection == Vector2.left || moveDirection == Vector2.right && !HasPlayerOnBridge)
            {
                if (!player.IsHandleWire || (player.IsHandleWire && !HasWireOnBridge))
                    return 2f;
            }
        }
        return -1f;
    }
}