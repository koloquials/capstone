using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turtle : MonoBehaviour
{
    GameObject turtleSprite;

    bool alive = true;

    float x;
    float y=-4.5f;

    public bool player = false; //Whether this turtle is the player
    public float speed; //The speed the non-player turtle moves

    bool leftActive = true; //Used for movement
    bool rightActive = true;

    bool doomed = false; //Whether or not this turtle will die
    float stringCut; //The y position where the turtle dies

    bool safe = false; //If the turtle has reached the end

    bool active = false; //Whether the turtle can start moving

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (alive && y < 4.5f) //If alive and not at the ocean, move to the ocean
            {
                if (player)
                {
                    if (Input.GetKeyUp(KeyCode.LeftArrow) && leftActive)
                    {
                        leftActive = false;
                        rightActive = true;
                        y += 7f * Time.deltaTime;
                    }
                    if (Input.GetKeyUp(KeyCode.RightArrow) && rightActive)
                    {
                        rightActive = false;
                        leftActive = true;
                        y += 7f * Time.deltaTime;
                    }
                    turtleSprite.transform.position = new Vector2(x, y);
                }
                else //An AI turtle will move automatically
                {
                    y += speed * Time.deltaTime;
                    turtleSprite.transform.position = new Vector2(x, y);
                }
            }
            else if (alive && y >= 4.5f)
            {
                safe = true;
            }
            if (alive && doomed && y >= stringCut) //If alive, will die, and at the point where it will die, then the turtle dies
            {
                alive = false;
                turtleSprite.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }

    public void initialize(bool fate, bool p) //Gives whether or not it will die, and whether or not it is the player
    {
        turtleSprite = this.gameObject;
        x = turtleSprite.transform.position.x;
        turtleSprite.GetComponent<SpriteRenderer>().color = Color.green;
        y = -4.5f;
        turtleSprite.transform.position = new Vector2(x, y);
        alive = true;
        leftActive = true;
        rightActive = true;
        safe = false;

        doomed = fate;
        if(doomed)
        {
            if (p)
                stringCut = Random.Range(0f, 3f);
            else
                stringCut = Random.Range(-4f, 3f);
        }
        player = p;
        speed = Random.Range(0.8f, 2f);
        active=true;
    }

    public bool isDead()
    {
        return !alive;
    }

    public bool isSafe()
    {
        return safe;
    }
}
