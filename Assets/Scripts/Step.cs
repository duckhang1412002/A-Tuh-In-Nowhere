using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 2.0f;
    private float step = 0;

    // Start is called before the first frame update
    void Start()
    {

    }
  
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.UpArrow)) {
        //     Debug.Log("Key Up");
        //     transform.Translate(0, 2.0f, 0);
        // }
        // else if(Input.GetKeyDown(KeyCode.DownArrow)){
        //    Debug.Log("Key Down"); 
        //    transform.Translate(0, -2.0f, 0);
        // } 
        // else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
        //     Debug.Log("Key Left"); 
        //     transform.Translate(-2.0f, 0, 0);
        // }
        // else if(Input.GetKeyDown(KeyCode.RightArrow)) {
        //     Debug.Log("Key Right"); 
        //     transform.Translate(2.0f, 0, 0);
        // }
        step = moveSpeed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.UpArrow)) {
            Debug.Log("Key Up"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x,this.transform.position.y+1*100));          
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)){
            Debug.Log("Key Down"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x,this.transform.position.y-1*100)); 
        } 
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            Debug.Log("Key Left"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x-1*100,this.transform.position.y)); 
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            Debug.Log("Key Right"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x+1*100,this.transform.position.y));
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        void StepMove(Vector2 currentPosition, Vector2 target){
            this.transform.position = Vector2.MoveTowards(currentPosition, target, moveSpeed);
        }
    }   
    
}
