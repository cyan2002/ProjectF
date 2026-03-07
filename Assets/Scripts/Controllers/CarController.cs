using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform leftWheel;
    public Transform rightWheel;
    public float speed;
    public float rotationSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        leftWheel.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        rightWheel.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
