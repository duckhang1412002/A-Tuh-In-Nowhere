using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    private static float[] wireRotation = { 0f, 90.0f, 180.0f, 270.0f };   
    public float wireZAxis{get; set;}

    [SerializeField]
    private Sprite[] wireSprites;
    private GameObject wireInstance;
    private GameObject wireClone;

    // Start is called before the first frame update
    public void Start()
    {
        wireZAxis = 7f;
        wireSprites = new Sprite[3];       
        wireSprites[0] = Resources.Load<Sprite>("Sprites/Wire/straight");
        wireSprites[1] = Resources.Load<Sprite>("Sprites/Wire/curve");
        wireSprites[2] = Resources.Load<Sprite>("Sprites/Wire/cap");
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        wireInstance = gameManager.GetPrefabByName("Wire");
    }

    public GameObject GenerateWire(Player player)
    {
        GameObject wire = null;
        if (player.TempNextKey == "Right")
        {
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                wire = RenderWire(player.CurrentPosition, 2, 0, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                wire = RenderWire(player.TargetPosition, 2, 2, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (player.PreviousMove == "Right")
                {
                    wire = RenderWire(player.CurrentPosition, 0, 0, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Down")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 0, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Up")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 3, player.HandleWireColor);
                }
            }
        }
        if (player.TempNextKey == "Left")
        {
            Debug.Log(player.TargetPosition);
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                wire = RenderWire(player.CurrentPosition, 2, 2, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                wire = RenderWire(player.TargetPosition, 2, 0, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (player.PreviousMove == "Left")
                {
                    wire = RenderWire(player.CurrentPosition, 0, 0, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Down")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 1, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Up")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 2, player.HandleWireColor);
                }
            }
        }
        if (player.TempNextKey == "Up")
        {
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                wire = RenderWire(player.CurrentPosition, 2, 1, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                wire = RenderWire(player.TargetPosition, 2, 3, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (player.PreviousMove == "Up")
                {
                    wire = RenderWire(player.CurrentPosition, 0, 1, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Left")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 0, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Right")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 1, player.HandleWireColor);
                }
            }
        }
        if (player.TempNextKey == "Down")
        {
            if (player.IsAtSocket && !player.IsNotPickWire)
            {
                wire = RenderWire(player.CurrentPosition, 2, 3, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else if (player.IsAtSocket && player.IsNotPickWire)
            {
                wire = RenderWire(player.TargetPosition, 2, 1, player.HandleWireColor);
                player.IsAtSocket = false;
            }
            else
            {
                if (player.PreviousMove == "Down")
                {
                    wire = RenderWire(player.CurrentPosition, 0, 1, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Left")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 3, player.HandleWireColor);
                }
                else if (player.PreviousMove == "Right")
                {
                    wire = RenderWire(player.CurrentPosition, 1, 2, player.HandleWireColor);
                }
            }
        }
        if (wire == null) Debug.Log("Can not render wire!");
        return wire;
    }

    //new Vector 3
    public GameObject RenderWire(Vector3 renderPosition, int pipeTypeIndex, int wireRotationIndex, string handleWireColor)
    {
        // Optionally, you can specify a position and rotation for the instance
        //wireClone = Instantiate(wireInstance, renderPosition, Quaternion.identity);
        wireClone = PhotonNetwork.Instantiate(wireInstance.name, renderPosition, Quaternion.identity);
        wireClone.name = "Wire" + handleWireColor;

        SpriteRenderer spriteRenderer = wireClone.GetComponent<SpriteRenderer>();
        Transform transform = wireClone.GetComponent<Transform>();
        ChangeColor changeColor = wireClone.GetComponent<ChangeColor>();

        changeColor.Start();
        changeColor.ChangeSpriteColor(wireClone, handleWireColor);

        spriteRenderer.sprite = wireSprites[pipeTypeIndex];
        transform.Rotate(0f, 0f, wireRotation[wireRotationIndex]);

        transform.position = new Vector3(renderPosition.x, renderPosition.y, renderPosition.z);

        return wireClone;
    }

    //Vector 2
    public GameObject RenderWire(Vector2 renderPosition, int pipeTypeIndex, int wireRotationIndex, string handleWireColor)
    {
        // Optionally, you can specify a position and rotation for the instance
        //wireClone = Instantiate(wireInstance, renderPosition, Quaternion.identity);
        wireClone = PhotonNetwork.Instantiate(wireInstance.name, renderPosition, Quaternion.identity);
        wireClone.name = "Wire" + handleWireColor;

        SpriteRenderer spriteRenderer = wireClone.GetComponent<SpriteRenderer>();
        Transform transform = wireClone.GetComponent<Transform>();
        ChangeColor changeColor = wireClone.GetComponent<ChangeColor>();

        changeColor.Start();
        changeColor.ChangeSpriteColor(wireClone, handleWireColor);

        spriteRenderer.sprite = wireSprites[pipeTypeIndex];
        transform.Rotate(0f, 0f, wireRotation[wireRotationIndex]);

        transform.position = new Vector3(renderPosition.x, renderPosition.y, wireZAxis);

        return wireClone;
    }
}
