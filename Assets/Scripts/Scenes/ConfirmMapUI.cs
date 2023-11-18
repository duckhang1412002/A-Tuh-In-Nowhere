using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;


public class ConfirmMapUI : MonoBehaviour {
    [SerializeField] private GameObject btn_Play;
    [SerializeField] private GameObject btn_Next;
    [SerializeField] private GameObject btn_Prev;

    public void SetupNavigateButton(){
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
            Debug.Log("The map block with ID " + PlayerMapController.MapID + " not found in DB!");
        }
    }

    public async Task ConfirmMapUISetup(MapProjector map){
        if(!map.IsUnlocked){
            btn_Play.GetComponent<Button>().interactable = false;
        } else {
            btn_Play.GetComponent<Button>().interactable = true;
        }

        this.gameObject.SetActive(true);
        await ShowMapInfo(map);
    }

    public async void ClickNextButton(){
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
        if(PlayerMapController.MapID <= 1){
            Debug.Log("---------------DISABLE SWIPE LEFT BUTTON----------------");
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
        RawImage imageComponent = this.gameObject.transform.Find("Board/Mask/Map Image").GetComponent<RawImage>();
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
    }
}