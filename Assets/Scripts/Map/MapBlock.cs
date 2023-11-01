using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBlock : MonoBehaviour {
    [SerializeField]
    private GameObject infoBlock_1;

    [SerializeField]
    private GameObject infoBlock_2;

    [SerializeField]
    private int[] previousMapID;

    private List<GameObject[]> wireList;

    public int MapID{get; set;}
    public bool IsUnlocked{get; set;}
    public bool IsSolved{get; set;}


    void Start(){
        if(this.gameObject.name != "Projector"){
            IsUnlocked = false;
            IsSolved = false;
            MapID = int.Parse(this.gameObject.name.Split('_')[3].Trim());
            wireList = new List<GameObject[]>();

            GameObject[] foundWire = FindObjectsWithNameContaining("Wire_Map_" + MapID);    
            wireList.Add(foundWire);
        }
    }

    // Custom method to find game objects by name containing a specific string
    private GameObject[] FindObjectsWithNameContaining(string partialName) {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Use LINQ to filter objects by name
        GameObject[] matchingObjects = allObjects.Where(obj => obj.name.Contains(partialName)).ToArray();

        return matchingObjects;
    }

    public void ChangeColor(){
        ChangeColor changeColorObject = this.gameObject.GetComponent<ChangeColor>();
        foreach(GameObject[] wire in wireList){
            foreach(GameObject w in wire){
                changeColorObject.ChangeSpriteColor(w, "Green");
            }
        }
    }

    public int[] GetPreviousMapID(){
        return previousMapID;
    }

    public void ChangeMapMachineStatus(bool status, GameObject obj){
        Animator animator;  
        animator = obj.GetComponent<Animator>();

        if(status){
            animator.SetTrigger("MCC-Blue");
        } else {
            animator.SetTrigger("MCC-Red");
        }
    }
}