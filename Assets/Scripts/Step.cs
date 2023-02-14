using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{

    [SerializeField] private float moveSteps = 2.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    private bool enableMove = true;

    private Vector2 currentPosition = new Vector2(0,0);
    private Vector2 targetPosition = new Vector2(0,0);


    // Start is called before the first frame update
    void Start()
    {

    }
  
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) && enableMove) {
            currentPosition = this.transform.position;      
            targetPosition = new Vector2(transform.position.x, transform.position.y+moveSteps);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow) && enableMove){
            currentPosition = this.transform.position;      
            targetPosition = new Vector2(transform.position.x, transform.position.y-moveSteps);
        } 
        else if(Input.GetKeyDown(KeyCode.LeftArrow) && enableMove) {
            currentPosition = this.transform.position;      
            targetPosition = new Vector2(transform.position.x-moveSteps, transform.position.y);
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) && enableMove) {
            currentPosition = this.transform.position;      
            targetPosition = new Vector2(transform.position.x+moveSteps, transform.position.y);
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        StepMove();   
    }

    void StepMove(){
        this.transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed*Time.deltaTime);
        if(new Vector2(this.transform.position.x,this.transform.position.y) != targetPosition){
            enableMove = false;
        } else {
            enableMove = true;
        }
    }
}
