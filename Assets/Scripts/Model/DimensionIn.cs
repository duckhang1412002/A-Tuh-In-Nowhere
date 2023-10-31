using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionIn : MonoBehaviour
{
    public int ID { get; set; }
    public GameObject exitTop { get; set; }
    public GameObject exitRight { get; set; }
    public GameObject exitBottom { get; set; }
    public GameObject exitLeft { get; set; }

    private Sprite[] dimensionInSprites;
    private static float[] dimensionInRotation = { 0f, 90.0f, 180.0f, 270.0f };

    public void Start()
    {
        dimensionInSprites = new Sprite[5];
        dimensionInSprites[0] = Resources.Load<Sprite>("Sprites/DimensionIn/dimension-in-1");
        dimensionInSprites[1] = Resources.Load<Sprite>("Sprites/DimensionIn/dimension-in-2");
        dimensionInSprites[2] = Resources.Load<Sprite>("Sprites/DimensionIn/dimension-in-3");
        dimensionInSprites[3] = Resources.Load<Sprite>("Sprites/DimensionIn/dimension-in-4");
        dimensionInSprites[4] = Resources.Load<Sprite>("Sprites/DimensionIn/dimension-in-5");
    }

    public Vector2 GetEntrancePosition(Vector2 moveDirection)
    {
        Vector2 entrancePosition = Vector2.zero;
        if (moveDirection == Vector2.right && HasLeft())
        {
            entrancePosition = new Vector3(exitLeft.transform.position.x + 1, exitLeft.transform.position.y);
        }
        else if (moveDirection == Vector2.down && HasTop())
        {
            entrancePosition = new Vector3(exitTop.transform.position.x, exitTop.transform.position.y - 1);
        }
        else if (moveDirection == Vector2.left && HasRight())
        {
            entrancePosition = new Vector3(exitRight.transform.position.x - 1, exitRight.transform.position.y);
        }
        else if (moveDirection == Vector2.up && HasBottom())
        {
            entrancePosition = new Vector3(exitBottom.transform.position.x, exitBottom.transform.position.y + 1);
        }

        return entrancePosition;
    }

    public GameObject GetDimensionOut(Vector2 moveDirection)
    {
        GameObject dimOut = null;
        if (moveDirection == Vector2.right && HasLeft())
        {
            dimOut = exitLeft;
        }
        else if (moveDirection == Vector2.down && HasTop())
        {
            dimOut = exitTop;
        }
        else if (moveDirection == Vector2.left && HasRight())
        {
            dimOut = exitRight;
        }
        else if (moveDirection == Vector2.up && HasBottom())
        {
            dimOut = exitBottom;
        }

        return dimOut;
    }

    public Vector3 GetNextPosition(Player player)
    {
        Vector3 entrancePosition = this.transform.position;
        if (player.TempNextKey == "Right" && HasLeft())
        {          
            entrancePosition = new Vector3(exitLeft.transform.position.x+1, exitLeft.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Down" && HasTop())
        {          
            entrancePosition = new Vector3(exitTop.transform.position.x, exitTop.transform.position.y-1, player.transform.position.z);
        }
        else if (player.TempNextKey == "Left" && HasRight())
        {          
            entrancePosition = new Vector3(exitRight.transform.position.x-1, exitRight.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Up" && HasBottom())
        {          
            entrancePosition = new Vector3(exitBottom.transform.position.x, exitBottom.transform.position.y+1, player.transform.position.z);
        }

        return entrancePosition;
    }

    public void RenderSprite(){
        //Debug.Log(HasTop() + " " + HasRight() + " " + HasBottom() + " " + HasLeft());
        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        Transform transform = this.gameObject.GetComponent<Transform>();    

        if(HasTop() && !HasRight() && !HasBottom() && !HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[0];
            transform.Rotate(0f, 0f, dimensionInRotation[0]); 
        } else if (!HasTop() && !HasRight() && !HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[0];
            transform.Rotate(0f, 0f, dimensionInRotation[1]); 
        } else if (!HasTop() && !HasRight() && HasBottom() && !HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[0];
            transform.Rotate(0f, 0f, dimensionInRotation[2]);
        } else if (!HasTop() && HasRight() && !HasBottom() && !HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[0];
            transform.Rotate(0f, 0f, dimensionInRotation[3]);
        } else if (HasTop() && !HasRight() && HasBottom() && !HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[1];
            transform.Rotate(0f, 0f, dimensionInRotation[0]);
        } else if (!HasTop() && HasRight() && !HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[1];
            transform.Rotate(0f, 0f, dimensionInRotation[1]);
        } else if (HasTop() && !HasRight() && !HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[2];
            transform.Rotate(0f, 0f, dimensionInRotation[0]);
        } else if (!HasTop() && HasRight() && HasBottom() && !HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[2];
            transform.Rotate(0f, 0f, dimensionInRotation[1]);
        } else if (!HasTop() && !HasRight() && HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[2];
            transform.Rotate(0f, 0f, dimensionInRotation[2]);
        } else if (HasTop() && HasRight() && !HasBottom() && !HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[2];
            transform.Rotate(0f, 0f, dimensionInRotation[3]);
        } else if (HasTop() && HasRight() && !HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[3];
            transform.Rotate(0f, 0f, dimensionInRotation[0]);
        } else if (HasTop() && !HasRight() && HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[3];
            transform.Rotate(0f, 0f, dimensionInRotation[1]);
        } else if (!HasTop() && HasRight() && HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[3];
            transform.Rotate(0f, 0f, dimensionInRotation[2]);
        } else if (HasTop() && HasRight() && HasBottom() && !HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[3];
            transform.Rotate(0f, 0f, dimensionInRotation[3]);
        } else if (HasTop() && HasRight() && HasBottom() && HasLeft()){
            spriteRenderer.sprite = dimensionInSprites[4];
            transform.Rotate(0f, 0f, dimensionInRotation[0]);
        } 
    }

    private bool HasTop(){
        return exitTop != null ? true : false;
    }
    private bool HasLeft(){
        return exitLeft != null ? true : false;
    }
    private bool HasBottom(){
        return exitBottom != null ? true : false;
    }
    private bool HasRight(){
        return exitRight != null ? true : false;
    }

    public GameObject GetDimensionOut(Player player){
        if (player.TempNextKey == "Right" && HasLeft())
        {          
            return exitLeft;
        }
        else if (player.TempNextKey == "Down" && HasTop())
        {          
            return exitTop;
        }
        else if (player.TempNextKey == "Left" && HasRight())
        {          
            return exitRight;
        }
        else if (player.TempNextKey == "Up" && HasBottom())
        {          
            return exitBottom;
        }

        return null;
    }

    public bool CheckNextStep(Player player, GameObject nextStepObject, Dictionary<Vector2,bool> wireMap){
        bool totalCheck = true;

        if(wireMap.ContainsKey(this.GetNextPosition(player)) && !player.IsNotPickWire){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Socket" && !player.IsNotPickWire 
        && nextStepObject.GetComponent<Socket>().Color != player.HandleWireColor
        && nextStepObject.GetComponent<Socket>().IsConnect == false){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Bridge" && !nextStepObject.GetComponent<Bridge>().CheckNextStep(player)){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Wall"){
            totalCheck = false;
        }

        return totalCheck;
    }

    void Update()
    {

    }
}
