using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject GameOverUI;

    [SerializeField]
    private GameObject PauseUI;
    
    [SerializeField]
    private GameObject GuideUI;

    private List<string> path;
    private Dictionary<Vector2, string> obstaclePosition;
    private Dictionary<Vector2, PipePoint> pointType;
    private Dictionary<Vector2, Bridge> bridgeType;
    private Dictionary<Vector2, Dimension> dimensionType;
    private Dictionary<Vector2, DimensionTeleporter> dimensionTeleporterType;
    private Dictionary<Vector2, DoorButton> doorButtonType;
    private Dictionary<Vector2, Door> doorType;
    private Dictionary<Vector2, WaterPool> poolType;

    private GameObject[] walls;
    private GameObject[] pipePoints;
    private GameObject[] bridges;
    private GameObject[] dimensions;
    private GameObject[] dimensionTeleporters;
    private GameObject[] doors;
    private GameObject[] doorButtons;
    private GameObject[] pools;

    private bool openPauseUI = false;

    private bool openGuideUI = false;

    public int Score{get; set;}

    public void Start()
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 6);
        Score = 0;

        obstaclePosition = new Dictionary<Vector2, string>();
        pointType = new Dictionary<Vector2, PipePoint>();
        bridgeType = new Dictionary<Vector2, Bridge>();
        dimensionType = new Dictionary<Vector2, Dimension>();
        dimensionTeleporterType = new Dictionary<Vector2, DimensionTeleporter>();
        doorButtonType = new Dictionary<Vector2, DoorButton>();
        doorType = new Dictionary<Vector2, Door>();
        poolType = new Dictionary<Vector2, WaterPool>();
        path = new List<string>();
        path.Add("");

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
            pointType[blockPosition] = item.GetComponent<PipePoint>();
        }

        bridges = GameObject.FindGameObjectsWithTag("Bridge");
        foreach (GameObject item in bridges)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "Bridge";
            bridgeType[blockPosition] = item.GetComponent<Bridge>();
        }

        dimensions = GameObject.FindGameObjectsWithTag("Dimension");
        foreach (GameObject item in dimensions)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "Dimension";
            dimensionType[blockPosition] = item.GetComponent<Dimension>();
        }

        dimensionTeleporters = GameObject.FindGameObjectsWithTag("DimensionTeleporter");
        foreach (GameObject item in dimensionTeleporters)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "DimensionTeleporter";
            dimensionTeleporterType[blockPosition] = item.GetComponent<DimensionTeleporter>();
        }

        doorButtons = GameObject.FindGameObjectsWithTag("DoorButton");
        foreach (GameObject item in doorButtons)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "DoorButton";
            doorButtonType[blockPosition] = item.GetComponent<DoorButton>();
        }

        doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject item in doors)
        {
            Door door = item.GetComponent<Door>();
            door.Start();
            Vector2 blockPosition;
            if(door.CheckReverseDoor() == true){
                blockPosition = new Vector2(door.GetReverseDoorLocation().x, door.GetReverseDoorLocation().y);
            }
            else{
                blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            }
            obstaclePosition[blockPosition] = "Door";
            doorType[blockPosition] = item.GetComponent<Door>();
        }

        pools = GameObject.FindGameObjectsWithTag("Pool");
        foreach (GameObject item in pools)
        {
            Vector2 blockPosition = new Vector2(item.GetComponent<Transform>().position.x, item.GetComponent<Transform>().position.y);
            obstaclePosition[blockPosition] = "Pool";
            poolType[blockPosition] = item.GetComponent<WaterPool>();
        }
    }

    public List<string> GetPath(){return path;}
    public Dictionary<Vector2, string> GetObstaclePosition(){return obstaclePosition;}
    public Dictionary<Vector2, PipePoint> GetPointType(){return pointType;}
    public Dictionary<Vector2, Bridge> GetBridgeType(){return bridgeType;}
    public Dictionary<Vector2, Dimension> GetDimensionType(){return dimensionType;}
    public Dictionary<Vector2, DimensionTeleporter> GetDimensionTeleporterType(){return dimensionTeleporterType;}
    public Dictionary<Vector2, DoorButton> GetDoorButtonType(){return doorButtonType;}
    public Dictionary<Vector2, Door> GetDoorType(){return doorType;}
    public Dictionary<Vector2, WaterPool> GetPoolType(){return poolType;}
    public GameObject GetPlayer(){return player;}

    // Update is called once per frame
    void Update()
    {
        GameOverUI.SetActive(false);
        if(Input.GetKeyDown(KeyCode.R)){
            ResetTheGame();
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            openPauseUI = !openPauseUI;
            if(openPauseUI){
                PauseUI.SetActive(true);
            } else{
                PauseUI.SetActive(false);
            }
        }
                
        if(Input.GetKeyDown(KeyCode.G)){
            openGuideUI = !openGuideUI;
            if(openGuideUI){
                GuideUI.SetActive(true);
            } else{
                GuideUI.SetActive(false);
            }
        }
        if(Score == pointType.Count/2){
            GameOverUI.SetActive(true);
        }
    }

    private void ResetTheGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
