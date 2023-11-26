using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SpriteGlow;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Space]
    [SerializeField] GameObject playerPrefabM;
    [SerializeField] GameObject playerPrefabF;
    [SerializeField] private TMP_Text roomName;
    public GameObject PlayerM;
    public GameObject PlayerF;

    private GameObject[,] grid;

    private GameObject[] prefabList;
    private List<string> path;
    public List<string[,]> inputList { get; set; }

    private InputManager inputManager;

    public List<GameObject[,]> MapGridList { get; set; }
    public List<GameObject[,]> PlayGridList { get; set; }

    private Dictionary<int, GameObject> doorButtonList;
    public Dictionary<Vector2, bool> WireMap { get; set; }
    private ChangeColor changeColor;

    public int Score;
    public int SocketAmount = 0;

    private PhotonView view;
    private bool singleMode, versusMode, duoMode, isUpdatedOtherCamera;

    private bool isWinGame;
    private int isMapLoaded;
    [SerializeField]
    private Button restartBtn;
    private PlayerMapAuthentication playerMapAuthentication;

    public UnityEvent downloadCompleteEvent, loadMapCompleteEvent;

    //public UnityEvent everyoneDownloadComplete;

    public void Start()
    {
        Debug.Log("This map from Custom Prop: " + PhotonNetwork.LocalPlayer.CustomProperties["MapID"]);
        Debug.Log("Gender for this player: " + PhotonNetwork.LocalPlayer.CustomProperties["Gender"]);
        playerMapAuthentication = PlayerMapAuthentication.GetInstance();
        isMapLoaded = 0;
        isWinGame = isUpdatedOtherCamera = false;
        view = this.gameObject.GetComponent<PhotonView>();
        inputManager = this.gameObject.GetComponent<InputManager>();
        changeColor = this.gameObject.GetComponent<ChangeColor>();
        WireMap = new Dictionary<Vector2, bool>();
        doorButtonList = new Dictionary<int, GameObject>();
        Score = 0;
        prefabList = FindAllPrefabs();
        singleMode = PhotonNetwork.OfflineMode;
        versusMode = (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") && PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Versus");
        Debug.Log("Welcome to the Game " + PhotonNetwork.LocalPlayer.ActorNumber);


        if (!PhotonNetwork.IsMasterClient && restartBtn != null)
        {
            restartBtn.interactable = false;
        } 
        StartDownloadOnClients();
        Debug.Log("Ping: " + PhotonNetwork.GetPing() + "ms");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //LoadMapInput();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player leftPlayer)
    {
        if (!singleMode && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.Disconnect();
            view.RPC("CallScene", RpcTarget.All, "Loading");
        }
    }

    [PunRPC]
    public void StartDownloadOnClients()
    {
        // Trigger the download on all clients
        CallInitializeMapRPC();
    }

    private void CallInitializeMapRPC()
    {
        /*        downloadCompleteEvent.AddListener(() =>
                {
                    // Place your additional code here
                    Debug.Log("Download completed! Additional code executed.");
                    view.RPC("AddMapLoaded", RpcTarget.All);
                });
                inputManager.DownloadFile(downloadCompleteEvent);*/
        loadMapCompleteEvent.AddListener(() =>
        {
            // Place your additional code here
            Debug.Log("Load map completed!");
            view.RPC("AddMapLoaded", RpcTarget.All);
        });
        inputManager.LoadModelAsync(loadMapCompleteEvent);
    }

    [PunRPC]
    private void AddMapLoaded()
    {
        ++isMapLoaded;
        Debug.Log("Load map cnt : " + isMapLoaded);
        if (isMapLoaded == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("Everyone downloaded the game!");
            //view.RPC("InitializeMapRPC", RpcTarget.All);
            InitializeMapRPC();
        }
    }

    [PunRPC]
    public void InitializeMapRPC()
    {
        Debug.Log("Initializing....!");
        InitializeMap();
        ConnectMap();

    }

    public GameObject InstantiatePrefab(string prefabName, int x, int y)
    {
        GameObject prefab = prefabList.FirstOrDefault(o => o.name == prefabName);
        Quaternion rotation = prefab.transform.rotation;
        float z = prefab.transform.position.z;
        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x, y, z), rotation) as GameObject;
        //GameObject instantiatedPrefab = PhotonNetwork.Instantiate($"Prefabs/{prefabName}", new Vector3(x, y, z), rotation) as GameObject;
        return instantiatedPrefab;
    }

    private GameObject InstantiatePlayerM(int id, int x, int y)
    {
        Quaternion rotation = playerPrefabM.transform.rotation;
        float z = playerPrefabM.transform.position.z;
        int roundedX = Mathf.RoundToInt(x);
        int roundedY = Mathf.RoundToInt(y);
        Vector3 flooredPosition = new Vector3(roundedX, roundedY, z);
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabM.name, flooredPosition, rotation) as GameObject;
        instantiatedPrefab.transform.position = flooredPosition;
        return instantiatedPrefab;
    }
    private GameObject InstantiatePlayerF(int id, int x, int y)
    {
        Quaternion rotation = playerPrefabF.transform.rotation;
        float z = playerPrefabF.transform.position.z;
        int roundedX = Mathf.RoundToInt(x);
        int roundedY = Mathf.RoundToInt(y);
        Vector3 flooredPosition = new Vector3(roundedX, roundedY, z);
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabF.name, flooredPosition, rotation) as GameObject;
        instantiatedPrefab.transform.position = flooredPosition;
        return instantiatedPrefab;
    }

    private void CallCreatePlayerM(int x, int y)
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        PlayerM = InstantiatePlayerM(playerID, x, y);
        GameObject.Find("CameraManager").GetComponent<CameraManager>().InitMyCamera(PlayerM);
        CameraManager.IsCameraTargetPlayer = true;
        CameraManager.IsCameraTargetOtherPlayer = false;
        GameObject.Find("CameraManager").GetComponent<CameraManager>().SetupCamera("Space");
        Debug.Log("Instantiated M");
        PlayerM.GetComponent<Player>().ID = playerID;

        // Synchronize the player object across the network
        if (!singleMode)
        photonView.RPC("SetPlayerM", RpcTarget.OthersBuffered, PlayerM.GetComponent<PhotonView>().ViewID, playerID);
    }

    [PunRPC] 
    private void SetPlayerM(int viewID, int playerID)
    {
       Debug.Log("Need to set player M");
       this.PlayerM = PhotonView.Find(viewID).gameObject;
       PlayerM.GetComponent<Player>().ID = playerID;
    }

    private void CallCreatePlayerF(int x, int y)
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        PlayerF = InstantiatePlayerF(playerID, x, y);
        GameObject.Find("CameraManager").GetComponent<CameraManager>().InitMyCamera(PlayerF);
        CameraManager.IsCameraTargetPlayer = true;
        CameraManager.IsCameraTargetOtherPlayer = false;
        GameObject.Find("CameraManager").GetComponent<CameraManager>().SetupCamera("Space");
        Debug.Log("Instantiated F");
        PlayerF.GetComponent<Player>().ID = playerID;

        // Synchronize the player object across the network
        photonView.RPC("SetPlayerF", RpcTarget.OthersBuffered, PlayerF.GetComponent<PhotonView>().ViewID, playerID);
    }

    [PunRPC]
    private void SetPlayerF(int viewID, int playerID)
    {
        Debug.Log("Need to set player F");
        this.PlayerF = PhotonView.Find(viewID).gameObject;
        PlayerF.GetComponent<Player>().ID = playerID;
    }

    // Function to count the number of walls among adjacent cells
    int CountWalls(params string[] cells)
    {
        return cells.Count(cell => cell.Contains("Wall"));
    }

    bool IsEdgeObject(string obj)
    {
        return (obj.Contains("Wall") || obj.Contains("DimensionOut"));
    }
    private void InitializeMap()
    {
        WireMap = new Dictionary<Vector2, bool>();
        MapGridList = new List<GameObject[,]>();
        int offset = 0;
        int currentMap = 0;
        int playerCount = 1;
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") && PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Versus")
        {
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        }
            
        Debug.Log("P count: " + playerCount);
        foreach (string[,] inputMap in inputList)
        {
            int n = inputMap.GetLength(0);
            int m = inputMap.GetLength(1);
            //Debug.Log(n + " x " + m);
            string[,] randomMap = new string[m, n];

            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    randomMap[i, j] = inputMap[n - j - 1, i];
                }
            }


                //check Wall type
                for (int i = 0; i < m; ++i)
                {
                    for (int j = 0; j < n; ++j)
                    {
                        string l = (i > 0) ? randomMap[i - 1, j] : "Null";
                        string r = (i + 1 < m) ? randomMap[i + 1, j] : "Null";
                        string d = (j > 0) ? randomMap[i, j - 1] : "Null";
                        string u = (j + 1 < n) ? randomMap[i, j + 1] : "Null";

                        if (randomMap[i, j].Contains("Wall"))
                        {
                            if (!IsEdgeObject(l) && !IsEdgeObject(r) && !IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:0:0";
                            else if (IsEdgeObject(l) && !IsEdgeObject(r) && !IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:1:1";
                            else if (!IsEdgeObject(l) && IsEdgeObject(r) && !IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:1:3";
                            else if (!IsEdgeObject(l) && !IsEdgeObject(r) && IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:1:0";
                            else if (IsEdgeObject(l) && !IsEdgeObject(r) && IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:2:0";
                            else if (IsEdgeObject(l) && !IsEdgeObject(r) && !IsEdgeObject(u) && IsEdgeObject(d)) randomMap[i, j] = "Wall:2:1";
                            else if (!IsEdgeObject(l) && IsEdgeObject(r) && IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:2:3";
                            else if (!IsEdgeObject(l) && IsEdgeObject(r) && !IsEdgeObject(u) && IsEdgeObject(d)) randomMap[i, j] = "Wall:2:2";
                            else if (!IsEdgeObject(l) && IsEdgeObject(r) && IsEdgeObject(u) && IsEdgeObject(d)) randomMap[i, j] = "Wall:3:1";
                            else if (IsEdgeObject(l) && !IsEdgeObject(r) && IsEdgeObject(u) && IsEdgeObject(d)) randomMap[i, j] = "Wall:3:3";
                            else if (IsEdgeObject(l) && IsEdgeObject(r) && !IsEdgeObject(u) && IsEdgeObject(d)) randomMap[i, j] = "Wall:3:0";
                            else if (IsEdgeObject(l) && IsEdgeObject(r) && IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:3:2";
                            else if (IsEdgeObject(l) && IsEdgeObject(r) && !IsEdgeObject(u) && !IsEdgeObject(d)) randomMap[i, j] = "Wall:5:1";
                            else randomMap[i, j] = "Wall:5:0";
                        }
                    }
                }
                grid = new GameObject[m, (playerCount-1)*100 + n];
                GameObject ground = prefabList.FirstOrDefault(o => o.name == "Ground");
                float groundZ = ground.transform.position.z;
                Quaternion groundRotate = ground.transform.rotation;
            for (int renderTime = 1; renderTime <= playerCount; ++renderTime)
            {
                for (int xx = 0; xx < m; ++xx)
                {
                    for (int yy = 0; yy < n; ++yy)
                    {
                        string item = randomMap[xx, yy];
                        int x = xx;
                        int y = yy + (renderTime - 1) * 100;
                        if (item.Contains("Null"))
                        {
                            grid[x, y] = null;
                            continue;
                        }
                        //Init ground
                        if (!item.Contains("Wall"))
                        {
                            //GameObject groundObject = Instantiate(ground, new Vector3(x + offset, y, groundZ), groundRotate);
                            GameObject groundObject = InstantiatePrefab("Ground", x + offset, y);
                            grid[x, y] = groundObject;
                        }
                        GameObject prefab;

                        if (item.Contains("Socket"))
                        {
                            string hexCode = item.Split("_")[1];
                            item = "Socket";

                            //change color
                            GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                            instantiatedPrefab.GetComponent<Socket>().Color = hexCode;

                            changeColor.ChangeSpriteColor(instantiatedPrefab.transform.Find("Inner").gameObject, hexCode);
                            instantiatedPrefab.transform.Find("Inner").gameObject.GetComponent<SpriteGlowEffect>().GlowColor = instantiatedPrefab.transform.Find("Inner").gameObject.GetComponent<ChangeColor>().GetColor(hexCode);

                            grid[x, y] = instantiatedPrefab;
                            SocketAmount++;
                        }
                        else if (item.Contains("PlayerM"))
                        {
                            int id = int.Parse(item.Split(':')[1]);
                            if (versusMode)
                            {
                                if (renderTime == PhotonNetwork.LocalPlayer.ActorNumber)
                                {
                                    if (PhotonNetwork.LocalPlayer.CustomProperties["Gender"].ToString() == "M") CallCreatePlayerM(x + offset, y);
                                    else CallCreatePlayerF(x + offset, y);
                                }
                            }
                            else if (singleMode || PhotonNetwork.LocalPlayer.CustomProperties["Gender"].ToString() == "M")
                            {
                                CallCreatePlayerM(x + offset, y);
                            }
                        }
                        else if (item.Contains("PlayerF"))
                        {
                            //int id = int.Parse(item.Split(':')[1]);
                            /*view.RPC("SetPlayerF", RpcTarget.AllBuffered, id, x + offset, y);*/
                            if (PhotonNetwork.LocalPlayer.CustomProperties["Gender"].ToString() == "F")
                            {
                                CallCreatePlayerF(x + offset, y);
                            }
                        }
                        else if (item.Contains("Bridge"))
                        {
                            string direction = item.Split('_')[0];
                            item = "Bridge";

                            GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                            if (direction == "H")
                            {
                                instantiatedPrefab.GetComponent<Bridge>().Direction = "Horizontal";
                            }
                            else
                            {
                                instantiatedPrefab.GetComponent<Bridge>().Direction = "Vertical";
                                instantiatedPrefab.GetComponent<Bridge>().RenderSprite();
                            }

                            grid[x, y] = instantiatedPrefab;
                        }
                        else if (item.Contains("Dimension"))
                        {
                            string[] dimension = item.Split(':');
                            string dimensionWay = dimension[0];

                            GameObject instantiatedPrefab = InstantiatePrefab(dimensionWay, x + offset, y);
                            if (dimensionWay == "DimensionIn")
                            {
                                int dimensionID = int.Parse(dimension[1]);
                                dimensionID += (renderTime - 1) * 100; //for versus mode
                                instantiatedPrefab.GetComponent<DimensionIn>().ID = dimensionID;
                            }
                            else
                            {
                                instantiatedPrefab.GetComponent<DimensionOut>().OutDirection = dimension[1];
                                instantiatedPrefab.GetComponent<DimensionOut>().RenderSprite(dimension[1]);
                            }
                            grid[x, y] = instantiatedPrefab;
                        }
                        else if (item.Contains("DoorButton"))
                        {
                            int buttonID = int.Parse(item.Split(':')[1]);
                            buttonID += (renderTime - 1) * 100; //for versus mode
                            item = "DoorButton";
                            GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                            instantiatedPrefab.GetComponent<DoorButton>().Start();
                            instantiatedPrefab.GetComponent<DoorButton>().ID = buttonID;
                            grid[x, y] = instantiatedPrefab;
                            doorButtonList[buttonID] = instantiatedPrefab;
                        }
                        else if (item.Contains("Door"))
                        {
                            string direction = item.Split('_')[0];
                            int doorID = int.Parse(item.Split(':')[1]);
                            doorID += (renderTime - 1) * 100; //for versus mode
                            GameObject instantiatedPrefab = InstantiatePrefab("Door", x + offset, y);
                            instantiatedPrefab.GetComponent<Door>().ID = doorID;
                            instantiatedPrefab.GetComponent<Door>().DoorOpenDirection = item.Split(':')[2];

                            if (direction == "H") //only need to rotate if Horizontal
                            {
                                //rotate 90 deg
                                instantiatedPrefab.transform.Rotate(0f, 0f, 90f);
                            }

                            instantiatedPrefab.GetComponent<Door>().isReverseDoor = item.Contains("Reverse");

                            instantiatedPrefab.GetComponent<Door>().Init();
                            grid[x, y] = instantiatedPrefab;
                        }
                        else if (item.Contains("Wall"))
                        {
                            GameObject instantiatedPrefab = InstantiatePrefab("Wall", x + offset, y);
                            if (item != "Wall")
                            {
                                int spriteIndex = int.Parse(item.Split(':')[1]);
                                int rotationIndex = int.Parse(item.Split(':')[2]);
                                instantiatedPrefab.GetComponent<Wall>().RenderSprite(spriteIndex, rotationIndex);
                            } //have more attribute

                            grid[x, y] = instantiatedPrefab;
                        }
                        // else if (item.Contains("EscButton"))
                        // {
                        //     item = "EscButton";

                        //     GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        //     instantiatedPrefab.GetComponent<EscButton>().Start();

                        //     btn=instantiatedPrefab;

                        //     grid[x, y] = instantiatedPrefab;
                        // }
                        // else if (item.Contains("Escalator"))
                        // {
                        //     string direction = item.Split(':')[1];
                        //     item = "Escalator";

                        //     GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);

                        //     if(direction == "U"){
                        //         instantiatedPrefab.GetComponent<Escalator>().Direction = "Up";
                        //     } else if(direction == "D"){
                        //         instantiatedPrefab.GetComponent<Escalator>().Direction = "Down";
                        //     } else if(direction == "L"){
                        //         instantiatedPrefab.GetComponent<Escalator>().Direction = "Left";
                        //     } else if(direction == "R"){
                        //         instantiatedPrefab.GetComponent<Escalator>().Direction = "Right";
                        //     }

                        //     instantiatedPrefab.GetComponent<Escalator>().Start();
                        //     instantiatedPrefab.GetComponent<Escalator>().RenderSprite();

                        //     esc=instantiatedPrefab;
                        //     esc.GetComponent<Escalator>().button = btn.GetComponent<EscButton>();

                        //     grid[x, y] = instantiatedPrefab;
                        // }
                        else if (!item.Contains("Ground"))
                        {
                            //Have Ice here
                            GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                            grid[x, y] = instantiatedPrefab;
                        }
                    }
                }
            }
            MapGridList.Add(grid);
            offset += 100;
            ++currentMap;
        }
    }

    private void ConnectMap()
    {
        for (int i = 0; i < MapGridList.Count; ++i)
        {
            foreach (GameObject item in MapGridList[i])
            {
                if (item == null) continue;
                //Connect Dimension In and Out
                if (item.tag == "DimensionIn")
                {
                    int factor = item.GetComponent<DimensionIn>().ID / 100; 
                    int dimensionID = item.GetComponent<DimensionIn>().ID % 100;
                    foreach (GameObject insideItem in MapGridList[dimensionID])
                    {
                        int ok = 0;
                        if (insideItem == null) continue;
                        if (insideItem.tag == "DimensionOut")
                        {
                            int yFactor = (int)insideItem.transform.position.y / 100;
                            if (yFactor != factor) continue;
                            string dir = insideItem.GetComponent<DimensionOut>().OutDirection;
                            if (dir == "Left")
                            {
                                item.GetComponent<DimensionIn>().exitLeft = insideItem;
                            }
                            else if (dir == "Top")
                            {
                                item.GetComponent<DimensionIn>().exitTop = insideItem;
                            }
                            else if (dir == "Bottom")
                            {
                                item.GetComponent<DimensionIn>().exitBottom = insideItem;
                            }
                            else
                            {
                                item.GetComponent<DimensionIn>().exitRight = insideItem;
                            }
                            insideItem.GetComponent<DimensionOut>().BaseDimension = item.GetComponent<DimensionIn>();
                        }
                    }
                    item.GetComponent<DimensionIn>().Start();
                    item.GetComponent<DimensionIn>().RenderSprite();
                }


                //Connect Button & Door
                else if (item.tag == "Door")
                {
                    int doorID = item.GetComponent<Door>().ID;
                    int factor = doorID / 100;
                    if (item.GetComponent<Door>().isReverseDoor)
                    {
                        //item.GetComponent<Door>().DoorTransition();
                        item.GetComponent<Door>().DebugPosition();
                    }
                    foreach (int btnID in inputManager.ListDoor[doorID%100])
                    {
                        item.GetComponent<Door>().Button = doorButtonList[btnID+(factor * 100)].GetComponent<DoorButton>();
                    }
                }
            }
        }
        PlayGridList = MapGridList;
        photonView.RPC("EnableMove", RpcTarget.All);
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") && PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Versus")
        {
            PhotonNetwork.LocalPlayer.CustomProperties["Score"] = 0; //set score for VS mode
            SocketAmount /= PhotonNetwork.CurrentRoom.PlayerCount;
        }

        Debug.Log("Socket Amount: " + SocketAmount);
        /*        if (PlayerM != null)
                {
                    //PlayerM.GetComponent<Step>().enabled = true;
                    PlayerM.GetComponent<MoveController>().enabled = true;
                }
                else
                {
                    //PlayerF.GetComponent<Step>().enabled = true;
                    PlayerF.GetComponent<MoveController>().enabled = true;
                }*/
    }

    [PunRPC]
    private void EnableMove()
    {
        PlayGridList = MapGridList;
        if (PlayerM != null)
        {
            PlayerM.GetComponent<MoveController>().enabled = true;
        }
        else
        {
            PlayerF.GetComponent<MoveController>().enabled = true;
        }
    }

    public List<string> GetPath() { return path; }
    public GameObject GetObjectFromGrid(int x, int y)
    {
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        return null;
    }

    public List<GameObject[,]> GetMapGridList()
    {
        return MapGridList;
    }

    public GameObject GetPlayer()
    {
        for (int i = 0; i < MapGridList.Count; ++i)
        {
            foreach (GameObject item in MapGridList[i])
            {
                if (item == null) continue;
                if (item.tag == "Player") return item;
            }
        }
        return null;
    }

    public void ReloadScene()
    {
        PhotonView view = this.gameObject.GetComponent<PhotonView>();
        if (versusMode)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("Surrender", RpcTarget.Others);
        }
        else 
            view.RPC("ResetTheGame", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {
        if (isWinGame) return;
        //GameOverUI.SetActive(false);
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") && PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Versus")
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Score"))
            {
                int playerScore = int.Parse(PhotonNetwork.LocalPlayer.CustomProperties["Score"].ToString());
                if (playerScore == SocketAmount / 2 && SocketAmount != 0)
                {
                    Debug.Log("Player win: " + PhotonNetwork.LocalPlayer.ActorNumber + " prepare to call RPC");
                    int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                    string winPlayerName = playerMapAuthentication.currentAccount.Fullname;
                    string gender = PhotonNetwork.LocalPlayer.CustomProperties["Gender"].ToString();
                    int viewID = (gender == "M") ? PlayerM.GetComponent<PhotonView>().ViewID : PlayerF.GetComponent<PhotonView>().ViewID;
                    int stepCount = PhotonView.Find(viewID).gameObject.GetComponent<MoveController>().stepCount;
                    photonView.RPC("CallWinGameVSMode", RpcTarget.All, actorNumber, gender, winPlayerName, stepCount);
                    
                }
            }

        }
        if (Score == SocketAmount / 2 && SocketAmount != 0 && !isWinGame)
        {
            isWinGame = true;
            GameObject.Find("PlayerMapController").GetComponent<PlayerMapController>().UpdatePlayerMap();
            GameObject.Find("UIManager").GetComponent<UIManager>().SetupResultUI(PlayerMapController.CurrentGameMode, PlayerMapController.MapID, PlayerMapController.RestartNumber);         
        }

        if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") && PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Co-op" && PlayerM != null && PlayerF != null && !isUpdatedOtherCamera){
            isUpdatedOtherCamera = true;

            if(PlayerM.GetComponent<MoveController>().enabled && PlayerF.GetComponent<MoveController>().enabled){
                GameObject.Find("CameraManager").GetComponent<CameraManager>().InitOtherCamera(PlayerM);
                GameObject.Find("CameraManager").GetComponent<CameraManager>().SetupCamera("Space");
            } else {
                GameObject.Find("CameraManager").GetComponent<CameraManager>().InitOtherCamera(PlayerF);
                GameObject.Find("CameraManager").GetComponent<CameraManager>().SetupCamera("Space"); 
            }
        }
    }

    public void BackToLobbyScene(){
        /*Kick out the lobby - comment*/
        if (singleMode)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("SingleLobby");
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel("MultiplayerLobby");
            /*                view.RPC("CallScene", RpcTarget.All, "Loading");
                            // Clear custom properties before leaving the room
                            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
                            customProps.Clear();
                            PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);
                            PhotonNetwork.LeaveRoom();*/
        }
    }

    [PunRPC]
    private void CallScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    [PunRPC]
    private void CallLeaveRoom()
    {
        // Clear custom properties before leaving the room
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps.Clear();
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);

        //PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    private void Surrender()
    {
        Debug.Log("The opponent surrenderd! - prepare to call RPC");
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string winPlayerName = playerMapAuthentication.currentAccount.Fullname;
        string gender = PhotonNetwork.LocalPlayer.CustomProperties["Gender"].ToString();
        int viewID = (gender == "M") ? PlayerM.GetComponent<PhotonView>().ViewID : PlayerF.GetComponent<PhotonView>().ViewID;
        photonView.RPC("CallWinGameVSMode", RpcTarget.All, actorNumber, gender, winPlayerName, -1);
    }

    [PunRPC]
    private void CallWinGameVSMode(int actorNumber, string gender, string playerName, int stepCount)
    {
        isWinGame = true;
        //GameObject.Find("PlayerMapController").GetComponent<PlayerMapController>().UpdatePlayerMap();
        GameObject.Find("UIManager").GetComponent<UIManager>().SetupVSResultUI(playerName, PhotonNetwork.LocalPlayer.CustomProperties["MapID"].ToString(), stepCount, gender);
        Debug.Log($"Player {actorNumber} win the game!!");
/*        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("MultiplayerLobby");*/
    }

    //only master client
    [PunRPC]
    private void ResetTheGame()
    {
        //SceneManager.LoadSceneAsync("Game");
       PhotonNetwork.LoadLevel("Game");
    }

    public GameObject[,] GetGrid()
    {
        return grid;
    }

    private static GameObject[] FindAllPrefabs()
    {
        string prefabFolder = "Prefabs";
        GameObject[] prefabList = Resources.LoadAll<GameObject>(prefabFolder);
        return prefabList;
    }

    public GameObject GetPrefabByName(string prefabName)
    {
        GameObject[] prefabList = FindAllPrefabs();

        foreach (GameObject prefab in prefabList)
        {
            if (prefab.name == prefabName) return prefab;
        }
        return null;
    }
}