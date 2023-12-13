using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
 
public class Eye : MonoBehaviour {
    public GameObject eye;

    private bool isNotMove = true;
    private bool isNotRunCoroutine = true;

    [SerializeField]
    private float timeWaitAnimation = 3.0f;

    private IEnumerator coroutine = null;

    // Use this for initialization
    void Start () {

    }
 
    // Update is called once per frame
    void Update () {
        if(
        !Input.GetKeyDown(KeyCode.UpArrow) && 
        !Input.GetKeyDown(KeyCode.DownArrow) && 
        !Input.GetKeyDown(KeyCode.LeftArrow) && 
        !Input.GetKeyDown(KeyCode.RightArrow)
        ){
            isNotMove = true;
        }else{
            isNotMove = false;
            isNotRunCoroutine = true;
            StopCoroutine(coroutine);
        }

        if(isNotMove && isNotRunCoroutine) {
            isNotRunCoroutine = false;
            coroutine = EyeIdle("Eye-Idle");
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator EyeIdle(string animationName){
        yield return new WaitForSeconds(timeWaitAnimation);
        Animator _animator = eye.GetComponent<Animator>();
        _animator.Play(animationName);
        float clipLength = _animator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length;
        yield return new WaitForSeconds(clipLength);
        StopCoroutine(coroutine);
        isNotRunCoroutine = true;
    }
}