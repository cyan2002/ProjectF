using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySymbol : MonoBehaviour
{
    public float speed = 2f;
    public float lifetime = 1f;

    private float timer;

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        timer += Time.deltaTime;

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
