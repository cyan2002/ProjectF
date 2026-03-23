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
    public UnityEngine.Rendering.Universal.Light2D carLight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0f);
        carLight.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Clock.Instance.timeOfDay >= 18 || Clock.Instance.timeOfDay <= 8)
        {
            carLight.enabled = true;
        }
        else
        {
            carLight.enabled = false;
        }
        leftWheel.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        rightWheel.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
