using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{

    [SerializeField] private float moveSteps = 2.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    private bool enableMove = true;
    private bool isNotPickPipe = true;
    private bool isAtPointPosition = false;

    private string handlePipeColor;

    private Vector2 currentPosition = new Vector2(0, 0);
    private Vector2 targetPosition = new Vector2(0, 0);

    [SerializeField] private GameObject player;
    public Sprite[] pipeSprites;
    private List<string> path;
    private List<Vector2> obstaclePosition;
    private List<string> obstacleType;
    private List<Vector2> pointPosition;
    private List<string> pointType;
    private float[] pipeRotation;

    public GameObject respawnPrefab;
    private GameObject[] walls;

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
        handlePipeColor = null;

        pointPosition.Add(new Vector2(-12, 0));
        pointType.Add("Red");
        pointPosition.Add(new Vector2(12, 4));
        pointType.Add("Red");

        walls = GameObject.FindGameObjectsWithTag("Wall");
        for (int i=0; i<walls.Length; i++)
        {
            Vector2 blockPosition = new Vector2(walls[i].GetComponent<Transform>().position.x, walls[i].GetComponent<Transform>().position.y);
            obstaclePosition.Add(blockPosition);
            obstacleType.Add("Wall");
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
                CheckPipeStartPoint(targetPosition);
                // CheckPipeEndPoint(targetPosition);
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
                CheckPipeStartPoint(targetPosition);
                // CheckPipeEndPoint(targetPosition);
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
                CheckPipeStartPoint(targetPosition);
                // CheckPipeEndPoint(targetPosition);
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
                CheckPipeStartPoint(targetPosition);
                // CheckPipeEndPoint(targetPosition);
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
            if (targetPosition == pointPosition[i])
            {
                Debug.Log("Is start point");
                isNotPickPipe = !isNotPickPipe;
                isAtPointPosition = true;
                handlePipeColor = pointType[i];
                Debug.Log("Color: " + handlePipeColor);
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
                Debug.Log("Is end point");
                isNotPickPipe = true;
                isAtPointPosition = true;
                handlePipeColor = null;
                return;
            }
        }
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

    void GeneratePipe(string key, Vector2 currentPosition, Vector2 targetPosition)
    {
        GameObject pipe = new GameObject();
        pipe.name = "PipeClone";
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
            }
            else if(isAtPointPosition && isNotPickPipe){
                Debug.Log("OK VeRY GOOD");
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
            }
        }
        if (key.Equals("Left"))
        {
             if (isAtPointPosition && !isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[2]);
                isAtPointPosition = false;
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
            }
        }
        if (key.Equals("Up"))
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[1]);
                isAtPointPosition = false;
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
            }
        }
        if (key.Equals("Down"))
        {
            if (isAtPointPosition && !isNotPickPipe)
            {
                pipe.GetComponent<SpriteRenderer>().sprite = pipeSprites[2];
                pipe.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[3]);
                isAtPointPosition = false;
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
            }
        }
        pipe.GetComponent<Transform>().position = new Vector3(currentPosition.x, currentPosition.y, 1);
    }

    private string GetPreviousMove()
    {
        return path[path.Count - 1];
    }

    private void SetPreviousMove(string move)
    {
        path.Add(move);
    }

    private bool CanStepToPosition(Vector2 targetMove)
    {
        for (int i = 0; i < obstaclePosition.Count; i++)
        {
            if (obstacleType[i] == "Pipe" && targetMove == obstaclePosition[i])
            {
                if (!isNotPickPipe)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (obstacleType[i] == "Wall" && targetMove == obstaclePosition[i])
            {
                return false;
            }
        }
        return true;
    }
}
