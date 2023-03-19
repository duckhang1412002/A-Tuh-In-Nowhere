using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class PlayerFloat : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 lastVelocity;

    [SerializeField]
    private float speed;

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(9.8f * speed, 9.8f * speed));
    }

    void Update(){
        lastVelocity = rb.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var speed = lastVelocity.magnitude;
        var direction = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
        rb.velocity = direction * Mathf.Max(speed, 0f);
    }
}