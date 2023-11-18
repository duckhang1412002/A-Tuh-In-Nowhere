using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpriteGlow;

public class MapProjector : MonoBehaviour {
    [SerializeField]
    private GameObject infoBlock_1, infoBlock_2;

    [SerializeField]
    private int[] previousMapProjectorID;

    private List<GameObject[]> wireList;

    [HideInInspector]
    public int ProjectorID;

    [HideInInspector]
    public bool IsSolved, IsUnlocked;

    [HideInInspector]
    public Map MapInfo;

    void Start(){
        if(this.gameObject.name != "Projector"){
            IsUnlocked = false;
            IsSolved = false;
            ProjectorID = int.Parse(this.gameObject.name.Split('_')[3].Trim());
            wireList = new List<GameObject[]>();

            GameObject[] foundWire = FindObjectsWithNameContaining("Wire_Map_" + ProjectorID);    
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

    public int[] GetPreviousMapProjectorID(){
        return previousMapProjectorID;
    }

    public void ChangeMapMachineStatus(bool status, GameObject obj){
        Animator animator;  
        animator = obj.GetComponent<Animator>();

        if(status){
            animator.SetTrigger("MCC-Blue");
            obj.transform.Find("Inner").gameObject.GetComponent<SpriteGlowEffect>().GlowColor = obj.GetComponent<ChangeColor>().GetColor("Blue");
        } else {
            animator.SetTrigger("MCC-Red");
            obj.transform.Find("Inner").gameObject.GetComponent<SpriteGlowEffect>().GlowColor = obj.GetComponent<ChangeColor>().GetColor("Red");
        }
    }
}