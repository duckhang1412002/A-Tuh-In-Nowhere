using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;
    private float minSwipeDistance = 50f; // Adjust this threshold to your preference
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

        // Check for touch input
        Vector2 moveDirection = Vector2.zero;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Record the start position of the touch
                    touchStartPos = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    // Determine the direction of the swipe
                    touchEndPos = touch.position;
                    Vector2 swipeDirection = touchEndPos - touchStartPos;

                    // Check if the swipe distance exceeds the threshold
                    if (isSwiping && swipeDirection.magnitude > minSwipeDistance)
                    {
                        // Normalize the swipe direction to get a consistent movement speed
                        swipeDirection.Normalize();

                        // Determine the dominant axis of the swipe
                        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                        {
                            // Horizontal swipe
                            if (swipeDirection.x > 0)
                            {
                                moveDirection = Vector2.right;
                            }
                            else
                            {
                                moveDirection = Vector2.left;
                            }
                        }
                        else
                        {
                            if (swipeDirection.y > 0)
                            {
                                moveDirection = Vector2.up;
                            }
                            else
                            {
                                moveDirection = Vector2.down;
                            }
                        }
                        isSwiping = false;
                    }
                    break;

                case TouchPhase.Ended:
                    isSwiping = false;
                    break;
            }
        }

        if ((Input.GetKeyDown(KeyCode.LeftArrow) || moveDirection == Vector2.left) && !isLeft && isMove)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(-moveX, 0f);
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            StartCoroutine(Move());
        }
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || moveDirection == Vector2.right) && !isRight && isMove)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(moveX, 0f);
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            StartCoroutine(Move());
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) || moveDirection == Vector2.up) && !isTop && isMove)
        {
            startPosition = rb.position;
            targetPosition = startPosition + new Vector2(0f, moveY);
            StartCoroutine(Move());
        }
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || moveDirection == Vector2.down) && !isDown && isMove)
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
