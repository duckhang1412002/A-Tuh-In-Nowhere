using UnityEngine;
using UnityEngine.InputSystem;

public class LookCamera : MonoBehaviour
{
    public float speedNormal = 10.0f;
    public float speedFast = 50.0f;

    public float rotationSpeed = 5.0f;
    public Vector3 movementDirection = new Vector3(1.0f, 0.0f, 1.0f);

    float rotY = 0.0f;

    void Start()
    {
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void Update()
    {
        // Rotation
        float rotationInput = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationInput);

        // Movement
        float movementInput = speedNormal * Time.deltaTime;
        transform.Translate(movementDirection * movementInput);

        if (Keyboard.current.uKey.isPressed)
        {
            gameObject.transform.localPosition = new Vector3(0.0f, 3500.0f, 0.0f);
        }
    }
}
