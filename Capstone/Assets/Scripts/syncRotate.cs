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

    SpriteRenderer sr; //The sprite renderer for the note. Currently used to denote phase 3 health.

    private int score;

    public bool inZone = false; //Whether the note is in the zone

    public fretFeedback fret; //The script of the fret, the circle that the player wants to line up notes with.

    float cooldown = 0f; //The cooldown for inputting keys. Prevents the game from considering a hit to be wrong because the player doesn't release the keys frame-perfectly.
    float cN = 0.2f; //The cooldown amount. Put here for ease of testing.

    bool hit = false; //If a note is hit this cycle
    bool miss = false; //If a note is missed this tick

    public Text pressedKeyText; //Debug, what keys are being pressed
    public Text targetText; //Debug feature, what keys are correct
    public Text debugTargetText; //Debug feature, what keys are correct in the format of pressedKeyText
    public Text phaseText; //Debug feature, what phase we're in
    public Text scoreText; //Debug feature, score count

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

    int phase = 0; //Phase of the game. 0 waits for a single correct press, 1 is repeating the basic pattern, 2 expands to the bars with moving notes.
    //Any miss during phases 0 or 1 resets to phase 0. Having a negative score in phase 2 resets to phase 0

    bool pattern = true; //Whether the notes should be a preset pattern (true), or random (false)
    string[] song1 = { "UU", "DD", "LL", "RR", "UD", "LR", "UU","DD","LL","RR"}; //For example
    string[] song1direction = { "^ ^","v v","< <", "> >", "^ v", "< >", "^ ^", "v v", "< <", "> >" }; //Direction equivalent of the notes. Ideally find a way to automate making these.
    int song1Max = 10; //How many notes are in this song.
    int currentNote = 0; //Which note of the song is the active one. Once this is equal to the song max, should either reset to 0 or progress the phase.
    //Nonrandom is on the to-do list.

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Vector3.zero;
        score = 0;
        sr = GetComponent<SpriteRenderer>();
        setTarget();
    }

    // Update is called once per frame
    void Update()
    {
        miss = false;

        
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
                    //Debug.Log(score);

                    inZone = false;
                    fret.fretHit(true);
                    hit = true;
                    
                }
                else //Otherwise trigger negative feedback
                {
                    fret.fretHit(false);
                    score--;
                    miss = true;
                }
            }
            else //Timing incorrect, negative feedback
            {
                fret.fretHit(false);
                score--;
                miss = true;
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
                score--;
            }
            primeCool -= Time.deltaTime;
        }

        phaseCheck();
        phaseText.text = "Phase: " + phase;
        scoreText.text = "Score: " + score;
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
            if(!hit && cooldown <= 0)
            {
                score--;
                miss = true;
                fret.fretHit(false);
                phaseCheck();
            }
            setTarget(); //Find a new key to want to press
            miss = false;
            hit = false;
        }
    }

    void phaseCheck() //Checks the result of misses and hits on the game's phases.
    {
        if (phase == 0) //Phase 0, the note stands still until you hit it, starting phase 1
        {
            if (miss)
            {
                score = 0;
                currentNote = 0;
                setTarget();
            }
            else if (hit)
            {
                phase = 1;
                script.startMusic();
            }
        }
        else if (phase == 1) //Phase 1, have to hit 10 notes in a row to go to phase 2
        {
            if (miss) //A miss in phase 1 resets to phase 0
            {
                script.stopMusic();
                score = 0;
                phase = 0;
                currentNote = 0;
                setTarget();
            }
            else if (hit && score >= 10)
            {
                phase = 2;
                pattern = false;
                sr.color = Color.blue;
            }
        }
        else if (phase == 2) //Phase 2, keep score above 0
        {
            if (score <= 0)
            {
                sr.color = Color.gray;
                script.stopMusic();
                score = 0;
                phase = 0;
                pattern = true;
                currentNote = 0;
                setTarget();
            }
            else if(score >= 10)
            {
                sr.color = Color.blue;
            }
            else
            {
                sr.color = new Color(1f - (score / 10f), 0, 1f);
            }
        }
    }

    void setTarget() //Set a new target key combination
    {
        target = ""; //Reset the old combination
        string uiT = "";
        if (pattern)
        {
            target = song1[currentNote];
            uiT = song1direction[currentNote];
            currentNote++;
            if (currentNote >= song1Max)
                currentNote = 0;
        }
        else
        {
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
