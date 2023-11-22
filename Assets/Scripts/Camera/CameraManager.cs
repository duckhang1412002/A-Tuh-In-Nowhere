using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera currentCamera;
    private Camera worldCamera;
    private Camera myCamera;
    private Camera otherCamera;

    public static bool IsCameraTargetPlayer{get; set;}
    public static bool IsCameraTargetOtherPlayer{get; set;}

    [Space]
    [Header("Canvas")]
    [SerializeField] private Canvas pauseUI;
    [SerializeField] private Canvas resultUI;
    [SerializeField] private Canvas vsResultUI;
    [SerializeField] private Canvas mainUI;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitMyCamera(GameObject myObj){
        worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if(myObj != null)
            myCamera = myObj.transform.Find("Camera").GetComponent<Camera>();
    }

    public void InitOtherCamera(GameObject otherObj){
        worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if(otherObj != null)
            otherCamera = otherObj.transform.Find("Camera").GetComponent<Camera>();
    }

    public void SetupCamera(string mode){
        /*Mode C: Change Camera ("Focus to player" or "The whole map")*/
        /*Mode S: Switch Camera (Switch to other player)*/
        if(mode == "C" && myCamera != null){
            if (IsCameraTargetPlayer)
            {
                currentCamera = myCamera;

                worldCamera.enabled = false;
                myCamera.enabled = true;
                if(otherCamera != null) otherCamera.enabled = false;
            }
            else
            {
                currentCamera = worldCamera;

                worldCamera.enabled = true;
                myCamera.enabled = false;
                if(otherCamera != null) otherCamera.enabled = false;
            }
        } else if (mode == "X" && myCamera != null && otherCamera != null) {
            if (IsCameraTargetOtherPlayer)
            {
                currentCamera = otherCamera;

                worldCamera.enabled = false;
                myCamera.enabled = false;
                otherCamera.enabled = true;
            }
            else
            {
                currentCamera = myCamera;

                worldCamera.enabled = false;
                myCamera.enabled = true;
                otherCamera.enabled = false;
            }
        }
        mainUI.worldCamera = currentCamera;
        pauseUI.worldCamera = currentCamera;
        resultUI.worldCamera = currentCamera;
        vsResultUI.worldCamera = currentCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            IsCameraTargetPlayer = !IsCameraTargetPlayer;
            SetupCamera("C");
        }
        if(Input.GetKeyDown(KeyCode.X)){
            IsCameraTargetOtherPlayer = !IsCameraTargetOtherPlayer;
            SetupCamera("X");
        }
    }
}