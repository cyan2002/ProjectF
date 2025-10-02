using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ButtonChangeScene : MonoBehaviour
{
    [SerializeField] private string buttonName;
    [SerializeField] private string nextLevel;

    // Update is called once per frame
    
    //plays when object collider enters a Trigger Collider. Must have a collider and rigidbody
    void OnTriggerStay2D(Collider2D Collision)
    {
        if (Collision.CompareTag(buttonName))
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(nextLevel);
            }
        }
    }
}
