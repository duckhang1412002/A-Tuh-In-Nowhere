using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager instance = null;
    
    [SerializeField]
    private AudioSource[] audio;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        if (instance == this) return; 
        Destroy(gameObject);
    }


    void Start()
    {
        audio[0].enabled = true;
        audio[1].enabled = false;
    }

    void Update(){
        if(SceneManager.GetActiveScene().buildIndex == 0){      
            audio[0].enabled = true;
            audio[1].enabled = false;
        } else {
            audio[0].enabled = false;
            audio[1].enabled = true;
        }
    }
}
