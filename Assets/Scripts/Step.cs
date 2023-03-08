using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private float moveSteps = 4.0f;
    [SerializeField] private float moveSpeed = 20.0f;
    [SerializeField] private Sprite[] pipeSprites;
    private GameObject player;
    [SerializeField] private GameObject body;

    private bool enableMove = true;
    private bool isNotPickPipe = true;
    private bool isAtPointPosition = false;
    private string handlePipeColor;
    private Vector2 positionOfStartPoint;
    private Vector2 currentPosition;
    private Vector2 targetPosition;
    private Vector2 tempCurrentPosition;
    private Vector2 tempTargetPosition;
    private Vector2 entranceDimensionPosition;

    private List<string> path;
    private Dictionary<Vector2, string> obstaclePosition;
    private Dictionary<Vector2, PipePoint> pointType;
    private Dictionary<Vector2, Bridge> bridgeType;
    private Dictionary<Vector2, Dimension> dimensionType;
    private Dictionary<Vector2, DimensionTeleporter> dimensionTeleporterType;
    private Dictionary<Vector2, DoorButton> doorButtonType;
    private Dictionary<Vector2, Door> doorType;
    private static float[] pipeRotation = { 0f, 90.0f, 180.0f, 270.0f };
    private float defaultZAxis = 6;
    // Start is called before the first frame update
    void Start()
    {
        gameManager.GetComponent<GameManager>().Start();

        player = gameManager.GetComponent<GameManager>().GetPlayer();
        currentPosition = player.transform.position;
        targetPosition = player.transform.position;
        tempCurrentPosition = player.transform.position;
        tempTargetPosition = player.transform.position;
        entranceDimensionPosition = player.transform.position;
        obstaclePosition = gameManager.GetComponent<GameManager>().GetObstaclePosition();
        pointType = gameManager.GetComponent<GameManager>().GetPointType();
        bridgeType = gameManager.GetComponent<GameManager>().GetBridgeType();
        dimensionType = gameManager.GetComponent<GameManager>().GetDimensionType();
        dimensionTeleporterType = gameManager.GetComponent<GameManager>().GetDimensionTeleporterType();
        doorButtonType = gameManager.GetComponent<GameManager>().GetDoorButtonType();
        doorType = gameManager.GetComponent<GameManager>().GetDoorType();
        path = gameManager.GetComponent<GameManager>().GetPath();
        
        handlePipeColor = "Default";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && enableMove)
        {
            tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            tempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
            if (CanStepToPosition(tempCurrentPosition, tempTargetPosition, "Up"))
            {
                currentPosition = this.transform.position;
                if((obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "Dimension")
                || (obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "DimensionTeleporter"))
                    currentPosition = tempCurrentPosition;

                targetPosition = tempTargetPosition;
                if (!isNotPickPipe) GeneratePipe("Up", currentPosition, targetPosition);
                CheckPipeEndPoint(targetPosition);
                if (isNotPickPipe && isAtPointPosition) GeneratePipe("Up", currentPosition, targetPosition);
                CheckPipeStartPoint(targetPosition);
                SetPreviousMove("Up");
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && enableMove)
        {
            tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            tempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
            if (CanStepToPosition(tempCurrentPosition, tempTargetPosition, "Down"))
            {
                currentPosition = this.transform.position;
                if((obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "Dimension")
                || (obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "DimensionTeleporter"))
                    currentPosition = tempCurrentPosition;

                targetPosition = tempTargetPosition;
                if (!isNotPickPipe) GeneratePipe("Down", currentPosition, targetPosition);
                CheckPipeEndPoint(targetPosition);
                if (isNotPickPipe && isAtPointPosition) GeneratePipe("Down", currentPosition, targetPosition);
                CheckPipeStartPoint(targetPosition);
                SetPreviousMove("Down");
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && enableMove)
        {
            tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            tempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
            if (CanStepToPosition(tempCurrentPosition, tempTargetPosition, "Left"))
            {
                currentPosition = this.transform.position;
                if((obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "Dimension")
                || (obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "DimensionTeleporter"))
                    currentPosition = tempCurrentPosition;
                
                targetPosition = tempTargetPosition;
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
            tempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            tempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
            if (CanStepToPosition(tempCurrentPosition, tempTargetPosition, "Right"))
            {
                currentPosition = this.transform.position;
                if((obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "Dimension")
                || (obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "DimensionTeleporter"))
                    currentPosition = tempCurrentPosition;
                
                targetPosition = tempTargetPosition;
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
        if (pointType.ContainsKey(targetPosition) && pointType[targetPosition].IsConnect == false)
        {
            isNotPickPipe = false;
            isAtPointPosition = true;
            handlePipeColor = pointType[targetPosition].GetColorType();
            positionOfStartPoint = targetPosition;
            Debug.Log("Is start point --- " + handlePipeColor);

            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, handlePipeColor);
        }
    }

    void CheckPipeEndPoint(Vector2 targetPosition)
    {
        if (pointType.ContainsKey(targetPosition) && handlePipeColor == pointType[targetPosition].GetColorType() &&  !pointType[targetPosition].IsConnect)
        {
            isNotPickPipe = true;
            isAtPointPosition = true;                     
            pointType[targetPosition].IsConnect = true;
            pointType[positionOfStartPoint].IsConnect = true;
            Debug.Log("Is end point --- " + handlePipeColor);

            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, "Default");
            gameManager.GetComponent<GameManager>().Score++;
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
        else if(obstaclePosition.ContainsKey(currentPosition) && obstaclePosition[currentPosition] == "DoorButton"){
            DoorButton button = doorButtonType[currentPosition];
            Door door = button.GetDoor();
            button.IsActive = false;
            if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Wall")
                button.IsActive = true;
            else if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Pipe" && !isNotPickPipe)
                button.IsActive = true;
            else if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "PipePoint" && pointType[targetPosition].IsConnect == true)
            button.IsActive = true;

            if(!isNotPickPipe){
                button.HasPipeOn = true;
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
        else if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Dimension"){
            Dimension dimension = dimensionType[targetPosition];

            if(tempNextKey == "Right" && dimension.GetTargetTeleporterList().ContainsKey("Left")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Left"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !isNotPickPipe)
                    return false;
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && handlePipeColor != pointType[entranceTeleporterPosition].GetColorType() && !isNotPickPipe)
                        return false;
                }

                tempCurrentPosition = player.transform.position;                
                tempTargetPosition = entranceTeleporterPosition;
                entranceDimensionPosition = dimension.transform.position;
                player.transform.position = tempTargetPosition;
                /*n*/
                if(!isNotPickPipe) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x-4,entranceTeleporterPosition.y);
                    RenderPipe(ladder, 0, 0);
                }                
            }
            else if(tempNextKey == "Left" && dimension.GetTargetTeleporterList().ContainsKey("Right")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Right"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !isNotPickPipe)
                    return false;
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && handlePipeColor != pointType[entranceTeleporterPosition].GetColorType() && !isNotPickPipe)
                        return false;
                }

                tempCurrentPosition = player.transform.position;
                tempTargetPosition = dimension.GetTargetTeleporterList()["Right"];
                entranceDimensionPosition = dimension.transform.position;
                player.transform.position = tempTargetPosition; 
                /*n*/
                if(!isNotPickPipe) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x+4,entranceTeleporterPosition.y);
                    RenderPipe(ladder, 0, 0);
                }   
            }
            else if(tempNextKey == "Up" && dimension.GetTargetTeleporterList().ContainsKey("Bottom")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Bottom"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !isNotPickPipe)
                    return false;
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && handlePipeColor != pointType[entranceTeleporterPosition].GetColorType() && !isNotPickPipe)
                        return false;
                }

                tempCurrentPosition = player.transform.position;
                tempTargetPosition = dimension.GetTargetTeleporterList()["Bottom"];
                entranceDimensionPosition = dimension.transform.position;
                player.transform.position = tempTargetPosition;
                /*n*/
                if(!isNotPickPipe) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x,entranceTeleporterPosition.y-4);
                    RenderPipe(ladder, 0, 1);
                }  
            }
            else if(tempNextKey == "Down" && dimension.GetTargetTeleporterList().ContainsKey("Top")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Top"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !isNotPickPipe)
                    return false;
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && handlePipeColor != pointType[entranceTeleporterPosition].GetColorType() && !isNotPickPipe)
                        return false;
                }

                tempCurrentPosition = player.transform.position;               
                tempTargetPosition = dimension.GetTargetTeleporterList()["Top"];  
                entranceDimensionPosition = dimension.transform.position;              
                player.transform.position = tempTargetPosition; 
                /*n*/
                if(!isNotPickPipe) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x,entranceTeleporterPosition.y+4);
                    RenderPipe(ladder, 0, 1);
                } 
            }else{
                return false;
            }

            defaultZAxis = 6;
            dimension.SetTargetBaseCamera();    
        }
        else if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "DimensionTeleporter"){
            DimensionTeleporter dimensionTeleporter = dimensionTeleporterType[targetPosition];
            Dimension dimension = dimensionType[dimensionTeleporter.getBaseDimension()];
            Vector2 previousBaseDimensionEntrance = dimension.GetPreviousTeleporterList()[targetPosition];

            if(obstaclePosition.ContainsKey(previousBaseDimensionEntrance) && obstaclePosition[previousBaseDimensionEntrance] == "Pipe" && !isNotPickPipe)
                return false;
            else if(obstaclePosition.ContainsKey(previousBaseDimensionEntrance) && obstaclePosition[previousBaseDimensionEntrance] == "PipePoint"){
                if (pointType.ContainsKey(previousBaseDimensionEntrance) && handlePipeColor != pointType[previousBaseDimensionEntrance].GetColorType() && !isNotPickPipe)
                    return false;
            }

            tempTargetPosition = previousBaseDimensionEntrance;
            entranceDimensionPosition = dimensionTeleporter.transform.position;
            player.transform.position = tempTargetPosition;
            defaultZAxis = 6;
            dimension.SetPreviousBaseCamera();   

            /*n*/
            if(!isNotPickPipe && (tempNextKey == "Right" || tempNextKey == "Left")) {
                Vector2 ladder = new Vector2(dimensionTeleporter.transform.position.x,dimensionTeleporter.transform.position.y);
                RenderPipe(ladder, 0, 0);
            } else if(!isNotPickPipe && (tempNextKey == "Up" || tempNextKey == "Down")) {
                Vector2 ladder = new Vector2(dimensionTeleporter.transform.position.x,dimensionTeleporter.transform.position.y);
                RenderPipe(ladder, 0, 1);
            }    
        }  
        else if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "DoorButton"){
            DoorButton button = doorButtonType[targetPosition];
            Door door = button.GetDoor();
            button.IsActive = true;
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
            if (pointType.ContainsKey(targetPosition) && handlePipeColor != pointType[targetPosition].GetColorType() && !isNotPickPipe)
                totalCheck = false;
        } 
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Door")
        {   
            Door door = doorType[targetPosition];
            if(!door.IsActive) return false;
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
