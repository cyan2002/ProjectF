using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonQuitGame : MonoBehaviour
{
    [SerializeField] string buttonName;

    //plays when object collider enters a Trigger Collider. Must have a collider and rigidbody
    //mouse and button must have the same tag name
    void OnTriggerStay2D(Collider2D Collision)
    {
        if(Collision.CompareTag(buttonName))
        {
            if (Input.anyKey)
            {
                Application.Quit();
            }
        }
    }
}
