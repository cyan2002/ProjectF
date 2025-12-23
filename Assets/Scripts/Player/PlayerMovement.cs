using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //declaring my variables.
    public float speed;
    private bool isFacingRight = true;
    private RaycastHit2D hit;

    private Vector2 moveInput;
    
    public string facing = "right";
    public string tankFace;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private new Animator animation;

    void Start(){
        PlayerInput.onMove += HandleMove;
    }

    void HandleMove(Vector2 input){
        moveInput = input;
    }

    void Update(){
        move(moveInput);
    }

    //reads if player is moving, if player moves then run animation. If not remain idle
    private void move(Vector2 movement){
        if(movement.x == 0 && movement.y == 0){
            animation.SetBool("IsMoving", false);
        }
        else{
            animation.SetBool("IsMoving", true);
        }
        //updating speed
        rb.velocity = new Vector2(movement.x * speed, movement.y * speed);

        findFacing(movement);
        Flip(movement);
    }

    //Flips character sprite based off of velocity
    private void Flip(Vector2 movement)
    {
        if (isFacingRight && movement.x < 0f || !isFacingRight && movement.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //Determines which direction the player is facing so that you can place tanks in the direction that you face
    private void findFacing(Vector2 movement)
    {
        if(movement.x > 0 && movement.y > 0)
        {
            facing = "right";
        }
        else if(movement.x < 0 && movement.y < 0)
        {
            facing = "left";
        }
        else if(movement.x > 0 && movement.y < 0)
        {
            facing = "right";
        }
        else if(movement.x < 0 && movement.y > 0)
        {
            facing = "left";
        }
        else if(movement.x < 0 && movement.y == 0)
        {
            facing = "left";
        }
        else if(movement.x > 0 && movement.y == 0)
        {
            facing = "right";
        }
        else if(movement.x == 0 && movement.y > 0)
        {
            facing = "up";
        }
        else if(movement.x == 0 && movement.y < 0)
        {
            facing = "down";
        }
    }

    //checks for tank in front of player, in the direction they are facing
    public GameObject checkForTank()
    {
        //playing around with raycast
        RaycastHit2D[] hit;

        //Creating a raycast in the direction that the player is facing to detect if it sees a tank in front of it
        if (facing == "right")
        {
            hit = Physics2D.RaycastAll(transform.position, Vector2.right, 1.5f);
            tankFace = "right";
        }
        else if(facing == "up")
        {
            hit = Physics2D.RaycastAll(transform.position, Vector2.up, 1.5f);
            tankFace = "up";
        }
        else if(facing == "down")
        {
            hit = Physics2D.RaycastAll(transform.position, -Vector2.up, 1.5f);
            tankFace = "down";
        }
        else
        {
            hit = Physics2D.RaycastAll(transform.position, -Vector2.right, 1.5f);
            tankFace = "left";
        }
        
        for(int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.tag == "pickObject")
            {
                //return object to remove
                return hit[i].collider.gameObject;
            }
        }

        return null;
    }

    public string getDirection()
    {
        return facing;
    }
}
