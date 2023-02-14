using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Eye : MonoBehaviour {
 
    private Animator playerAnim;
    private int direction = 0;
 
    // Use this for initialization
    void Start () {
 
        playerAnim = GetComponent<Animator>();
 
    }
 
    // Update is called once per frame
    void Update () {
   
        if(Input.GetKey(KeyCode.W))
        {
            playerAnim.Play("Player_Walk_Up", 0);
            direction = 1;
        }
 
        if (Input.GetKey(KeyCode.A))
        {
            playerAnim.Play("Player_Walk_Left", 0);
            direction = 2;
        }
 
        if (Input.GetKey(KeyCode.S))
        {
            playerAnim.Play("Player_Walk_Down", 0);
            direction = 3;
        }
 
        if (Input.GetKey(KeyCode.D))
        {
            playerAnim.Play("Player_Walk_Right", 0);
            direction = 4;
        }
 
        if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            if(direction == 1){playerAnim.Play("Player_Face_Up", 0, 0f);}
            if(direction == 2){playerAnim.Play("Player_Face_Left", 0, 0f); }
            if(direction == 3){playerAnim.Play("Player_Face_Down", 0, 0f); }
            if(direction == 4){playerAnim.Play("Player_Face_Right", 0, 0f); }
        }
 
    }
}