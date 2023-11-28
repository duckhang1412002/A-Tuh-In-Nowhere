using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MapController : MonoBehaviour
{
    public List<Map> SingleMapList {get; set;}
    public List<Map> MultiplayerMapList {get; set;}
    public List<Map> CreativeMapList {get; set;}
    private MapAuthentication mapAuthentication;

    public async void Start(){
        mapAuthentication = MapAuthentication.GetInstance();
        if(mapAuthentication != null){
            SingleMapList = await mapAuthentication.GetSingleMapList();
            SingleMapList = SingleMapList.OrderBy(obj => obj.MapID).ToList();

            MultiplayerMapList = await mapAuthentication.GetMultiplayerMapList();
            MultiplayerMapList = MultiplayerMapList.OrderBy(obj => obj.MapID).ToList();

            CreativeMapList = await mapAuthentication.GetCreativeMapList();
            CreativeMapList = CreativeMapList.OrderBy(obj => obj.MapID).ToList();
        }
    }

    GameObject[] FindObjectsWithNameContaining(string partialName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Use LINQ to filter objects by name
        GameObject[] matchingObjects = allObjects.Where(obj => obj.name.Contains(partialName)).ToArray();

        return matchingObjects;
    }

    public Map GetMapByID(int id, string mode){
        //mode S: Single, mode M: Multiplayer, mode C: Creative 
        if(mode == "S"){
            foreach(Map m in SingleMapList){
                if(m.MapID == id) return m;
            }
        } else if(mode == "M"){
            foreach(Map m in MultiplayerMapList){
                if(m.MapID == id) return m;
            }
        }
        return null;
    }
}