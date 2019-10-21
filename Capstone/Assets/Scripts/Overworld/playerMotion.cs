using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMotion : MonoBehaviour
{

    //Apply to player, lets them move
    //Player must have a 2d rigidbody, a 2d collider for main collision, and a 2d trigger collider around their feet. 

    Rigidbody2D rb; //The player's rigidbody.

    public float speed = 8f; //Modifies how fast the player accelerates.
    public float maxSpeed = 1.5f; //Max speed of the player.
    bool still = true; //Whether the player is not trying to move

    float speedConstant = 5f; //Used because rigidbody velocity needs to be higher.

    bool grounded = false; //Whether the player is on the ground.

    public float jumpSpeed = 1.5f; //The upward velocity the player gets while jumping
    float boost = 0.2f; //Used so the player can hold space to jump higher. Maybe not important, but makes jumping more fun.
    float boostMax = 0.2f; //Maximum boost you can have
    bool jumping = false; //If the player is holding down space;
    public float gravity = 2f; //How high gravity should be, important because the script changes rigidbody gravity
    public float lowGrav = 0.5f; //How high the gravity should be when early in a jump.

    public bool momentumJump = false; //Test variable, means that not pressing anything during a jump will preserve left and right momentum

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vel = rb.velocity; //Get the player's velocity
        still = true; //Default still to true
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) //Move left
        {
            still = false; //Trying to move, still is false
            if(vel.x > -1 * maxSpeed * speedConstant) //If you aren't at max speed, increase speed
            {
                vel.x -= speed * speedConstant * Time.deltaTime;
            }
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) //Move right, same as above but different direction
        {
            still = false;
            if(vel.x < maxSpeed * speedConstant)
            {
                vel.x += speed * speedConstant * Time.deltaTime;
            }
        }
        if (momentumJump) //Testing purposes, decides how jumping cares about not pressing arrow keys
        {
            if (still && grounded) //If the player is on the ground and not moving, slow down
            {
                vel.x = Mathf.Lerp(vel.x, 0, 0.03f);
                if (vel.x > -0.01 && vel.x < 0.01)
                    vel.x = 0;
            }
        }
        else
        {
            if (still) //If the player is not moving, slow down
            {
                vel.x = Mathf.Lerp(vel.x, 0, 0.03f);
                if (vel.x > -0.01 && vel.x < 0.01)
                    vel.x = 0;
            }
        }

        rb.velocity = vel; //Set the velocity

        if(grounded && Input.GetKeyDown(KeyCode.Space)) //If on the ground and pressing space, jump
        {
            jumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * speedConstant); //Set the jump speed
            rb.gravityScale = lowGrav; //Less gravity temporarily
        }
        if(Input.GetKeyUp(KeyCode.Space)) //Stop adding upward momentum when space is let go
        {
            jumping = false;
            rb.gravityScale = gravity; //Reset gravity
        }
        if(jumping) //If holding space to jump
        {
            //rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * speedConstant); //Your velocity won't go down
            boost -= Time.deltaTime;
            if(boost <= 0) //For about 0.5 seconds
            {
                jumping = false;
                rb.gravityScale = gravity;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //If the trigger isn't in a collider, the player isn't on the ground.
    {
        grounded = false;
    }

    private void OnTriggerStay2D(Collider2D collision) //If the trigger is touching a collider, the player is on the ground. Important that it's after TriggerExit, so it doesn't consider you as in the air if you walk between two floor colliders. 
    {
        grounded = true;
        boost = boostMax;
        if(rb.velocity.y < -0.1) //Sometimes prevents a very annoying effect where the player jumps into the floor for a frame before being pushed out. Jarring and unpleasant. Smoother landings means it doesn't happen as often.
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Lerp(rb.velocity.y, 0, 0.5f));
        }
    }
}
