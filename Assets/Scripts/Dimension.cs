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

    [SerializeField]
    private Camera previousBaseCam;
    [SerializeField]
    private Camera targetBaseCam;

    private Dictionary<string,Vector2> targetTeleporterPosition;
    private Dictionary<Vector2,Vector2> previousTeleporterPosition;

    void Start()
    {
        targetTeleporterPosition = new Dictionary<string,Vector2>();
        previousTeleporterPosition = new Dictionary<Vector2,Vector2>();
        float x_axis =  this.transform.position.x;
        float y_axis = this.transform.position.y;
        if(dimensionTop){
            targetTeleporterPosition["Top"] = new Vector2(topTeleporter.x, topTeleporter.y-4);
            previousTeleporterPosition[topTeleporter] = new Vector2(x_axis,y_axis+4);
        }
        if(dimensionRight){
            targetTeleporterPosition["Right"] = new Vector2(rightTeleporter.x-4,rightTeleporter.y);
            previousTeleporterPosition[rightTeleporter] = new Vector2(x_axis+4,y_axis);
        }
        if(dimensionBottom){
            targetTeleporterPosition["Bottom"] = new Vector2(bottomTeleporter.x,bottomTeleporter.y+4);
            previousTeleporterPosition[bottomTeleporter] = new Vector2(x_axis,y_axis-4);
        }
        if(dimensionLeft){
            targetTeleporterPosition["Left"] = new Vector2(leftTeleporter.x+4,leftTeleporter.y);
            previousTeleporterPosition[leftTeleporter] = new Vector2(x_axis-4,y_axis);
        }
    }

    public Dictionary<string,Vector2> GetTargetTeleporterList(){
        return targetTeleporterPosition;
    }

    public Dictionary<Vector2,Vector2> GetPreviousTeleporterList(){
        return previousTeleporterPosition;
    }

    public void SetTargetBaseCamera(){
        targetBaseCam.enabled = true;
        previousBaseCam.enabled = false;
    }

    public void SetPreviousBaseCamera(){
        previousBaseCam.enabled = true;
        targetBaseCam.enabled = false;
    }

    void Update()
    {

    }
}
