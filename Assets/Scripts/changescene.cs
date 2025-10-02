using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class changescene : MonoBehaviour

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
            SceneManager.LoadScene(nextLevel);
        }
    }
}
