using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{
    //Script for doors, used for scene transitions. Should have a boxCollider2D trigger attached, as well as a rigidbody2D to make the trigger work. Needs a reference to the player, a ui image for the fade to black and a gameobject that it teleports the player to.
    //Doors should have the interactableHighlight

    public Image fade; //The image used for fading

    float fadeAmount = 0; //How far along the fade it is

    bool fading = false; //Whether or not it's fading

    float pause = 0; //Waits a bit when at full black
    bool pausing = false; //Whether it's pausing

    public GameObject destinationWaypoint; //The gameobject that the player will be teleported to when clicking on the door. Ideally should be invisible.

    public GameObject player; //The player

    Vector2 playerPos; //The player's position when opening the door

    bool primed = false; //The complicated roundabout way we're detecting if the player wants to go through the door. Is true when the mouse is hovering over the door.
    bool active = false; //True when the mouse is clicked while primed is true. If the player goes into the door while active they will use it.
    bool playerOver = false; //If the player is over the door

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !fading) //If mouse is clicked while not already using the door, then either set it to active if the door is clicked on or deactivate if the door is not clicked on
        {
            if(primed)
            {
                active = true;
            }
            else
            {
                active = false;
            }
        }
        if(active && playerOver) //If the door is active and the player is over it, open the door
        {
            openDoor();
        }

        if(fading) //Fading out
        {
            player.transform.position = playerPos;
            if (fadeAmount < 1)
            {
                //Debug.Log("Fading");
                fade.color = new Color(0, 0, 0, fadeAmount);
                fadeAmount += 2 * Time.deltaTime;
            }
            else
            {
                player.transform.position = destinationWaypoint.transform.position; //Move the player. May want a better way of doing this, and may not move the camera as well
                playerPos = player.transform.position; //Reset the player's position to their new position
                // player.GetComponent<Yarn.Unity.Example.PlayerCharacter>().teleport(playerPos); //Tell the player character motion script that it teleported and therefore should stop trying to move
                fading = false;
                pausing = true;
            }
        }
        if (pausing)
        {
            if (pause < 1) //Pausing for a half second after fading to black
            {
                //Debug.Log("Pause");
                pause += 1 * Time.deltaTime;
                player.transform.position = playerPos;
            }
            else if (fadeAmount > 0) //Fading back in
            {
                fadeAmount -= 2 * Time.deltaTime;
                if (fadeAmount < 0)
                {
                    fadeAmount = 0;
                    pausing = false;
                }
                fade.color = new Color(0, 0, 0, fadeAmount);
                player.transform.position = playerPos;
            }
        }
    }

    //Some method to activate fading when clicked. Should reference scripts used for talking to npcs. May also want to freeze the player when transitioning
    public void openDoor()
    {
        playerPos = player.transform.position; //Mark where the player is
        fading = true; //Start fading
        //Add a function call to stop the player from doing things for a bit
        pause = 0; //reset pause
        primed = false;
        active = false;

    }

    private void OnMouseOver()
    {
        primed = true;
    }

    private void OnMouseExit()
    {
        primed = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ENTERED");
        if(collision.gameObject.tag == "Player")
        {
            playerOver = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerOver = false;
        }
    }

}
