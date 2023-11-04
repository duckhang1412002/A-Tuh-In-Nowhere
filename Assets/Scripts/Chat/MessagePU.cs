using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePU : MonoBehaviour
{
    // Start is called before the first frame update
    public bool IsActive;
    private float delayTimer = 0f;
    private float delayDuration = 1.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayDuration)
            {
                delayTimer = 0.0f;
                IsActive = false; // Enable input after the delay
            }
        }
    }
}
