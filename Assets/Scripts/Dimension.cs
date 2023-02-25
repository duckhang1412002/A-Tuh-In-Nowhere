using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimension : MonoBehaviour
{
    [SerializeField]
    private bool dimensionTop;
    [SerializeField]
    private Vector2 topTeleporter;

    [SerializeField]
    private bool dimensionRight;
    [SerializeField]
    private Vector2 rightTeleporter;

    [SerializeField]
    private bool dimensionBottom;
    [SerializeField]
    private Vector2 bottomTeleporter;

    [SerializeField]
    private bool dimensionLeft;
    [SerializeField]
    private Vector2 leftTeleporter;

    private Dictionary<string,Vector2> targetTeleporterPosition;
    private Dictionary<Vector2,Vector2> previousTeleporterPosition;

    void Start()
    {
        targetTeleporterPosition = new Dictionary<string,Vector2>();
        previousTeleporterPosition = new Dictionary<Vector2,Vector2>();
        float x_axis =  this.transform.position.x;
        float y_axis = this.transform.position.y;
        if(dimensionTop){
            targetTeleporterPosition["Top"] = topTeleporter;
            previousTeleporterPosition[topTeleporter] = new Vector2(x_axis,y_axis+4);
        }
        if(dimensionRight){
            targetTeleporterPosition["Right"] = rightTeleporter;
            previousTeleporterPosition[topTeleporter] = new Vector2(x_axis+4,y_axis);
        }
        if(dimensionBottom){
            targetTeleporterPosition["Bottom"] = bottomTeleporter;
            previousTeleporterPosition[topTeleporter] = new Vector2(x_axis,y_axis-4);
        }
        if(dimensionLeft){
            targetTeleporterPosition["Left"] = leftTeleporter;
            previousTeleporterPosition[topTeleporter] = new Vector2(x_axis-4,y_axis);
        }
    }

    public Dictionary<string,Vector2> GetTargetTeleporterList(){
        return targetTeleporterPosition;
    }

    public Dictionary<Vector2,Vector2> GetPreviousTeleporterList(){
        return previousTeleporterPosition;
    }

    void Update()
    {

    }
}
