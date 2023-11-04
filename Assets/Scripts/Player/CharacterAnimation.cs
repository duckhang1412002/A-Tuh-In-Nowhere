using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
 
public class CharacterAnimation : MonoBehaviour {

    private Animator animator;
    public bool IsMoving { get; set; }

    // Use this for initialization
    void Start () {
        IsMoving = false;

        GameObject wholePlayerObject = this.gameObject.transform.Find("PlayerInner").transform.Find("WholePlayerObject").gameObject;
        animator = wholePlayerObject.GetComponent<Animator>();
    }
 
    void Update(){
        if(
        Input.GetKeyDown(KeyCode.UpArrow) ||
        Input.GetKeyDown(KeyCode.DownArrow) ||
        Input.GetKeyDown(KeyCode.LeftArrow) ||
        Input.GetKeyDown(KeyCode.RightArrow)
        ){
            animator.SetTrigger("IsMoving");
        }
    }
}