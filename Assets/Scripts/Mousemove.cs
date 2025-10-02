using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mousemove : MonoBehaviour
{
   void Start()
    {
        Cursor.visible = true;
    }   
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        transform.position = mousePos;
    }
}
