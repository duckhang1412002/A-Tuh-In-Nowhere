using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField] private float moveSteps = 4.0f;
    [SerializeField] private float moveSpeed = 20.0f;
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

    public GameObject pipeClone;

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
        handlePipeColor = "Default";

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

                body.GetComponent<ChangeColor>().ChangeSpriteColor(body, handlePipeColor);
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
                handlePipeColor = "Default";
                pointType[i] = "Done";
                pointType[indexOfStartPoint] = "Done";
                Debug.Log("Is end point --- " + handlePipeColor);

                body.GetComponent<ChangeColor>().ChangeSpriteColor(body, "Default");
                return;
            }
        }
    }

    void GeneratePipe(string key, Vector2 currentPosition, Vector2 targetPosition)
    {
        obstaclePosition.Add(currentPosition);
        obstacleType.Add("Pipe");

        if (key.Equals("Right"))
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
                if (GetPreviousMove().Equals("Right"))
                {
                    RenderPipe(currentPosition, 0, 0);
                }
                else if (GetPreviousMove().Equals("Down"))
                {
                    RenderPipe(currentPosition, 1, 0);
                }
                else if (GetPreviousMove().Equals("Up"))
                {
                    RenderPipe(currentPosition, 1, 3);
                }
            }
        }
        if (key.Equals("Left"))
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
                if (GetPreviousMove().Equals("Left"))
                {
                    RenderPipe(currentPosition, 0, 0);
                }
                else if (GetPreviousMove().Equals("Down"))
                {
                    RenderPipe(currentPosition, 1, 1);
                }
                else if (GetPreviousMove().Equals("Up"))
                {
                    RenderPipe(currentPosition, 1, 2);
                }
            }
        }
        if (key.Equals("Up"))
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
                if (GetPreviousMove().Equals("Up"))
                {
                    RenderPipe(currentPosition, 0, 1);
                }
                else if (GetPreviousMove().Equals("Left"))
                {
                    RenderPipe(currentPosition, 1, 0);
                }
                else if (GetPreviousMove().Equals("Right"))
                {
                    RenderPipe(currentPosition, 1, 1);
                }
            }
        }
        if (key.Equals("Down"))
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
                if (GetPreviousMove().Equals("Down"))
                {
                    RenderPipe(currentPosition, 0, 1);
                }
                else if (GetPreviousMove().Equals("Left"))
                {
                    RenderPipe(currentPosition, 1, 3);
                }
                else if (GetPreviousMove().Equals("Right"))
                {
                    RenderPipe(currentPosition, 1, 2);
                }
            }
        }
    }

    private void RenderPipe(Vector2 renderPosition, int pipeTypeIndex, int pipeRotationIndex)
    {
        pipeClone = new GameObject();

        pipeClone.AddComponent<SpriteRenderer>();
        pipeClone.GetComponent<SpriteRenderer>().sprite = pipeSprites[pipeTypeIndex];
        pipeClone.GetComponent<Transform>().position = new Vector3(renderPosition.x, renderPosition.y, 1);       
        pipeClone.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[pipeRotationIndex]);

        pipeClone.AddComponent<ChangeColor>();
        pipeClone.GetComponent<ChangeColor>().Start();
        pipeClone.GetComponent<ChangeColor>().ChangeSpriteColor(pipeClone, handlePipeColor);
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
