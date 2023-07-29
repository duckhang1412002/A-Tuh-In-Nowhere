using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMove : MonoBehaviour
{

    public int moveX = 1;
    public int moveY = 1;
    public int moveSpeed = 5;
    public bool isMove = true;

    [SerializeField] Transform checkTop;
    [SerializeField] Transform checkFront;
    [SerializeField] Transform checkDown;

    [SerializeField] LayerMask brickLayer;
    [SerializeField]
    private bool isTop;
    [SerializeField]
    private bool isLeft;
    [SerializeField]
    private bool isRight;
    [SerializeField]
    private bool isDown;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        CheckTop();
        CheckFront();
        CheckDown();
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !isLeft && isMove)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(-moveX, 0f);
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            StartCoroutine(Move());
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !isRight && isMove)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(moveX, 0f);
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            StartCoroutine(Move());
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && !isTop && isMove)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(0f, moveY);
            StartCoroutine(Move());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !isDown && isMove)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(0f, -moveY);
            StartCoroutine(Move());
        }

    }
    private IEnumerator Move()
{
    isMove = false;
    float distanceToMove = Vector2.Distance(startPosition, targetPosition);
    float moveTime = distanceToMove / moveSpeed;
    float elapsedTime = 0f;

    while (elapsedTime < moveTime)
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, targetPosition, moveSpeed*Time.deltaTime);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    rb.position = targetPosition;
    isMove = true;
}

    void CheckTop()
    {
        isTop = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkTop.position, 0.2f, brickLayer);
        if (colliders.Length > 0)
            isTop = true;
    }

    void CheckFront()
    {
        isRight = false;
        isLeft = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkFront.position, 0.2f, brickLayer);
        if (colliders.Length > 0)
            if (transform.localScale.x == -0.5f)
            {
                isLeft = true;
                isRight = false;
            }
            else
            {
                isLeft = false;
                isRight = true;
            }

    }
    void CheckDown()
    {
        isDown = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkDown.position, 0.2f, brickLayer);
        if (colliders.Length > 0)
            isDown = true;
    }
    public void LoadSceneByName(string sceneName)
    {
        InputManager.fileName = sceneName + ".txt";
        if (sceneName == "Loading" || sceneName == "Map")
        {
            if (sceneName == "Map")
            {
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.CreateRoom("single", new RoomOptions(), TypedLobby.Default);
            }
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene("Game");
        }

    }
}
