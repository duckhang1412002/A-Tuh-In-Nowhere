using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField] private float moveSteps = 4.0f;
    [SerializeField] private float moveSpeed = 20.0f;
    [SerializeField] private Sprite[] pipeSprites;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject body;
    private bool enableMove = true;
    private bool isNotPickPipe = true;
    private bool isAtPointPosition = false;

    private string handlePipeColor;
    private Vector2 positionOfStartPoint;
    private Vector2 currentPosition;
    private Vector2 targetPosition;
    private List<string> path;

    private Dictionary<Vector2, string> obstaclePosition;
    private Dictionary<Vector2, string> pointType;

    private Dictionary<Vector2, Bridge> bridgeType;
    private static float[] pipeRotation = { 0f, 90.0f, 180.0f, 270.0f };
    private GameObject[] walls;
    private GameObject[] pipePoints;

    private GameObject[] bridges;

    private float defaultZAxis = 6;
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = player.transform.position;
        targetPosition = player.transform.position;
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 6);
        obstaclePosition = new Dictionary<Vector2, string>();
        pointType = new Dictionary<Vector2, string>();
        bridgeType = new Dictionary<Vector2, Bridge>();
        path = new List<string>();
        path.Add("");
        handlePipeColor = "Default";

        walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject item in walls)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "Wall";
        }

        pipePoints = GameObject.FindGameObjectsWithTag("PipePoint");
        foreach (GameObject item in pipePoints)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "PipePoint";
            pointType[blockPosition] = item.GetComponent<PipePoint>().GetColorType();
        }

        bridges = GameObject.FindGameObjectsWithTag("Bridge");
        foreach (GameObject item in bridges)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "Bridge";
            bridgeType[blockPosition] = item.GetComponent<Bridge>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && enableMove)
        {
            Vector2 tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 tempNextMove = new Vector2(transform.position.x, transform.position.y + moveSteps);
            if (CanStepToPosition(tempCurrentPosition, tempNextMove, "Up"))
            {
                currentPosition = this.transform.position;
                targetPosition = tempNextMove;
                if (!isNotPickPipe) GeneratePipe("Up", currentPosition, targetPosition);
                CheckPipeEndPoint(targetPosition);
                if (isNotPickPipe && isAtPointPosition) GeneratePipe("Up", currentPosition, targetPosition);
                CheckPipeStartPoint(targetPosition);
                SetPreviousMove("Up");
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && enableMove)
        {
            Vector2 tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 tempNextMove = new Vector2(transform.position.x, transform.position.y - moveSteps);
            if (CanStepToPosition(tempCurrentPosition, tempNextMove, "Down"))
            {
                currentPosition = this.transform.position;
                targetPosition = tempNextMove;
                if (!isNotPickPipe) GeneratePipe("Down", currentPosition, targetPosition);
                CheckPipeEndPoint(targetPosition);
                if (isNotPickPipe && isAtPointPosition) GeneratePipe("Down", currentPosition, targetPosition);
                CheckPipeStartPoint(targetPosition);
                SetPreviousMove("Down");
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && enableMove)
        {
            Vector2 tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 tempNextMove = new Vector2(transform.position.x - moveSteps, transform.position.y);
            if (CanStepToPosition(tempCurrentPosition, tempNextMove, "Left"))
            {
                currentPosition = this.transform.position;
                targetPosition = tempNextMove;
                if (!isNotPickPipe) GeneratePipe("Left", currentPosition, targetPosition);
                CheckPipeEndPoint(targetPosition);
                if (isNotPickPipe && isAtPointPosition) GeneratePipe("Left", currentPosition, targetPosition);
                CheckPipeStartPoint(targetPosition);
                SetPreviousMove("Left");
            }
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && enableMove)
        {
            Vector2 tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 tempNextMove = new Vector2(transform.position.x + moveSteps, transform.position.y);
            if (CanStepToPosition(tempCurrentPosition, tempNextMove, "Right"))
            {
                currentPosition = this.transform.position;
                targetPosition = tempNextMove;
                if (!isNotPickPipe) GeneratePipe("Right", currentPosition, targetPosition);
                CheckPipeEndPoint(targetPosition);
                if (isNotPickPipe && isAtPointPosition) GeneratePipe("Right", currentPosition, targetPosition);
                CheckPipeStartPoint(targetPosition);
                SetPreviousMove("Right");
            }
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        StepMove();
    }

    void CheckPipeStartPoint(Vector2 targetPosition)
    {
        if (pointType.ContainsKey(targetPosition) && pointType[targetPosition] != "Done")
        {
            isNotPickPipe = false;
            isAtPointPosition = true;
            handlePipeColor = pointType[targetPosition];
            positionOfStartPoint = targetPosition;
            Debug.Log("Is start point --- " + handlePipeColor);

            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, handlePipeColor);
        }
    }

    void CheckPipeEndPoint(Vector2 targetPosition)
    {
        if (pointType.ContainsKey(targetPosition) && handlePipeColor == pointType[targetPosition])
        {
            isNotPickPipe = true;
            isAtPointPosition = true;                     
            pointType[targetPosition] = "Done";
            pointType[positionOfStartPoint] = "Done";
            Debug.Log("Is end point --- " + handlePipeColor);

            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, "Default");
        }
    }

    void GeneratePipe(string key, Vector2 currentPosition, Vector2 targetPosition)
    {
        obstaclePosition[currentPosition] = "Pipe";
        if(bridgeType.ContainsKey(currentPosition)) 
            obstaclePosition[currentPosition] = "Bridge";

        if (key == "Right")
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(currentPosition, 2, 0);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(targetPosition, 2, 2);
            }
            else
            {
                if (GetPreviousMove() == "Right")
                {
                    RenderPipe(currentPosition, 0, 0);
                }
                else if (GetPreviousMove() == "Down")
                {
                    RenderPipe(currentPosition, 1, 0);
                }
                else if (GetPreviousMove() =="Up")
                {
                    RenderPipe(currentPosition, 1, 3);
                }
            }
        }
        if (key == "Left")
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(currentPosition, 2, 2);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(targetPosition, 2, 0);
            }
            else
            {
                if (GetPreviousMove() == "Left")
                {
                    RenderPipe(currentPosition, 0, 0);
                }
                else if (GetPreviousMove() == "Down")
                {
                    RenderPipe(currentPosition, 1, 1);
                }
                else if (GetPreviousMove() == "Up")
                {
                    RenderPipe(currentPosition, 1, 2);
                }
            }
        }
        if (key == "Up")
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(currentPosition, 2, 1);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(targetPosition, 2, 3);
            }
            else
            {
                if (GetPreviousMove() == "Up")
                {
                    RenderPipe(currentPosition, 0, 1);
                }
                else if (GetPreviousMove() == "Left")
                {
                    RenderPipe(currentPosition, 1, 0);
                }
                else if (GetPreviousMove() == "Right")
                {
                    RenderPipe(currentPosition, 1, 1);
                }
            }
        }
        if (key == "Down")
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(currentPosition, 2, 3);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                isAtPointPosition = false;
                RenderPipe(targetPosition, 2, 1);
            }
            else
            {
                if (GetPreviousMove() == "Down")
                {
                    RenderPipe(currentPosition, 0, 1);
                }
                else if (GetPreviousMove() == "Left")
                {
                    RenderPipe(currentPosition, 1, 3);
                }
                else if (GetPreviousMove() == "Right")
                {
                    RenderPipe(currentPosition, 1, 2);
                }
            }
        }
    }

    private void RenderPipe(Vector2 renderPosition, int pipeTypeIndex, int pipeRotationIndex)
    {
        GameObject pipeClone = new GameObject();
        pipeClone.AddComponent<SpriteRenderer>();
        pipeClone.AddComponent<ChangeColor>();

        SpriteRenderer spriteRenderer = pipeClone.GetComponent<SpriteRenderer>();
        Transform transform = pipeClone.GetComponent<Transform>();
        ChangeColor changeColor = pipeClone.GetComponent<ChangeColor>();

        changeColor.Start();
        changeColor.ChangeSpriteColor(pipeClone, handlePipeColor);     
        
        spriteRenderer.sprite = pipeSprites[pipeTypeIndex];       
        transform.Rotate(0f, 0f, pipeRotation[pipeRotationIndex]);

        if(bridgeType.ContainsKey(renderPosition)){
            if(bridgeType[renderPosition].GetBridgeType() == "Vertical" 
            && (GetPreviousMove() == "Left" || GetPreviousMove() == "Right") 
            && bridgeType[renderPosition].HasPipeUnderBridge){
                transform.position = new Vector3(renderPosition.x, renderPosition.y, 6);
            }
            else if(bridgeType[renderPosition].GetBridgeType() == "Horizontal"
            && (GetPreviousMove() == "Up" || GetPreviousMove() == "Down"
            && bridgeType[renderPosition].HasPipeUnderBridge)){
                transform.position = new Vector3(renderPosition.x, renderPosition.y, 6);
            }
            else if(bridgeType[renderPosition].GetBridgeType() == "Vertical"
            && (GetPreviousMove() == "Up" || GetPreviousMove() == "Down"
            && bridgeType[renderPosition].HasPipeOnBridge)){
                transform.position = new Vector3(renderPosition.x, renderPosition.y, 3);
            }
            else if(bridgeType[renderPosition].GetBridgeType() == "Horizontal"
            && (GetPreviousMove() == "Left" || GetPreviousMove() == "Right"
            && bridgeType[renderPosition].HasPipeOnBridge)){
                transform.position = new Vector3(renderPosition.x, renderPosition.y, 3);
            }
        } else{
            transform.position = new Vector3(renderPosition.x, renderPosition.y, 7);
        }        
    }

    private bool CanStepToPosition(Vector2 currentPosition, Vector2 targetPosition, string tempNextKey)
    {     
        bool totalCheck = true; 
        if(obstaclePosition.ContainsKey(currentPosition) && obstaclePosition[currentPosition] == "Bridge"){
            bool isOnBridge = false;
            Bridge bridge = bridgeType[currentPosition];

            if ((bridge.GetBridgeType() == "Horizontal" && (GetPreviousMove() == "Left" || GetPreviousMove() == "Right"))
            || (bridge.GetBridgeType() == "Vertical" && (GetPreviousMove() == "Up" || GetPreviousMove() == "Down")))
                isOnBridge = true;
           
            if(isOnBridge){
                if ((bridge.GetBridgeType() == "Horizontal" && (tempNextKey == "Up" || tempNextKey == "Down"))
                || (bridge.GetBridgeType() == "Vertical" && (tempNextKey == "Left" || tempNextKey == "Right")))
                    return false;
                
                if(!isNotPickPipe) bridge.HasPipeOnBridge = true;

                if(!obstaclePosition.ContainsKey(targetPosition)) totalCheck = true;
                else if(isNotPickPipe) totalCheck = true;
                else if(obstaclePosition[targetPosition] == "Pipe") totalCheck = false;
            }else{
                if ((bridge.GetBridgeType() == "Horizontal" && (tempNextKey == "Left" || tempNextKey == "Right"))
                || (bridge.GetBridgeType() == "Vertical" && (tempNextKey == "Up" || tempNextKey == "Down")))
                    return false;

                if(!isNotPickPipe) bridge.HasPipeUnderBridge = true;

                if(!obstaclePosition.ContainsKey(targetPosition)) totalCheck = true;
                else if(isNotPickPipe) totalCheck = true;
                else if(obstaclePosition[targetPosition] == "Pipe" && !isNotPickPipe) totalCheck = false;
            }
        }
        if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Bridge") {
            bool isOnBridge = false;
            Bridge bridge = bridgeType[targetPosition];

            if ((bridge.GetBridgeType() == "Horizontal" && (tempNextKey == "Left" ||tempNextKey == "Right"))
            || (bridge.GetBridgeType() == "Vertical" && (tempNextKey == "Up" || tempNextKey == "Down")))
                isOnBridge = true;
           
            if(isOnBridge){
                if(bridge.HasPipeOnBridge && !isNotPickPipe){
                    return false;
                }
                defaultZAxis = 2;
            }else{
                if(bridge.HasPipeUnderBridge && !isNotPickPipe){
                    return false;
                }
                defaultZAxis = 5;
            }           
        }       
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Pipe")
        {
            if (!isNotPickPipe) totalCheck = false;
        }
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Wall")
        {
            totalCheck = false;
        }
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "PipePoint")
        {
            if (pointType.ContainsKey(targetPosition) && handlePipeColor != pointType[targetPosition] && !isNotPickPipe)
                totalCheck = false;
        }      
        return totalCheck;
    }

    void StepMove()
    {
        this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, targetPosition.y, defaultZAxis), moveSpeed * Time.deltaTime);
        if (new Vector2(this.transform.position.x, this.transform.position.y) != targetPosition)
        {
            enableMove = false;
        }
        else
        {
            enableMove = true;
        }
    }

    private string GetPreviousMove()
    {
        return path[path.Count - 1];
    }

    private void SetPreviousMove(string move)
    {
        path.Add(move);
    }
}
