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

    }
}