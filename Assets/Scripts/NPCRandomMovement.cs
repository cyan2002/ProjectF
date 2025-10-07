using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRandomMovement : MonoBehaviour
{
    public string direction;
    public bool pause = false;
    RaycastHit2D[] hit;
    public float speed;

    public float resetTimer = 0f;
    public float pauseTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        decideInitialDirection();
    }

    void Update()
    {
        timer();
        
        //move NPC if not pause in the direction it's been assigned. Also checks if the direction is valid and if not then decides a new direction.
        if (!pause)
        {
            if (direction == "up")
            {
                rb.AddForce(new Vector2(0, 1));
                if (checkForBarrier())
                {
                    decideDirection();
                }
            }
            else if (direction == "down")
            {
                rb.AddForce(new Vector2(0, -1));
                if (checkForBarrier())
                {
                    decideDirection();
                }
            }
            else if (direction == "right")
            {
                rb.AddForce(new Vector2(1, 0));
                if (checkForBarrier())
                {
                    print("hi");
                    decideDirection();
                }
            }
            else if (direction == "left")
            {
                rb.AddForce(new Vector2(-1, 0));
                if (checkForBarrier())
                {
                    print("bye");
                    decideDirection();
                }
            }
            else
            {
                print("error!");
            }
        }
    }
    
    //dealing with timer things.
    void timer()
    {
        //timer so that the random switch doesn't get called multiple times when it's near a 0.5 on the grid point.
        if (resetTimer >= 1)
        {
            randomSwitch();
            resetTimer = 0f;
        }
        else
        {
            resetTimer += Time.deltaTime;
        }

        //timer that is randomized so NPC sometimes takes pauses, anywhere between 5 to 20 seconds it can pause
        //pause can happen on non-grid spaces (between 0.5f)
        if (pauseTimer >= Random.Range(5f, 20f))
        {
            pause = true;
            pauseTimer = 0f;
        }
        //if the NPC is already paused, must wait a certain amount of time before moving again.
        else if (pause)
        {
            pauseTimer += Time.deltaTime;
            //only pauses for about 5-7 seconds
            if (pauseTimer >= Random.Range(5f, 7f))
            {
                pause = false;
                pauseTimer = 0f;
            }
        }
        else
        {
            pauseTimer += Time.deltaTime;
        }
    }

    //compares the distance between a point on the grid (that matches up to the grid) and the player's position and allows for the NPC to possibly turn
    void randomSwitch()
    {
        Vector3 upVec = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, 0);
        //checks to see if the position is close enough to the grid position.
        if (Vector3.Distance(transform.position, upVec) <= 0.05f)
        {
            decideDirection();
        }
    }

    //Decides the initial direction of the character.
    void decideInitialDirection()
    {
        float num = Random.Range(0f, 1f);
        if (num <= .25f)
        {
            direction = "up";
            if (checkForBarrier())
            {
                decideDirection();
            }
        }
        else if (num <= .50f)
        {
            direction = "down";
            if (checkForBarrier())
            {
                decideDirection();
            }
        }
        else if (num <= .75f)
        {
            direction = "right";
            if (checkForBarrier())
            {
                decideDirection();
            }
        }
        else
        {
            direction = "left";
            if (checkForBarrier())
            {
                decideDirection();
            }
        }
    }

    //decides the direction of the NPC based off of chance. No pun intended. Change change here to get a different feel for NPC movement.
    //I want to reduce or remove the chance of the NPC going backwards.
    void decideDirection()
    {
        float num = Random.Range(0f, 1f);

        if(num <= .5f)
        {
            num = Random.Range(0f, 1f);
            if (num <= .25f)
            {
                direction = "up";
                if (checkForBarrier())
                {
                    decideDirection();
                }
            }
            else if (num <= .50f)
            {
                direction = "down";
                if (checkForBarrier())
                {
                    decideDirection();
                }
            }
            else if (num <= .75f)
            {
                direction = "right";
                if (checkForBarrier())
                {
                    decideDirection();
                }
            }
            else
            {
                direction = "left";
                if (checkForBarrier())
                {
                    decideDirection();
                }
            }
        } 
    }

    //checks if there is a barrier in the direct the NPC is moving, if there is it return true, if not returns false.
    private bool checkForBarrier()
    {
        if (direction == "up")
        {
            hit = Physics2D.RaycastAll(transform.position, Vector2.up, .5f);
        }
        else if (direction == "down")
        {
            hit = Physics2D.RaycastAll(transform.position, -Vector2.up, .5f);
        }
        else if (direction == "right")
        {
            hit = Physics2D.RaycastAll(transform.position, Vector2.right, .5f);
        }
        else if (direction == "left")
        {
            hit = Physics2D.RaycastAll(transform.position, -Vector2.right, .5f);
        }
        else
        {
            print("error!");
        }

        for(int i = 0; i < hit.Length; i++)
        {
            if(hit[i].collider.tag == "Scene" || hit[i].collider.tag == "pickObject")
            {
                return true;
            }
        }
        return false;
    }
}
