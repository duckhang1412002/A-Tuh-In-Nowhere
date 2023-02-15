using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField] private float moveSteps = 2.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Sprite[] pipeSprites;
    private bool enableMove = true;
    private bool isNotPickPipe = true;
    private bool isAtPointPosition = false;

    private string handlePipeColor;
    private int indexOfStartPoint;
    private Vector2 currentPosition = new Vector2(0, 0);
    private Vector2 targetPosition = new Vector2(0, 0);
    private List<string> path;
    private List<Vector2> obstaclePosition;
    private List<string> obstacleType;
    private List<Vector2> pointPosition;
    private List<string> pointType;
    private float[] pipeRotation;
    private GameObject[] walls;
    private GameObject[] pipePoints;

    public GameObject body;

    // Start is called before the first frame update
    void Start()
    {       
        currentPosition = this.transform.position;
        path = new List<string>();
        pipeRotation = new float[4] { 0f, 90.0f, 180.0f, 270.0f };
        pointPosition = new List<Vector2>();
        pointType = new List<string>();
        obstaclePosition = new List<Vector2>();
        obstacleType = new List<string>();
        handlePipeColor = "Null";

        walls = GameObject.FindGameObjectsWithTag("Wall");
        for (int i = 0; i < walls.Length; i++)
        {
            Vector2 blockPosition = new Vector2(walls[i].GetComponent<Transform>().position.x, walls[i].GetComponent<Transform>().position.y);
            obstaclePosition.Add(blockPosition);
            obstacleType.Add("Wall");
        }

        pipePoints = GameObject.FindGameObjectsWithTag("PipePoint");
        for (int i = 0; i < pipePoints.Length; i++)
        {
            Vector2 blockPosition = new Vector2(pipePoints[i].GetComponent<Transform>().position.x, pipePoints[i].GetComponent<Transform>().position.y);
            obstaclePosition.Add(blockPosition);
            obstacleType.Add("PipePoint");
            pointPosition.Add(blockPosition);
            pointType.Add(pipePoints[i].GetComponent<PipePoint>().GetColorType());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && enableMove)
        {
            Vector2 tempNextMove = new Vector2(transform.position.x, transform.position.y + moveSteps);
            if (CanStepToPosition(tempNextMove))
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
            Vector2 tempNextMove = new Vector2(transform.position.x, transform.position.y - moveSteps);
            if (CanStepToPosition(tempNextMove))
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
            Vector2 tempNextMove = new Vector2(transform.position.x - moveSteps, transform.position.y);
            if (CanStepToPosition(tempNextMove))
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
            Vector2 tempNextMove = new Vector2(transform.position.x + moveSteps, transform.position.y);
            if (CanStepToPosition(tempNextMove))
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
        for (int i = 0; i < pointPosition.Count; i++)
        {
            if (targetPosition == pointPosition[i] && !pointType[i].Equals("Done"))
            {
                isNotPickPipe = false;
                isAtPointPosition = true;
                handlePipeColor = pointType[i];
                indexOfStartPoint = i;               
                Debug.Log("Is start point --- " + handlePipeColor);

                body.GetComponent<ChangeColor>().ChangeBodyColor(body, handlePipeColor);
                return;
            }
        }
    }

    void CheckPipeEndPoint(Vector2 targetPosition)
    {
        for (int i = 0; i < pointPosition.Count; i++)
        {
            if (targetPosition == pointPosition[i] && handlePipeColor.Equals(pointType[i]))
            {
                isNotPickPipe = true;
                isAtPointPosition = true;
                handlePipeColor = "Null";
                pointType[i] = "Done";
                pointType[indexOfStartPoint] = "Done";
                Debug.Log("Is end point --- " + handlePipeColor);

                body.GetComponent<ChangeColor>().ChangeBodyColor(body, "Default");
                return;
            }
        }
    }

    void GeneratePipe(string key, Vector2 currentPosition, Vector2 targetPosition)
    {
        GameObject pipe = new GameObject();
        pipe.name = "PipeClone" + handlePipeColor;
        pipe.AddComponent<SpriteRenderer>();

        obstaclePosition.Add(currentPosition);
        obstacleType.Add("Pipe");

        if (key.Equals("Right"))
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[0]);
                isAtPointPosition = false;
                RenderPipe(pipe, currentPosition);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[2]);
                isAtPointPosition = false;
                RenderPipe(pipe, targetPosition);
            }
            else
            {
                if (GetPreviousMove().Equals("Right"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[0];
                }
                else if (GetPreviousMove().Equals("Down"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[0]);
                }
                else if (GetPreviousMove().Equals("Up"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[3]);
                }
                RenderPipe(pipe, currentPosition);
            }
        }
        if (key.Equals("Left"))
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[2]);
                isAtPointPosition = false;
                RenderPipe(pipe, currentPosition);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[0]);
                isAtPointPosition = false;
                RenderPipe(pipe, targetPosition);
            }
            else
            {
                if (GetPreviousMove().Equals("Left"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[0];
                }
                else if (GetPreviousMove().Equals("Down"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[1]);
                }
                else if (GetPreviousMove().Equals("Up"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[2]);
                }
                RenderPipe(pipe, currentPosition);
            }
        }
        if (key.Equals("Up"))
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[1]);
                isAtPointPosition = false;
                RenderPipe(pipe, currentPosition);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[3]);
                isAtPointPosition = false;
                RenderPipe(pipe, targetPosition);
            }
            else
            {
                if (GetPreviousMove().Equals("Up"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[0];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[1]);
                }
                else if (GetPreviousMove().Equals("Left"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[0]);
                }
                else if (GetPreviousMove().Equals("Right"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[1]);
                }
                RenderPipe(pipe, currentPosition);
            }
        }
        if (key.Equals("Down"))
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[3]);
                isAtPointPosition = false;
                RenderPipe(pipe, currentPosition);
            }
            else if (isAtPointPosition && isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[1]);
                isAtPointPosition = false;
                RenderPipe(pipe, targetPosition);
            }
            else
            {
                if (GetPreviousMove().Equals("Down"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[0];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[1]);
                }
                else if (GetPreviousMove().Equals("Left"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[3]);
                }
                else if (GetPreviousMove().Equals("Right"))
                {
                    pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[1];
                    pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[2]);
                }
                RenderPipe(pipe, currentPosition);
            }
        }
    }

    private void RenderPipe(GameObject pipe, Vector2 renderPosition)
    {
        pipe.GetComponent<Transform>().position = new Vector3(renderPosition.x, renderPosition.y, 1);
        pipe.AddComponent<ChangeColor>();
        pipe.GetComponent<ChangeColor>().ChangeBodyColor(pipe, handlePipeColor);              
    }

    private bool CanStepToPosition(Vector2 targetMove)
    {
        for (int i = 0; i < obstaclePosition.Count; i++)
        {
            if (obstacleType[i] == "Pipe" && targetMove == obstaclePosition[i])
            {
                if (!isNotPickPipe) return false;
            }
            else if (obstacleType[i] == "Wall" && targetMove == obstaclePosition[i])
            {
                return false;
            }
            else if(obstacleType[i] == "PipePoint" && targetMove == obstaclePosition[i]){               
                if(!handlePipeColor.Equals(GetPointType(targetMove)) && !isNotPickPipe)
                    return false;
            }
        }
        return true;
    }

    void StepMove()
    {
        this.transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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
    private string GetPointType(Vector2 coordinates){
        for(int i=0; i<pointPosition.Count; i++){
            if(coordinates == pointPosition[i])
                return pointType[i];
        }
        return "Null";
    }

}
