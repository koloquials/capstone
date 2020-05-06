using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldRhythm : MonoBehaviour

{
    //A more basic version of the rhythm game, used for environmental puzzles. Right now it doesn't care about rhythm, just which note you press.

    //public ExampleVariableStorage varStor; //The example variable storage for the yarn system. Used to tell dialogue when you've won.

    //SHOULD BE ON A CHILD OF THE PLAYER, NOT THE PLAYER ITSELF

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

    SpriteRenderer sr;

    float cooldown = 0f; //The cooldown for inputting keys. Prevents the game from considering a hit to be wrong because the player doesn't release the keys frame-perfectly.
    float cN = 0.2f; //The cooldown amount. Put here for ease of testing.

    string keys = ""; //What keys are being pressed
    string wasdK = ""; //Which wasd key
    string arrowK = ""; //Which arrow key

    bool primed = false; //If one key is pressed. This way it only considers a keypress when you press the second key, in case you aren't pressing both on the same frame.
    bool keyed = false; //If both keys are pressed.
    bool lk = false; //Left key pressed
    bool rk = false; //Right key pressed
    float primeCool = 0f; //How long you can keep one key pressed for before it registers an incorrect hit.
    float pcN = 0.1f; //Default value for pcNS

    bool active = false; //Whether the environment rhythm is active and tracking keypresses.

    EnvironmentController ec; //The environment controller, which contains references to all the interactable objects

    //Probably a bad way of doing this. Colors for the note to make it visible or not
    Color invis;
    Color vis;

    bool beat = false; //Wait a frame when turning on or off, prevents some issues.

    GameObject player;
    Vector3 playerPos;

    List<NoteLock> localLocks;
    List<NoteBlock> localBlocks;
    List<NoteSpinner> localSpinners;

    // Start is called before the first frame update
    void Start()
    {
        localLocks = new List<NoteLock>();
        localBlocks = new List<NoteBlock>();
        localSpinners = new List<NoteSpinner>();
        invis = new Color(1, 1, 1, 0);
        vis = new Color(1, 1, 1, 1);
        sr = GetComponent<SpriteRenderer>();
        ec = FindObjectOfType<EnvironmentController>();
        player = GameObject.Find("Player"); //ONLY WORKS IF THE PLAYER IS NAMED PLAYER. May wish to find a better way of getting a reference to the player
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && !beat && Input.GetKeyDown(KeyCode.R))
        {
            active = true;
            sr.color = vis;
            Debug.Log("Turning on");
            beat = true;
            //playerPos = player.transform.position;
            player.GetComponent<Yarn.Unity.Example.PlayerCharacter>().setMovement(false); //Stops the player from moving

            foreach (NoteLock n in ec.lockList) //Interacting with locks
            {
                if (Vector3.Distance(transform.position, n.transform.position) < 10) //If the note is within a certain range. May wish to edit this to get a good range.
                {
                    localLocks.Add(n);
                    n.inRange(true);
                }
            }
            foreach (NoteBlock n in ec.blockList) //Interacting with locks
            {
                if (Vector3.Distance(transform.position, n.transform.position) < 10) //If the note is within a certain range. May wish to edit this to get a good range.
                {
                    localBlocks.Add(n);
                    n.inRange(true);
                }
            }
            foreach (NoteSpinner n in ec.spinnerList)
            {
                if (Vector3.Distance(transform.position, n.transform.position) < 10) //If the note is within a certain range. May wish to edit this to get a good range.
                {
                    localSpinners.Add(n);
                    //n.inRange(true);
                    //Right now there isn't a highlight for spinners in range
                }
            }
        }

        if (active)
        {
            //player.transform.position = playerPos; //Old method of locking position. 

            if (!beat && Input.GetKeyDown(KeyCode.R)) //Closes the rhythm game.
            {
                active = false;
                sr.color = invis;
                Debug.Log("Turning off");
                beat = true;
                foreach(NoteLock n in localLocks)
                {
                    n.inRange(false);
                }
                foreach(NoteBlock n in localBlocks)
                {
                    n.inRange(false);
                }
                localLocks.Clear();
                localBlocks.Clear();
                localSpinners.Clear();
                player.GetComponent<Yarn.Unity.Example.PlayerCharacter>().setMovement(true);
            }

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
                pressedKey();
                cooldown = cN;
                primed = false;
                keyed = false;
                wasdK = "";
                arrowK = "";
            }

            cooldown -= Time.deltaTime;

            if (primed) //If one note has been pressed, trigger a miss if no other note is pressed within 0.1 seconds
            {
                if (primeCool <= 0)
                {
                    primed = false;
                    lk = false;
                    rk = false;
                    wasdK = "";
                    arrowK = "";
                }
                primeCool -= Time.deltaTime;
            }
        }
        beat = false;

    }

    void pressedKey()
    {
        switch (keys) //I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
        {
            case "UU":
                sr.sprite = UU;
                break;
            case "UR":
                sr.sprite = UR;
                break;
            case "UL":
                sr.sprite = UL;
                break;
            case "UD":
                sr.sprite = UD;
                break;
            case "RU":
                sr.sprite = RU;
                break;
            case "RR":
                sr.sprite = RR;
                break;
            case "RL":
                sr.sprite = RL;
                break;
            case "RD":
                sr.sprite = RD;
                break;
            case "LU":
                sr.sprite = LU;
                break;
            case "LR":
                sr.sprite = LR;
                break;
            case "LL":
                sr.sprite = LL;
                break;
            case "LD":
                sr.sprite = LD;
                break;
            case "DU":
                sr.sprite = DU;
                break;
            case "DR":
                sr.sprite = DR;
                break;
            case "DL":
                sr.sprite = DL;
                break;
            case "DD":
                sr.sprite = DD;
                break;
        }

        if (localLocks != null)
        {
            foreach (NoteLock n in localLocks) //Interacting with locks
            {
                if (keys.Equals(n.getCode())) //Interact with the lock based on if the note is correct or not
                {
                    n.opened();
                }
                else
                {
                    n.closed();
                }
            }
        }

        if (localBlocks != null)
        {
            foreach(NoteBlock n in localBlocks)
            {
                if(keys.Equals(n.getCode()))
                {
                    n.keyed();
                }
            }
        }
        
        if(localSpinners != null)
        {
            foreach(NoteSpinner n in localSpinners)
            {
                if(keys.Equals(n.getCode()))
                {
                    n.keyed();
                }
            }
        }

        //Interacting with note blocks
    }
}
