using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;
using TMPro;


public class ConfirmMapUI : MonoBehaviour {
    [SerializeField] private Sprite defautThumbnail;
    [SerializeField] private GameObject btn_Play;
    [SerializeField] private GameObject btn_Next;
    [SerializeField] private GameObject btn_Prev;

    [SerializeField] private TextMeshProUGUI txt_MapName;
    [SerializeField] private TextMeshProUGUI txt_RestartNumber;
    [SerializeField] private TextMeshProUGUI txt_MapStatus;

    public void SetupNavigateButton(){
        btn_Next.gameObject.SetActive(false);
        btn_Prev.gameObject.SetActive(false);
        return;

        btn_Next.GetComponent<Button>().interactable = true;
        btn_Prev.GetComponent<Button>().interactable = true;

        try{
            if(PlayerMapController.MapID >= PlayerMapController.ProjectorList[PlayerMapController.ProjectorList.Count-1].ProjectorID){
                btn_Next.GetComponent<Button>().interactable = false;
            }
            if (PlayerMapController.MapID <= 1){
                btn_Prev.GetComponent<Button>().interactable = false;
            }
        } catch(Exception e){
            Debug.Log("The projector with ID " + PlayerMapController.MapID + " not found in DB!");
        }
    }

    public async Task ConfirmMapUISetup(MapProjector map){
        if(!map.IsUnlocked){
            btn_Play.GetComponent<Button>().interactable = false;
        } else {
            if (PhotonNetwork.IsMasterClient || SceneManager.GetActiveScene().name == "SingleLobby"){
                btn_Play.GetComponent<Button>().interactable = true;          
                btn_Play.GetComponentInChildren<TextMeshProUGUI>().text = "START";
            }
        }

        this.gameObject.SetActive(true);
        await ShowMapInfo(map);
    }

    public async void ClickNextButton(){
        //---------------DISABLE SWIPE RIGHT BUTTON----------------
        if(PlayerMapController.MapID >= PlayerMapController.ProjectorList[PlayerMapController.ProjectorList.Count-1].ProjectorID){
            SetupNavigateButton();
            return;
        }
        ++PlayerMapController.MapID;
        SetupNavigateButton();

        foreach(MapProjector m in PlayerMapController.ProjectorList){
            if(m.ProjectorID == PlayerMapController.MapID){
                await ConfirmMapUISetup(m);
                return;
            }
        }
    }

    public async void ClickPrevButton(){
        //---------------DISABLE SWIPE LEFT BUTTON----------------
        if(PlayerMapController.MapID <= 1){

            SetupNavigateButton();
            return;
        }
        --PlayerMapController.MapID;
        SetupNavigateButton();

        foreach(MapProjector m in PlayerMapController.ProjectorList){
            if(m.ProjectorID == PlayerMapController.MapID){
                await ConfirmMapUISetup(m);
                return;
            }
        }
    }

    private async Task ShowMapInfo(MapProjector map)
    {
        if(!map.gameObject.name.Contains("GameObj_MapBlock_VSMap")){
            txt_MapName.text = map.MapInfo.MapName;
            txt_MapStatus.text = "CONNECTED!";
            txt_MapStatus.color = Color.green;

            if(!map.IsSolved){
                txt_MapStatus.text = "DISCONNECTED!";
                txt_MapStatus.color = Color.red;
            }
            if(!map.IsUnlocked){
                string prevMapID = map.GetPreviousMapProjectorID()[0].ToString();
                for(int i=1; i<map.GetPreviousMapProjectorID().Length; i++){
                    prevMapID += ", " + map.GetPreviousMapProjectorID()[i];
                }
                txt_RestartNumber.text = "You need to complete level " + prevMapID + " before challenge the map!";
            }
            else{
                txt_RestartNumber.text = "";
            }

            //Setup map screenshot
            RawImage imageComponent = this.gameObject.transform.Find("Board/Frame/Mask/Map Image").GetComponent<RawImage>();
            if (imageComponent != null)
            {
                string imagePath = $"{Application.persistentDataPath}/Thumbs/{map.MapInfo.MapID}.png";

                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D(1, 1);
                    if (texture.LoadImage(imageBytes))
                    {
                        imageComponent.texture = texture;
                    }
                }
                else
                {
                    Debug.LogError("Image file not found: " + imagePath);
                }
            }
        } else {
            btn_Next.GetComponent<Button>().interactable = false;
            btn_Prev.GetComponent<Button>().interactable = false;  

            RawImage imageComponent = this.gameObject.transform.Find("Board/Frame/Mask/Map Image").GetComponent<RawImage>();
            imageComponent.texture = defautThumbnail.texture;

            txt_MapName.text = "Versus Mode Custom Map";
            txt_MapStatus.text = "";
            txt_RestartNumber.text = "";
        }
    }
}