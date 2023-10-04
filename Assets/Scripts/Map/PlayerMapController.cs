using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMapController : MonoBehaviour
{
    public string CurrentPlayerID { get; set; }
    public List<PlayerMap> ActiveMapList{get; set;}

    public PlayerMapController(string currentPlayerID, List<PlayerMap> activeMapList){
        this.CurrentPlayerID = currentPlayerID;
        this.ActiveMapList = activeMapList;
    }


}
