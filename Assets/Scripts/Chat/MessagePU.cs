using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePU : MonoBehaviour
{
    // Start is called before the first frame update
    private bool IsActive;
    private float delayTimer = 0f;
    private float delayDuration = 2f;
    void Start()
    {
        IsActive = gameObject.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
        IsActive = gameObject.activeSelf;
        if (IsActive)
        {
            Debug.Log("Chat PU is active!");
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayDuration)
            {
                delayTimer = 0.0f;
                IsActive = false; // Enable input after the delay
                gameObject.SetActive(false);
            }
        }
    }
}
