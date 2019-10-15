using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class syncRotate : MonoBehaviour

{
    public conductorScript script;

    //tHTANK S YUO IAN
    public float theta;
    public float r = 1;
    public float speed = .1f;

    public Camera cam;

    public ParticleSystem party;

    private int score;

    public bool inZone = false; //Whether the note is in the zone

    public fretFeedback fret; //The script of the fret, the circle that the player wants to line up notes with.

    float cooldown = 0f; //The cooldown for inputting keys. Prevents the game from considering a hit to be wrong because the player doesn't release the keys frame-perfectly.
    float cN = 0.2f; //The cooldown amount. Put here for ease of testing.

    public Text pressedKeyText; //Debug, what keys are being pressed
    public Text targetText; //Debug feature, what keys are correct
    public Text debugTargetText; //Debug feature, what keys are correct in the format of pressedKeyText

    string keys = ""; //What keys are being pressed
    string wasdK = ""; //Which wasd key
    string arrowK = ""; //Which arrow key
    string target = "UR"; //What keys the game wants you to press. Currently randomly generated
                          // Left is wasd, right is arrow keys
                          // U : up, D : down, L : left, r : right
                          // So the default here corresponds to w + right arrow

    bool primed = false; //If one key is pressed. This way it only considers a keypress when you press the second key, in case you aren't pressing both on the same frame.
    bool keyed = false; //If both keys are pressed.
    float primeCool = 0f; //How long you can keep one key pressed for before it registers an incorrect hit.
    float pcN = 0.1f; //Default value for pcN

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Vector3.zero;
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = ((2 * Mathf.PI) * ((script.songPosinBeats % 3) / 3));
        //float angle = 0;
        this.transform.localPosition = PointOnCircle(angle, r);
        //Debug.Log(angle);

        //Code for pressing keys
        if (Input.GetKeyDown(KeyCode.W)) //When a key is pressed
        {
            wasdK += "U"; //Set which one was pressed
            if(!primed) //If this is the first key pressed, note that one key has been pressed
            {
                primed = true;
                primeCool = pcN;
            }
            else //If this is the second key pressed, then confirm that a key combination has been entered
            {
                keyed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            wasdK += "L";
            if (!primed)
            {
                primed = true;
                primeCool = pcN;
            }
            else
            {
                keyed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            wasdK += "D";
            if (!primed)
            {
                primed = true;
                primeCool = pcN;
            }
            else
            {
                keyed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            wasdK += "R";
            if (!primed)
            {
                primed = true;
                primeCool = pcN;
            }
            else
            {
                keyed = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            wasdK += "U";
            if (!primed)
            {
                primed = true;
                primeCool = pcN;
            }
            else
            {
                keyed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            wasdK += "L";
            if (!primed)
            {
                primed = true;
                primeCool = pcN;
            }
            else
            {
                keyed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            wasdK += "D";
            if (!primed)
            {
                primed = true;
                primeCool = pcN;
            }
            else
            {
                keyed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            wasdK += "R";
            if (!primed)
            {
                primed = true;
                primeCool = pcN;
            }
            else
            {
                keyed = true;
            }
        }

        //A key is pressed
        if (keyed && cooldown <= 0)
        {
            keys = wasdK + arrowK;
            pressedKeyText.text = keys;
            if (inZone == true) //If the timing is correct
            {
                if (keys.Equals(target)) //If the note is correct, add score and trigger feedback
                {
                    score++;
                    Debug.Log(score);

                    inZone = false;
                    fret.fretHit(true);
                }
                else //Otherwise trigger negative feedback
                {
                    fret.fretHit(false);
                }
            }
            else //Timing incorrect, negative feedback
            {
                fret.fretHit(false);
            }
            cooldown = cN;
            primed = false;
            keyed = false;
            wasdK = "";
        }

        cooldown -= Time.deltaTime;

        if(primed) //If one note has been pressed, trigger a miss if no other note is pressed within 0.1 seconds
        {
            if(primeCool <= 0)
            {
                fret.fretHit(false);
                primed = false;
                wasdK = "";
            }
            primeCool -= Time.deltaTime;
        }

    }

    Vector3 PointOnCircle(float angle, float radius)
    {
        return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //twue..............
        if (other.gameObject.tag == "beat")
        {
            inZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //nop
        if (other.gameObject.tag == "beat")
        {
            inZone = false;
            setTarget(); //Find a new key to want to press
        }
    }

    void setTarget() //Set a new target key combination
    {
        target = ""; //Reset the old combination
        string uiT = "";
        for (int y = 0; y < 2; y++) //Do this twice
        {
            int x = Random.Range(0, 4); //Randomly assign up, down, left, or right
            if (x == 0)
            {
                target += "U";
                uiT += "^ ";
            }
            else if (x == 1)
            {
                target += "L";
                uiT += "< ";
            }
            else if (x == 2)
            {
                target += "D";
                uiT += "v ";
            }
            else if (x == 3)
            {
                target += "R";
                uiT += "> ";
            }
        }
        targetText.text = uiT; //Set debug direction indicator
        debugTargetText.text = target;
    }
        

    //Older code, from a simpler time
    //Keeping it in case we need it
    /*if (inZone == true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    score++;
                    Debug.Log(score);

                    inZone = false;
                    fret.fretHit(true);
                    cooldown = cN;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    fret.fretHit(false);
                    cooldown = cN;
                }
            }*/
}
