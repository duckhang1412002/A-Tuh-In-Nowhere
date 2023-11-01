using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public static bool IsUnlockMultiplayerMode{get; set;}
    public static bool IsUnlockCreativeMode{get; set;}
    private static GameObject hiddenBlock_Mult;
    private static GameObject hiddenBlock_Crea;
    private static GameObject board;

    private static MapBlock[] projectors;

    // Start is called before the first frame update
    void Start()
    {
        projectors = FindObjectsOfType<MapBlock>();
        CheckIsUnlockMultiplayerMode();
        CheckIsUnlockCreativeMode();
    }

    public static void CheckIsUnlockMultiplayerMode(){
        if(IsUnlockMultiplayerMode){
            hiddenBlock_Mult = GameObject.Find("GameObj_Hidden_Mult");

            if(hiddenBlock_Mult == null) return;

            hiddenBlock_Mult.name = "GameObj_None";
            hiddenBlock_Mult.GetComponent<ChangeColor>().ChangeSpriteColor(hiddenBlock_Mult, "None");
        }
    }

    public static void CheckIsUnlockCreativeMode(){
        if(IsUnlockCreativeMode){
            hiddenBlock_Crea = GameObject.Find("GameObj_Hidden_Crea");

            if(hiddenBlock_Crea == null) return;

            hiddenBlock_Crea.name = "GameObj_None";
            board = GameObject.Find("Board");
            board.transform.position = new Vector3((float)0, (float)2.5, (float)5);

            projectors[0].gameObject.GetComponent<Animator>().SetTrigger("MCC-Blue");
            projectors[1].gameObject.GetComponent<Animator>().SetTrigger("MCC-Blue");
        } else {
            projectors[0].gameObject.GetComponent<Animator>().SetTrigger("MCC-Red");
            projectors[1].gameObject.GetComponent<Animator>().SetTrigger("MCC-Red");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
