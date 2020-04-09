using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundManager : MonoBehaviour //Deals with foreground parallax and display. Should have a 2d trigger collider that covers the entire area where the foreground should be applied. Note that increasing the scale of the gameobject will affect the foreground child, so the size of the collider should be changed directly instead.
{

    public float parallax = 0; //The amount of parallax for the foreground. 0 is a static foreground, 1 follows the player exactly.

    Transform foregroundSprite; //The foreground sprite, should be a child of the foregroundmanager

    //GameObject player;
    GameObject cam; //The camera. I found since the camera has a slight follow delay, it works better if parallax follows that instead of the camera.

    bool inRange = false;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        foregroundSprite = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(inRange)
        {
            //foregroundSprite.transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, parallax), foregroundSprite.transform.position.y, -2);
            foregroundSprite.transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x + (transform.position.x - cam.transform.position.x), parallax), foregroundSprite.transform.position.y, -2); //Sets the foreground position according to parallax. Should find a better way to ensure that the foreground is in front than just setting the z to -2.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            inRange = true;
            foregroundSprite.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            inRange = false;
            foregroundSprite.gameObject.SetActive(false);
        }
    }

}
