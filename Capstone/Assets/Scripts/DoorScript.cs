using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{
    //Script for doors, used for scene transitions. Needs a reference to the player, a ui image for the fade to black and a gameobject that it teleports the player to.
    //Doors should have the interactableHighlight

    public Image fade; //The image used for fading

    float fadeAmount = 0; //How far along the fade it is

    bool fading = false; //Whether or not it's fading

    float pause = 0; //Waits a bit when at full black

    public GameObject destinationWaypoint; //The gameobject that the player will be teleported to when clicking on the door. Ideally should be invisible.

    public GameObject player; //The player

    Vector2 playerPos; //The player's position when opening the door

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(fading) //Fading out
        {
            player.transform.position = playerPos;
            if(fadeAmount < 1)
            {
                fade.color = new Color(0, 0, 0, fadeAmount);
                fadeAmount += 2 * Time.deltaTime;
            }
            else if (pause < 1) //Pausing for a half second after fading to black
            {
                pause += 2 * Time.deltaTime;
            }
            else //Moving the player, starting to fade back in
            {
                fading = false;
                player.transform.position = destinationWaypoint.transform.position; //Move the player. May want a better way of doing this, and may not move the camera as well
                playerPos = player.transform.position; //Reset the player's position to their new position
            }
        }
        else if (fadeAmount > 0) //Fading back in
        {
            fadeAmount -= 2 * Time.deltaTime;
            if (fadeAmount < 0)
                fadeAmount = 0;
            fade.color = new Color(0, 0, 0, fadeAmount);
            player.transform.position = playerPos;
        }
    }

    //Some method to activate fading when clicked. Should reference scripts used for talking to npcs. May also want to freeze the player when transitioning
    public void openDoor()
    {
        playerPos = player.transform.position; //Mark where the player is
        fading = true; //Start fading
        //Add a function call to stop the player from doing things for a bit
        pause = 0; //reset pause
    }
}
