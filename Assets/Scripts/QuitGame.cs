using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    void OnTriggerStay2D(Collider2D Collision)
    {
        if (Input.anyKey)
        {
            Application.Quit();
        }
    }
}
