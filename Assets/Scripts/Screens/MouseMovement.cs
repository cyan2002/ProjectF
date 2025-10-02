using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
   void Start()
    {
        Cursor.visible = true;
    }   
    // Update is called once per frame
    void Update()
    {
        //changes the object position to the mouse position
        //camera.main convert it to the camera's view
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
    }
}
