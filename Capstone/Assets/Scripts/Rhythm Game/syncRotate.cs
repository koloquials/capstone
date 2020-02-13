using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class syncRotate : MonoBehaviour

{
    //IMPORTANT! The scene should be set up such that the rhythm game part should be at the origin of the scene's worldspace.
    //FOR CLARIFICATION: This script goes on the smaller circle orbiting a point on the screen. The fret is the larger circle intersecting the smaller circle's orbit on the right side, which should have the fretFeedback script.
    //The spriterenderer of both this and the fret should start disabled.

    public ExampleVariableStorage varStor; //The example variable storage for the yarn system. Used to tell dialogue when you've won.

    public conductorScript script;

    public Yarn.Unity.Example.PlayerCharacter pc; //The script for moving the player. Used to stop being able to move and interact during the rhythm game.

    int starting = 0; //Handles entering the rhythm game. 0 is not started, 1 is the little intro animation, 2 is ready to go.

    float startScale = 0f; //Scale of the rotating ball at the start. Used for the intro animation.

    public float maxScale = 0.5f; //Maximum scale of the rotating ball.

    float startTimer = 0f; //Used for the start animation.
    float endTimer = 0f; //Used for the end

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

    public Yarn.Unity.Example.CameraFollow cam; //Camerafollow script, should be attached to the camera.

    public ParticleSystem party;

    SpriteRenderer sr; //The sprite renderer for the note. Currently used to denote phase 3 health.

    public int score; //Currently used to denote how many notes have passed

    private int songNoteLength; //Length of the song, in notes

    public bool inZone = false; //Whether the note is in the zone

    public fretFeedback fret; //The script of the fret, the circle that the player wants to line up notes with.

    float cooldown = 0f; //The cooldown for inputting keys. Prevents the game from considering a hit to be wrong because the player doesn't release the keys frame-perfectly.
    float cN = 0.2f; //The cooldown amount. Put here for ease of testing.

    bool hit = false; //If a note is hit this cycle
    bool miss = false; //If a note is missed this tick

    public Text pressedKeyText; //Debug, what keys are being pressed
    public Text targetText; //Debug feature, what keys are correct
    //public Text debugTargetText; //Debug feature, what keys are correct in the format of pressedKeyText
    public Text phaseText; //Debug feature, what phase we're in
    public Text scoreText; //Debug feature, score count

    string keys = ""; //What keys are being pressed
    string wasdK = ""; //Which wasd key
    string arrowK = ""; //Which arrow key
    public string target = "UU"; //What keys the game wants you to press.
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
    //Any miss during phases 0 or 1 resets to phase 0. Missing three times in phase 2 reverts to phase 0

    bool pattern = true; //Whether the notes should be a preset pattern (true), or random (false)
    string[] song1 = { "UU", "DD", "LL", "RR", "UD", "LR", "UU","DD","LL","RR"}; //For example
    string[] song1direction = { "^ ^","v v","< <", "> >", "^ v", "< >", "^ ^", "v v", "< <", "> >" }; //Direction equivalent of the notes. Ideally find a way to automate making these.
    int song1Max = 10; //How many notes are in this song.
    int currentNote = 0; //Which note of the song is the active one. Once this is equal to the song max, should either reset to 0 or progress the phase.
                         //Nonrandom is on the to-do list.

    int strikes = 0; //5 strikes, you're out. Used in Phase 2.

    public GameObject lifeSprite1; //Sprites for your three lives. Should start deactivated.
    public GameObject lifeSprite2;
    public GameObject lifeSprite3;
    public GameObject lifeSprite4;
    public GameObject lifeSprite5;

    private GameObject[] lifeSprites;



    public GameObject background; //Sprite that covers the overworld while the game is up

    public bool debugMode = false; //Debug mode, currently just lets you start the game with spacebar. Set this in the inspector on a per-scene basis.

    // Start is called before the first frame update
    void Start() {
        lifeSprites = new GameObject[] {lifeSprite1, lifeSprite2, lifeSprite3, lifeSprite4, lifeSprite5};
        transform.localScale = Vector2.zero; //For the start animation

        this.transform.position = new Vector3(0,0,transform.position.z);
        score = 0;
        sr = GetComponent<SpriteRenderer>();

        defaultCol = sr.color;

        songNoteLength = (script.songLenghtinBeats) / 3;

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
            if(debugMode && Input.GetKey(KeyCode.Space)) //Debug Feature, starts the rhythm game. In the future, the rhythm game should be started based on the dialogue script.
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
                    //fret.noteRipple();
                    fret.noteRippleParticles();
                    if (keys.Equals(target)) //If the note is correct, add score and trigger feedback
                    {
                        //score++;
                        //Debug.Log(score);

                        inZone = false;
                        fret.fretHit(true);
                        hit = true;
                        //fret.noteRipple(true);
                    }
                    else //Otherwise trigger negative feedback
                    {
                        fret.fretHit(false);
                        //score--;
                        strikes += 1;
                        miss = true;
                        //fret.noteRipple(false);
                    }
                }
                // else //Timing incorrect, negative feedback
                // {
                //     fret.fretHit(false);
                //     //score--;
                //     miss = true;
                //     strikes += 1;
                // }
                cooldown = cN;
                primed = false;
                keyed = false;
                wasdK = "";
            }

            cooldown -= Time.deltaTime;

            if (primed && inZone) //If one note has been pressed, trigger a miss if no other note is pressed within 0.1 seconds
            {
                if (primeCool <= 0)
                {
                    Debug.Log("Decrementing the score within primed");
                    fret.fretHit(false);
                    primed = false;
                    lk = false;
                    rk = false;
                    wasdK = "";
                    arrowK = "";
                    //score--;
                    //strikes += 1;
                    hit = false;
                }
                primeCool -= Time.deltaTime;
            }

            if(strikes > 0)
            {
                lifeSprite5.SetActive(false);
            }
            if(strikes > 1)
            {
                lifeSprite4.SetActive(false);
            }
            if(strikes > 2)
            {
                lifeSprite3.SetActive(false);
            }
            if(strikes > 3) {
                lifeSprite2.SetActive(false);
            }
            if(strikes > 4) {
                lifeSprite1.SetActive(false);
            }

            phaseCheck();
            phaseText.text = "Phase: " + phase;
            //if (phase == 2)
            //scoreText.text = "Lives: " + (3 - strikes);
            //else
            //scoreText.text = ""+score;
            //scoreText.text = "";
            scoreText.text = "" + score + "/" + songNoteLength;

            if(score >= songNoteLength)
            {
                //You win, pass a value to dialogue
                varStor.SetValue("$completedRhythm", new Yarn.Value(1));
                starting = 3;
                endTimer = 0;
            }

        }
        if(starting == 3) //The end, wait 3 seconds before closing to give the song time to close out.
        {
            this.transform.position = PointOnCircle(0, r);
            if (endTimer > 3)
            {
                end();
            }
            else if(endTimer > 1)
            {
                if (endTimer < 1.1f)
                {
                    foreach (GameObject lifeSprite in lifeSprites) {
                        lifeSprite.SetActive(false);
                    }
                    fret.endingScaling(); //The fret handles it's own scaling process, just has to be told to start scaling down
                }
                startScale = Mathf.Lerp(startScale, 0, 0.1f);
                if (startScale < 0.1f)
                {
                    startScale = 0;
                }
                transform.localScale = new Vector2(startScale, startScale);
            }
            endTimer += Time.deltaTime;
        }
    }

    Vector3 PointOnCircle(float angle, float radius) //Used to move the beat indicator in a circle.
    {
        return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), -0.3f);
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
                Debug.Log("Decrementing the score within trigger check");
                //score--;
                strikes += 1;
                miss = true;
                fret.fretHit(false);
                phaseCheck();
            }
            setTarget(true); //Find a new key to want to press
            miss = false;
            hit = false;
            score++;
        }
    }

    void phaseCheck() //Checks the result of misses and hits on the game's phases.
    {
        if (starting < 3)
        {
            if (phase == 0) //Phase 0, the note stands still until you hit it, starting phase 1
            {
                if (miss)
                {
                    score = 0;
                    pattern = true;
                    currentNote = 0;
                    for (int x = 0; x < 5; x++)
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
                    strikes = 0;
                    pattern = false;
                    //sr.color = Color.blue;
                    foreach (GameObject lifeSprite in lifeSprites) {
                        lifeSprite.SetActive(true);
                    }
                }
            }
            else if (phase == 2) //Phase 2, keep score above 0
            {
                if (strikes >= 5)
                {
                    //sr.color = Color.gray;
                    script.stopMusic();
                    for (int x = 0; x < noteSprites.Length; x++)
                    {
                        noteSprites[x].stopMotion();
                    }
                    //score = 0;
                    strikes = 0;
                    phase = 0;
                    score = 0;
                    pattern = true;
                    currentNote = 0;
                    for (int x = 0; x < 5; x++) {
                        setTarget(true);
                    }
                    
                    foreach (GameObject lifeSprite in lifeSprites) {
                        lifeSprite.SetActive(false);
                    }
                }
                /*else if(score >= 10)
                {
                    sr.color = Color.blue;
                }
                else
                {
                    sr.color = new Color(1f - (score / 10f), 0, 1f);
                }*/
            }
        }
    }

    void setTarget(bool a) //Set a new target key combination. Parameter is if this is being called from start, because of an error on the first time it's called.
    {
        if (starting < 3)
        {
            string next = ""; //Reset the old combination
                              //string uiT = "";
            if (score >= songNoteLength - 5)
            {
                noteSprites[nextNote].stopMotion();
            }
            else
            {
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

                //Sets the note generated.
                noteList[nextNote] = next;
                switch (next) //I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
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
            }

            nextNote += 1;
            if (nextNote >= 5)
                nextNote = 0;

            if (a && noteSprites[nextNote].getSprite() != null && score < songNoteLength-1) //That last bit prevents the fret being changed as the song ends.
            {
                target = noteList[nextNote];
                fret.setSprite(noteSprites[nextNote].getSprite()); //Set the sprite of the fret based on noteList.[nextNote].getSprite()
            }

            targetText.text = target; //Set debug direction indicator
            //debugTargetText.text = target;
        }
    }

    public void begin() //Starts the rhythm game. This should ideally be called from a script handling dialogue.
    {
        background.SetActive(true);
        cam.setGame(true);
        scoreText.gameObject.SetActive(true);
        //targetText.gameObject.SetActive(true);
        phaseText.gameObject.SetActive(true);
        //pressedKeyText.gameObject.SetActive(true);
        Start();
        this.transform.localPosition = PointOnCircle(((2 * Mathf.PI) * ((script.songPosinBeats % 3) / 3)), r);
        starting = 1;
        sr.enabled = true;
        startTimer = 0;
        startScale = 0;
        pc.motionControl(false);
    }

    void end() //Exits the rhythm game and resets stats
    {

        background.SetActive(false);

        fret.endReset();

        scoreText.gameObject.SetActive(false);
        //targetText.gameObject.SetActive(false);
        phaseText.gameObject.SetActive(false);
        //pressedKeyText.gameObject.SetActive(false);

        foreach (GameObject lifeSprite in lifeSprites) {
            lifeSprite.SetActive(false);
        }

        starting = 0;
        startScale = 0;
        score = 0;

        r = 1;
        speed = .1f;
        nextNote = 0;
        primed = false;
        keyed = false;
        primeCool = pcN;
        cooldown = cN;
        inZone = false;
        hit = false;
        miss = false;

        keys = "";
        wasdK = "";
        arrowK = "";
        target = "UU";
        noteSprites[nextNote].setSprite(UU);
        fret.setSprite(noteSprites[nextNote].getSprite());

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

        pc.motionControl(true);
        cam.setGame(false);
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
