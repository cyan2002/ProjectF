using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    
    //both varaibles below must be changed when opening and closing an inventory in each individual script
    public string ActiveInventory { get; set; } = "NA";
    public bool canOpen { get; set; } = true;

    public static event Action<Vector2> onMove;
    public static event Action HandleB;
    public static event Action HandleM;
    public static event Action HandleP;
    public static event Action HandleI;
    public static event Action HandleR;
    public static event Action HandleLeftClick;
    public static event Action HandleJ;
    public static event Action HandleK;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        onMove?.Invoke(move);

        //Shop Open
        if(Input.GetKeyDown(KeyCode.B)){
            HandleB?.Invoke();
        }
        //Shelf Inventory Open
        if(Input.GetKeyDown(KeyCode.M)){
            HandleM?.Invoke();
        }
        //pick up items from shipping containers
        if(Input.GetKeyDown(KeyCode.P)){
            HandleP?.Invoke();
        }
        //Player Inventory Open
        if(Input.GetKeyDown(KeyCode.I)){
            HandleI?.Invoke();
        }
        //rotate Object
        if(Input.GetKeyDown(KeyCode.R)){
            HandleR?.Invoke();
        }
        //picking up and placing items
        if (Input.GetMouseButtonDown(0)){
            HandleLeftClick?.Invoke();
        }

        //both are for testing purposes
        if(Input.GetKeyDown(KeyCode.J)){
            HandleJ?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.K)){
            HandleK?.Invoke();
        }
    }
}

//current UIScripts accessing the Input
//Shop
//Shelf
//Player