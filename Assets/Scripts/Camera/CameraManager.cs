using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera currentCamera;
    private Camera worldCamera;
    private Camera myCamera;
    private Camera otherCamera;

    public bool IsCameraTargetPlayer{get; set;}
    public bool IsCameraTargetOtherPlayer{get; set;}

    [Space]
    [Header("Canvas")]
    [SerializeField] private Canvas pauseUI;
    [SerializeField] private Canvas resultUI;
    [SerializeField] private Canvas mainUI;


    // Start is called before the first frame update
    void Start()
    {
        IsCameraTargetPlayer = false;
        IsCameraTargetOtherPlayer = false;

        worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        currentCamera = worldCamera;
        
        mainUI.worldCamera = currentCamera;
        pauseUI.worldCamera = currentCamera;
        resultUI.worldCamera = currentCamera;
    }

    public void InitCamera(GameObject myObj, GameObject otherObj){
        if(myObj != null)
            myCamera = myObj.transform.Find("Camera").GetComponent<Camera>();

        if(otherObj != null)
            otherCamera = otherObj.transform.Find("Camera").GetComponent<Camera>();
    }

    public void SetupCamera(string mode){
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
        } else if (mode == "S" && myCamera != null && otherCamera != null) {
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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            IsCameraTargetPlayer = !IsCameraTargetPlayer;
            SetupCamera("C");
        }
        if(Input.GetKeyDown(KeyCode.S)){
            IsCameraTargetOtherPlayer = !IsCameraTargetOtherPlayer;
            SetupCamera("S");
        }
    }
}
