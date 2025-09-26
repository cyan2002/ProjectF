using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //declaring my variables.
    public float speed;
    private float horizontal;
    private float vertical;
    private bool isFacingRight = true;
    private RaycastHit2D hit;
    
    public string facing = "right";
    public string tankFace;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animation;
    
    //Getting input from user running the move function and flip function.
    void Update()
    {
        //vertical and horiontal input from the player
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        findFacing();
        move();
        Flip();
    }

    //reads if player is moving, if player moves then run animation. If not remain idle
    private void move(){
        if(horizontal == 0 && vertical == 0){
            animation.SetBool("IsMoving", false);
        }
        else{
            animation.SetBool("IsMoving", true);
        }
    }

    //updates speed
    private void FixedUpdate()
    {
        //updating speed
        rb.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

    //Flips character sprite based off of velocity
    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //Determines which direction the player is facing so that you can place tanks in the direction that you face
    private void findFacing()
    {
        if(horizontal > 0 && vertical > 0)
        {
            facing = "right";
        }
        else if(horizontal < 0 && vertical < 0)
        {
            facing = "left";
        }
        else if(horizontal > 0 && vertical < 0)
        {
            facing = "right";
        }
        else if(horizontal < 0 && vertical > 0)
        {
            facing = "left";
        }
        else if(horizontal < 0 && vertical == 0)
        {
            facing = "left";
        }
        else if(horizontal > 0 && vertical == 0)
        {
            facing = "right";
        }
        else if(horizontal == 0 && vertical > 0)
        {
            facing = "up";
        }
        else if(horizontal == 0 && vertical < 0)
        {
            facing = "down";
        }
    }

    //checks for tank in front of player, in the direction they are facing
    public bool checkForTank()
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

        if (hit[1] && hit[1].collider.tag == "pickObject")
        {
            //remove object
            Destroy(hit[1].collider.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    public string getDirection()
    {
        return facing;
    }
}
