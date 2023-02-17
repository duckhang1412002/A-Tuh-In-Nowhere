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
    private Dictionary<Vector2, string> pointPosition;
    private static float[] pipeRotation = { 0f, 90.0f, 180.0f, 270.0f };
    private GameObject[] walls;
    private GameObject[] pipePoints;
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = player.transform.position;
        targetPosition = player.transform.position;
        obstaclePosition = new Dictionary<Vector2, string>();
        pointPosition = new Dictionary<Vector2, string>();
        path = new List<string>();
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
            pointPosition[blockPosition] = item.GetComponent<PipePoint>().GetColorType();
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
        if (pointPosition.ContainsKey(targetPosition) && !pointPosition[targetPosition].Equals("Done"))
        {
            isNotPickPipe = false;
            isAtPointPosition = true;
            handlePipeColor = pointPosition[targetPosition];
            positionOfStartPoint = targetPosition;
            Debug.Log("Is start point --- " + handlePipeColor);

            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, handlePipeColor);
        }
    }

    void CheckPipeEndPoint(Vector2 targetPosition)
    {
        if (pointPosition.ContainsKey(targetPosition) && handlePipeColor.Equals(pointPosition[targetPosition]))
        {
            isNotPickPipe = true;
            isAtPointPosition = true;                     
            pointPosition[targetPosition] = "Done";
            pointPosition[positionOfStartPoint] = "Done";
            Debug.Log("Is end point --- " + handlePipeColor);

            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, "Default");
        }
    }

    void GeneratePipe(string key, Vector2 currentPosition, Vector2 targetPosition)
    {
        obstaclePosition[currentPosition] = "Pipe";

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
        GameObject pipeClone = new GameObject();

        pipeClone.AddComponent<SpriteRenderer>();
        pipeClone.GetComponent<SpriteRenderer>().sprite = pipeSprites[pipeTypeIndex];
        pipeClone.GetComponent<Transform>().position = renderPosition;
        pipeClone.GetComponent<Transform>().Rotate(0f, 0f, pipeRotation[pipeRotationIndex]);

        pipeClone.AddComponent<ChangeColor>();
        pipeClone.GetComponent<ChangeColor>().Start();
        pipeClone.GetComponent<ChangeColor>().ChangeSpriteColor(pipeClone, handlePipeColor);             
    }

    private bool CanStepToPosition(Vector2 targetMove)
    {
        if (obstaclePosition.ContainsKey(targetMove) && obstaclePosition[targetMove].Equals("Pipe"))
        {
            if (!isNotPickPipe) return false;
        }
        else if (obstaclePosition.ContainsKey(targetMove) && obstaclePosition[targetMove].Equals("Wall"))
        {
            return false;
        }
        else if (obstaclePosition.ContainsKey(targetMove) && obstaclePosition[targetMove].Equals("PipePoint"))
        {
            if (pointPosition.ContainsKey(targetMove) && !handlePipeColor.Equals(pointPosition[targetMove]) && !isNotPickPipe)
                return false;
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
}
