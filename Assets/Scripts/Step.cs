using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{

    [SerializeField] private float moveStep = 2.0f;


    // Start is called before the first frame update
    void Start()
    {

    }
  
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) {
            Debug.Log("Key Up"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x,this.transform.position.y+1));          
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)){
            Debug.Log("Key Down"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x,this.transform.position.y-1)); 
        } 
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            Debug.Log("Key Left"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x-1,this.transform.position.y)); 
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            Debug.Log("Key Right"); 
            StepMove(this.transform.position, new Vector2(this.transform.position.x+1,this.transform.position.y));
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        void StepMove(Vector2 currentPosition, Vector2 target){
            this.transform.position = Vector2.MoveTowards(currentPosition, target, moveStep);
        }
    }   
}
