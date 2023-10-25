using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ConfirmMapUI : MonoBehaviour {
    public async Task ConfirmMapUISetup(bool isActive, MapBlock map){
        GameObject btn_Play = this.gameObject.transform.Find("Board").Find("Btn_Play").gameObject;

        if(!isActive){
            btn_Play.GetComponent<Button>().interactable = false;
        } else {
            btn_Play.GetComponent<Button>().interactable = true;
        }

        this.gameObject.SetActive(true);
        await ShowMapInfo(map);  
    }

    private async Task ShowMapInfo(MapBlock map)
    {
        RawImage imageComponent = this.gameObject.transform.Find("Board/Mask/Map Image").GetComponent<RawImage>();
        if (imageComponent != null)
        {
            string imagePath = $"{Application.persistentDataPath}/Thumbs/{map.MapID}.png";

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

        // playBtn.onClick.AddListener(() => LoadGameByID(map.MapID));
        // mapName.text = map.MapName;
        // authorName.text = $"Made by ID: {map.AccountID}";
        // mapPopUp.SetActive(true);
    }
}
