using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class syncRotate : MonoBehaviour

{
    //FOR CLARIFICATION: This script goes on the smaller circle orbiting a point on the screen. The fret is the larger circle intersecting the smaller circle's orbit on the right side, which should have the fretFeedback script.
    //The spriterenderer of both this and the fret should start disabled.

    public conductorScript script;

    int starting = 0; //Handles entering the rhythm game. 0 is not started, 1 is the little intro animation, 2 is ready to go.

    float startScale = 0f; //Scale of the rotating ball at the start. Used for the intro animation.

    public float maxScale = 0.5f; //Maximum scale of the rotating ball.

    float startTimer = 0f; //Used for the start animation.

    Color defaultCol;

    //Sprites representing the 16 possible note combinations
    public Sprite UU;
    public Sprite UD;
    public Sprite UL;
    public Sprite UR;
    public Sprite DU;
    public Sprite DD;
    public Sprite DL;
    public Sprite DR;
    public Sprite LU;
    public Sprite LD;
    public Sprite LL;
    public Sprite LR;
    public Sprite RU;
    public Sprite RD;
    public Sprite RL;
    public Sprite RR;

    //The actual gameobjects for the notes
    public note[] noteSprites; //Should be length 5.

    //tHTANK S YUO IAN (Variables used to make orbits)
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
    string target = "UU"; //What keys the game wants you to press.
                          // Left is wasd, right is arrow keys
                          // U : up, D : down, L : left, r : right
                          // So the default here corresponds to w + right arrow
    int nextNote = 0; //Which note is the next to hit the main.
    string[] noteList = new string[5]; //List of the next five notes

    bool primed = false; //If one key is pressed. This way it only considers a keypress when you press the second key, in case you aren't pressing both on the same frame.
    bool keyed = false; //If both keys are pressed.
    bool lk = false; //Left key pressed
    bool rk = false; //Right key pressed
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
        transform.localScale = Vector2.zero; //For the start animation

        this.transform.position = Vector3.zero;
        score = 0;
        sr = GetComponent<SpriteRenderer>();

        defaultCol = sr.color;

        for (int x = 0; x < noteSprites.Length; x++)
        {
            noteSprites[x].setBPM(script.sBPM);
            noteSprites[x].setDestination(fret.transform.position);
        }

        for (int x = 0; x < 5; x++)
        {
            setTarget(false); //Set the first 5 notes
        }
        target = song1[0]; //Sets the target to the first note in the song. Has to be this way because of what I think was an issue with setting sprites in start. Also why the fret's default sprite should be the first note to press.
    }

    // Update is called once per frame
    void Update()
    {
        if (starting == 0)
        {
            if(Input.GetKey(KeyCode.Space)) //Debug Feature, starts the rhythm game. In the future, the rhythm game should be started based on the dialogue script.
            {
                begin();
            }
        }
        if (starting == 1)
        {
            startTimer += Time.deltaTime;
            if (startTimer > 1)
            {
                startScale = Mathf.Lerp(startScale, maxScale, 0.1f);
                if (startScale > maxScale + 0.1f)
                {
                    startScale = maxScale;
                }
                transform.localScale = new Vector2(startScale, startScale);
            }
            if(startTimer > 2 && startTimer < 2.1f)
            {
                fret.startingScaling();
            }
            if(startTimer > 3)
            {
                starting = 2;
            }
        }
        else if (starting == 2)
        {
            if(Input.GetKey(KeyCode.Escape)) //Closes the rhythm game. In the future, this could also pass a value to the dialog manager so characters can comment on how you quit.
            {
                end();
            }
            miss = false;


            float angle = ((2 * Mathf.PI) * ((script.songPosinBeats % 3) / 3));
            //float angle = 0;
            this.transform.localPosition = PointOnCircle(angle, r);
            //Debug.Log(angle);

            //Code for pressing keys
            if (Input.GetKeyDown(KeyCode.W)) //When a key is pressed
            {
                wasdK = "U"; //Set which one was pressed
                if (!rk) //If this is the first key pressed, note that one key has been pressed
                {
                    primed = true;
                    lk = true;
                    primeCool = pcN;
                }
                else //If this is the second key pressed, then confirm that a key combination has been entered
                {
                    keyed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                wasdK = "L";
                if (!rk)
                {
                    primed = true;
                    lk = true;
                    primeCool = pcN;
                }
                else
                {
                    keyed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                wasdK = "D";
                if (!rk)
                {
                    primed = true;
                    lk = true;
                    primeCool = pcN;
                }
                else
                {
                    keyed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                wasdK = "R";
                if (!rk)
                {
                    primed = true;
                    lk = true;
                    primeCool = pcN;
                }
                else
                {
                    keyed = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                arrowK = "U";
                if (!lk)
                {
                    primed = true;
                    rk = true;
                    primeCool = pcN;
                }
                else
                {
                    keyed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                arrowK = "L";
                if (!lk)
                {
                    primed = true;
                    rk = true;
                    primeCool = pcN;
                }
                else
                {
                    keyed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                arrowK = "D";
                if (!lk)
                {
                    primed = true;
                    rk = true;
                    primeCool = pcN;
                }
                else
                {
                    keyed = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                arrowK = "R";
                if (!lk)
                {
                    primed = true;
                    rk = true;
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
                lk = false;
                rk = false;
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

            if (primed) //If one note has been pressed, trigger a miss if no other note is pressed within 0.1 seconds
            {
                if (primeCool <= 0)
                {
                    fret.fretHit(false);
                    primed = false;
                    lk = false;
                    rk = false;
                    wasdK = "";
                    arrowK = "";
                    score--;
                }
                primeCool -= Time.deltaTime;
            }

            phaseCheck();
            phaseText.text = "Phase: " + phase;
            scoreText.text = "Score: " + score;
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
            if(!hit && cooldown <= 0)
            {
                score--;
                miss = true;
                fret.fretHit(false);
                phaseCheck();
            }
            setTarget(true); //Find a new key to want to press
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
                pattern = true;
                currentNote = 0;
                for(int x = 0; x < 5; x++)
                    setTarget(true);
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
                for (int x = 0; x < 5; x++)
                    setTarget(true);
            }
            else if (hit && score >= 10)
            {
                for (int x = 0; x < noteSprites.Length; x++)
                {
                    noteSprites[x].startMotion();
                }
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
                for(int x=0; x<noteSprites.Length; x++)
                {
                    noteSprites[x].stopMotion();
                }
                score = 0;
                phase = 0;
                pattern = true;
                currentNote = 0;
                for (int x = 0; x < 5; x++)
                    setTarget(true);
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

    void setTarget(bool a) //Set a new target key combination. Parameter is if this is being called from start, because of an error on the first time it's called.
    {
        string next = ""; //Reset the old combination
        //string uiT = "";
        if (pattern)
        {
            next = song1[currentNote];
            //uiT = song1direction[currentNote];
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
                    next += "U";
                    //uiT += "^ ";
                }
                else if (x == 1)
                {
                    next += "L";
                    //uiT += "< ";
                }
                else if (x == 2)
                {
                    next += "D";
                    //uiT += "v ";
                }
                else if (x == 3)
                {
                    next += "R";
                    //uiT += "> ";
                }
            }
        }

        //There may be a better way to do this, but I'm not finding it. Sets the note generated.
        noteList[nextNote] = next;
        switch(next) //I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
        {
            case "UU":
                noteSprites[nextNote].setSprite(UU);
                noteSprites[nextNote].setStart(new Vector2(14f, 0.32f));
                break;
            case "UR":
                noteSprites[nextNote].setSprite(UR);
                noteSprites[nextNote].setStart(new Vector2(14f, 0.27f));
                break;
            case "UL":
                noteSprites[nextNote].setSprite(UL);
                noteSprites[nextNote].setStart(new Vector2(14f, 0.22f));
                break;
            case "UD":
                noteSprites[nextNote].setSprite(UD);
                noteSprites[nextNote].setStart(new Vector2(14f, 0.17f));
                break;
            case "RU":
                noteSprites[nextNote].setSprite(RU);
                noteSprites[nextNote].setStart(new Vector2(14f, 0.12f));
                break;
            case "RR":
                noteSprites[nextNote].setSprite(RR);
                noteSprites[nextNote].setStart(new Vector2(14f, 0.07f));
                break;
            case "RL":
                noteSprites[nextNote].setSprite(RL);
                noteSprites[nextNote].setStart(new Vector2(14f, 0.02f));
                break;
            case "RD":
                noteSprites[nextNote].setSprite(RD);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.03f));
                break;
            case "LU":
                noteSprites[nextNote].setSprite(LU);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.08f));
                break;
            case "LR":
                noteSprites[nextNote].setSprite(LR);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.13f));
                break;
            case "LL":
                noteSprites[nextNote].setSprite(LL);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.18f));
                break;
            case "LD":
                noteSprites[nextNote].setSprite(LD);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.23f));
                break;
            case "DU":
                noteSprites[nextNote].setSprite(DU);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.28f));
                break;
            case "DR":
                noteSprites[nextNote].setSprite(DR);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.33f));
                break;
            case "DL":
                noteSprites[nextNote].setSprite(DL);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.38f));
                break;
            case "DD":
                noteSprites[nextNote].setSprite(DD);
                noteSprites[nextNote].setStart(new Vector2(14f, -0.43f));
                break;
        }
        
        nextNote += 1;
        if (nextNote >= 5)
            nextNote = 0;

        if (a && noteSprites[nextNote].getSprite() != null)
        {
            target = noteList[nextNote];
            fret.setSprite(noteSprites[nextNote].getSprite()); //Set the sprite of the fret based on noteList.[nextNote].getSprite()
        }

        //targetText.text = uiT; //Set debug direction indicator
        debugTargetText.text = target;
    }

    public void begin() //Starts the rhythm game. This should ideally be called from a script handling dialogue.
    {
        Start();
        this.transform.localPosition = PointOnCircle(((2 * Mathf.PI) * ((script.songPosinBeats % 3) / 3)), r);
        starting = 1;
        sr.enabled = true;
        startTimer = 0;
        startScale = 0;
    }

    void end() //Exits the rhythm game and resets stats
    {
        fret.endReset();

        starting = 0;
        startScale = 0;

        r = 1;
        speed = .1f;
        nextNote = 0;
        primed = false;
        keyed = false;
        primeCool = 0;
        pcN = 0;
        cooldown = 0;
        inZone = false;
        hit = false;
        miss = false;

        keys = "";
        wasdK = "";
        arrowK = "";
        target = song1[0];

        sr.color = defaultCol;
        script.stopMusic();
        for (int x = 0; x < noteSprites.Length; x++)
        {
            noteSprites[x].stopMotion();
        }
        phase = 0;
        pattern = true;
        currentNote = 0;

        sr.enabled = false;
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
