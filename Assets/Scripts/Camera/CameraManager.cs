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
    [SerializeField] private Canvas chatUI;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitMyCamera(GameObject myObj){
        worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if(myObj != null)
            myCamera = myObj.transform.Find("Camera").GetComponent<Camera>();

        if(myObj != null) Debug.Log("CAMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM-----" + myObj.name);
    }

    public void InitOtherCamera(GameObject otherObj){
        worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if(otherObj != null)
            otherCamera = otherObj.transform.Find("Camera").GetComponent<Camera>();
    }

    public void SetupCamera(string mode){
        /*Mode Space: Change Camera ("Focus to player" or "The whole map")*/
        /*Mode Follow: Hold Camera (focus to other player)*/
        if(mode == "Space" && myCamera != null){
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
        } else if (mode == "Follow" && myCamera != null && otherCamera != null) {
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
        chatUI.worldCamera = currentCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            IsCameraTargetPlayer = !IsCameraTargetPlayer;
            SetupCamera("Space");
        }
        if(Input.GetKeyDown(KeyCode.F)){
            IsCameraTargetOtherPlayer = !IsCameraTargetOtherPlayer;
            SetupCamera("Follow");
        }
    }

    public void SetupSingleplayerCamera(int oldPartionIndex ,int newPartionIndex){
        Camera activeCam = null;
        Camera deActiveCam = null;
        
        activeCam = GameObject.Find("Camera_Single_" + newPartionIndex).GetComponent<Camera>();
        deActiveCam = GameObject.Find("Camera_Single_" + oldPartionIndex).GetComponent<Camera>();

        activeCam.enabled = true;
        deActiveCam.enabled = false;
    }

    public void SetupMultiplayerCamera(int oldPartionIndex ,int newPartionIndex, string mode){
        Camera activeCam = null;
        Camera deActiveCam = null;
        
        if(mode == "Co-op"){
            activeCam = GameObject.Find("Camera_Coop_" + newPartionIndex).GetComponent<Camera>();
            deActiveCam = GameObject.Find("Camera_Coop_" + oldPartionIndex).GetComponent<Camera>();
        } else {
            activeCam = GameObject.Find("Camera_Versus").GetComponent<Camera>();
            deActiveCam = GameObject.Find("Camera_Coop_1").GetComponent<Camera>();
            GameObject.Find("Canvas_Screen").GetComponent<Canvas>().worldCamera = activeCam;
        }

        activeCam.enabled = true;
        deActiveCam.enabled = false;
    }
}