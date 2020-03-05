using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script will control the entire rhythm game: it takes care of key input, checking if a combination was correct, 
/// 
/// </summary>
public class RhythmGameController : MonoBehaviour
{
    //the rhythmGame script goes on an empty gameObject that also has the SimpleClock on it.
    //SimpleClock is a SINGLETON.
    //this script monitors events (keys pressed), generating all of the notes

    FiniteStateMachine<RhythmGameController> rhythmGameStateMachine;

    //the song will be represented in a 2D array. The outer array representing the measure and the inner array 
    //representing the four beats. 
    GameObject[,] thisSong = new GameObject[77, 4];

    public GameObject notePrefab;

    //the first x# of notes for any song will be scripted
    List<string> phase1Notes = new List<string>() { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD" };
    List<string> phase2Notes = new List<string>();
    string[] thisSongSequence;

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

    string keys = ""; //What keys are being pressed
    string wasdK = ""; //Which wasd key
    string arrowK = ""; //Which arrow key
    public string target = "UU"; //What keys the game wants you to press.
                                 // Left is wasd, right is arrow keys
                                 // U : up, D : down, L : left, r : right
                                 // So the default here corresponds to w + right arrow

    bool primed = false; //If one key is pressed. This way it only considers a keypress when you press the second key, in case you aren't pressing both on the same frame.
    bool keyed = false; //If both keys are pressed.
    bool lk = false; //Left key pressed
    bool rk = false; //Right key pressed
    float primeCool = 0f; //How long you can keep one key pressed for before it registers an incorrect hit.
    float pcN = 0.1f; //Default value for pcN

    string expectedCombo;   //what the correct combination for any given beat in a song is
    bool combinationCheck;  //manage if the keys input by the player match the expectedCombo

    GameObject fret;    //fret feedback object

    //booleans to control moving between states of the rhythm game
    bool inPhase1 = false;
    bool inPhase2 = false;

    private bool inWindow = true;
    private bool justLeftWindow = false;
    private bool correct = false;


    public SimpleClock simpleClockScript;   //another script that is attached to the same object (not a child)
                                            //SimpleClock manages the song itself (beats, measures, playing it, etc...)


    public int currMeasure;
    public int currBeat;
    public int currTick;

    public NewFretFeedback fretFeedbackScript;

    void Start()
    {
        rhythmGameStateMachine = new FiniteStateMachine<RhythmGameController>(this);
        rhythmGameStateMachine.TransitionTo<LoadRhythmGame>();

        simpleClockScript = gameObject.GetComponent<SimpleClock>();

        GetNotes();
        SetThisSongSequence(phase1Notes, phase2Notes);

        fret = transform.GetChild(0).gameObject;
        fretFeedbackScript = fret.gameObject.GetComponent<NewFretFeedback>();

        fretFeedbackScript.SetPhase1Sequence(phase1Notes);
        fretFeedbackScript.SetPhase2Sequence(phase2Notes);

        fretFeedbackScript.SetSong(phase1Notes, phase2Notes);
    }

    // Update is called once per frame
    void Update()
    {
        //have to constantly know what measure, beat and tick we're on
        currMeasure = simpleClockScript.Measures;
        currBeat = simpleClockScript.Beats;
        currTick = simpleClockScript.Ticks;

        //this will call the Update function of whatever state it is in. 
        rhythmGameStateMachine.Update();

        //PressedKeyCheck();
        expectedCombo = GetExpectedCombination();

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Restarting rhythm game");
            RestartRhythmGame();
        }

        //combinationCheck = CombinationCheck(keys, expectedCombo);
    }

    //initialisation function--all notes for a song are generated at the beginning as to improve
    //performance of the rhythm game as it runs
    private void GetNotes()
    {
        string thisNotesCombo = "";

        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); j++)
            {
                //Starting index 0, we will not be pressing anything anything on the second and fourth beats
                //of a measure, so just put null at that space
                if (j == 1 || j == 3)
                {
                    thisSong[i, j] = null;
                }
                //we only care about the first and third beats of a measure.
                else
                {
                    GameObject newNote = Instantiate(notePrefab);
                    NewNote newNoteScript = newNote.gameObject.GetComponent<NewNote>();

                    for (int y = 0; y < 2; y++) //Do this twice
                    {
                        int x = Random.Range(0, 4); //Randomly assign up, down, left, or right
                        if (x == 0)
                        {
                            thisNotesCombo += "U";
                        }
                        else if (x == 1)
                        {
                            thisNotesCombo += "L";
                        }
                        else if (x == 2)
                        {
                            thisNotesCombo += "D";
                        }
                        else if (x == 3)
                        {
                            thisNotesCombo += "R";
                        }
                    }

                    phase2Notes.Add(thisNotesCombo);

                    //set the properties of each note 
                    newNoteScript.SetBeat(j);
                    newNoteScript.SetMeasure(i);
                    newNoteScript.SetSprite(thisNotesCombo, transform.position);

                    //add the note to the 2D array, the combination to the correct combination list
                    thisSong[i, j] = newNote;
                    //thisSongsKeyCombos[i] = thisNotesCombo;


                    thisNotesCombo = "";
                }
            }
        }
    }

    void SetThisSongSequence(List<string> phase1Notes, List<string> phase2Notes)
    {

        phase1Notes.AddRange(phase2Notes);
        this.thisSongSequence = phase1Notes.ToArray();

        // foreach (string combination in thisSongSequence)
        //     Debug.Log("Setting the song sequence: " + combination);
    }

    //pass function as parameter to this.
    void RhythmGameEventHandler()
    {
        inWindow = CheckWindow();
        string pressedCombo = GetArrowKeys() + GetWASD();
        string expectedCombo = GetExpectedCombination();

        if (inWindow)
        {
            //if the user hit two keys, they are forced out of the window.
            if (pressedCombo.Length == 2)
            {
                inWindow = false;
            }
        }
        else if (!inWindow && justLeftWindow)
        {
            Debug.Log("just exited window, processing");
            //register a miss. In phase 1 this means restarting the entire rhythm game
            //                 In phase 2 this means losing a life
        }
        else if (!inWindow)
        {
            correct = CombinationCheck(pressedCombo, expectedCombo);
        }
    }

    //player is expected to hit the first and third beat of every measure. This function will check if 
    //the song is currently in that window and allow player to give input
    private bool CheckWindow()
    {
        //to hit the first beat in the measure, you get ticks 48 ~ 96 of the fourth beat in the PREVIOUS measure
        //along with the first 48 ticks of beat 1
        if (currBeat == 4 || currBeat == 1)
        {
            if ((currBeat == 4 && currTick >= 48) || (currBeat == 1 && currTick <= 48))
            {
                // expectedCombo = GetExpectedCombination();
                // PressedKeyCheck();
                // if (CombinationCheck(keys, expectedCombo)) 
                //     Debug.Log("Correct");
                // else 
                //     Debug.Log("Incorrect");
                return true;
            }
            //perhaps give it a range to check just exiting the window?
            else if (currTick == 49)
            {
                //this will only execute assuming that the player only hit one key or no keys at all during
                //the previous window 
                justLeftWindow = true;
                return false;
            }
            else
                return false;
        }

        //to hit the third beat in the measure, you get ticks 48 ~ 96 of the second beat in the CURRENT measure
        //along with the first 48 ticks of beat 2.
        else if (currBeat == 2 || currBeat == 3)
        {
            if ((currBeat == 2 && currTick >= 48) || (currBeat == 3 && currTick <= 48))
            {
                // expectedCombo = GetExpectedCombination();
                // PressedKeyCheck();
                // if (CombinationCheck(keys, expectedCombo)) 
                //     Debug.Log("Correct");
                // else 
                //     Debug.Log("Incorrect");
                return true;
            }
            else
                return false;
        }

        return false;
    }



    public string GetArrowKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            return "U";
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            return "L";
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            return "D";
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            return "R";

        return "";
    }

    public string GetWASD()
    {
        //Code for pressing keys
        if (Input.GetKeyDown(KeyCode.W))
            return "U";
        else if (Input.GetKeyDown(KeyCode.A))
            return "L";
        else if (Input.GetKeyDown(KeyCode.S))
            return "D";
        else if (Input.GetKeyDown(KeyCode.D))
            return "R";

        return "";
    }

    //have a function that looks at current beat and measure and returns the expected key combination
    //then have another function that checks if it was correct or not 
    private string GetExpectedCombination()
    {
        string expectedCombo = "";
        //Debug.Log("Curr measure: " + currMeasure + "curr beat: " + currBeat);

        //this if statement is because sometimes SimpleClock returns a fifth beat and that was breaking everything
        //also, measures start counting at 2. 0 Measures is the song has not started playing yet. 
        if (currBeat < 5 && currMeasure > 0)
        {
            GameObject posInSong = thisSong[currMeasure, currBeat - 1];

            if (posInSong != null)
            {
                expectedCombo = posInSong.gameObject.GetComponent<NewNote>().GetCombination();
                //Debug.Log(expectedCombo);
            }
        }

        return expectedCombo;
    }

    //check if the keys the player pressed match what the expected combination at the given moment is
    private bool CombinationCheck(string pressedKeys, string expectedCombo)
    {
        if (pressedKeys.Equals(expectedCombo))
            return true;
        else
            return false;
    }


    //utility functions for handling the present state of the rhythm game 
    public void RestartRhythmGame()
    {
        //notes are not destroyed when they reach the goal, they just turn invisible and
        // teleport somewhere irrelevant, so restarting the rhythm game is just resetting their start position
        StopAllCoroutines();

        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); j++)
            {
                //index out of bounds exception somewhere :'( 
                if (thisSong[i, j] != null)
                {
                    NewNote thisNoteScript = thisSong[i, j].gameObject.GetComponent<NewNote>();
                    thisNoteScript.ResetNote(thisNoteScript.GetCombination(), transform.position, false);
                }
            }
        }
    }

    //used for starting the notes moving in phase 2
    public IEnumerator StartNoteMovement()
    {
        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); i++)
            {
                StartCoroutine(thisSong[i, j].gameObject.GetComponent<NewNote>().WaitAndMove(0f));
                Debug.Log("i value is: " + i + "j value is: " + j);

                //this will have to be changed to get the notes to launch at the right moment
                yield return new WaitForSeconds(SimpleClock.QuarterLength());
            }
        }
    }

    //miscellaneous debugging functions! 

    //Debug function to see what is populating the 2D array
    void Print2DArray()
    {
        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); j++)
            {
                Debug.Log("measure: " + i + " beat: " + thisSong[i, j]);
            }
        }
    }

    //debugging function to test starting note movement
    public void Test()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Entering phase one, calling coroutine");

            //StartCoroutine(noteLaunchCoroutine);
        }
    }


    // IEnumerator KeyPressCoolDown() {
    //     while (true) {
    //         canPress = false;
    //         yield return new WaitForSeconds(2f);
    //         canPress = true;
    //     }
    // }




    //State machine
    //before the rhythm game. This is preparation for loading up the rhythm game and leading into the animation      
    private class LoadRhythmGame : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            //accessing variables that are part of rhythm game controller
            //Context.thisSong;
            Debug.Log("Entering LoadRhythmGame state");
        }

        public override void Update()
        {
            //call transitionto, then onExit will run, then onenter for whatever transitionto will be run
            if (Input.GetKeyDown(KeyCode.P))
            {
                TransitionTo<IntroAnimation>();
            }
        }

        public override void OnExit()
        {

        }
    }

    //intro animation for the rhythm game. Player input cannot be taken.
    private class IntroAnimation : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            Debug.Log("Scaling the fret");
            Context.StartCoroutine(Context.fretFeedbackScript.ScaleFret(3f));
        }

        public override void Update()
        {
            //GetScaleStatus will report when the fret has finished scaling to its final size. Once there, move on 
            //to phase 1 of the game.
            Context.inPhase1 = Context.fretFeedbackScript.GetScaleStatus();
            if (Context.inPhase1)
            {
                TransitionTo<Phase1>();
            }
        }

        public override void OnExit()
        {

        }
    }

    //phase 1 of rhythm game--press an x# of notes 
    private class Phase1 : FiniteStateMachine<RhythmGameController>.State
    {
        private bool firstHit;
        private int i = 0;
        string pressed = "";

        string[] firstNotes = new string[] {"U", "D", "L", "R", "U", "L", "U", "D"};

        public override void OnEnter()
        {
            Debug.Log("Entering phase 1");
            firstHit = false;
        }

        public override void Update()
        {
            // pressed = Context.GetArrowKeys();
            // Debug.Log("This was just pressed: " + pressed + " and I was expecting : " + firstNotes[i]);

            // if (i == 0 && pressed.Equals(firstNotes[i])) {
            //     i++;
            //     Debug.Log("Correct first input");
            //     StartRhythmGame();
            // }
            // else if (pressed.Equals(firstNotes[i])) {
            //     i++;
            //     Debug.Log("Correct input number" + i);
            // }
            //hit the first note to start the rhythm game and call FirstBeat() in SimpleClock
            // while (!firstHit) {

            // }
            //Context.RhythmGameEventHandler();

            // if (Input.GetKeyDown(KeyCode.Y)) {
            //     StartRhythmGame();
            // }

            // if (firstHit) {
            //     Context.StartCoroutine(Context.fretFeedbackScript.SetFret());
            // }
            // Context.inPhase2 = Context.fretFeedbackScript.GetPhase1Status();
            // Context.CheckWindow();
            // if (Context.inPhase2)
            // {
            //     TransitionTo<Phase2>();
            // }
        }

        private bool CheckWindow()
    {
        //to hit the first beat in the measure, you get ticks 48 ~ 96 of the fourth beat in the PREVIOUS measure
        //along with the first 48 ticks of beat 1
        if (currBeat == 4 || currBeat == 1)
        {
            if ((currBeat == 4 && currTick >= 48) || (currBeat == 1 && currTick <= 48))
            {
                // expectedCombo = GetExpectedCombination();
                // PressedKeyCheck();
                // if (CombinationCheck(keys, expectedCombo)) 
                //     Debug.Log("Correct");
                // else 
                //     Debug.Log("Incorrect");
                return true;
            }
            //perhaps give it a range to check just exiting the window?
            else if (currTick == 49)
            {
                //this will only execute assuming that the player only hit one key or no keys at all during
                //the previous window 
                justLeftWindow = true;
                return false;
            }
            else
                return false;
        }

        //to hit the third beat in the measure, you get ticks 48 ~ 96 of the second beat in the CURRENT measure
        //along with the first 48 ticks of beat 2.
        else if (currBeat == 2 || currBeat == 3)
        {
            if ((currBeat == 2 && currTick >= 48) || (currBeat == 3 && currTick <= 48))
            {
                // expectedCombo = GetExpectedCombination();
                // PressedKeyCheck();
                // if (CombinationCheck(keys, expectedCombo)) 
                //     Debug.Log("Correct");
                // else 
                //     Debug.Log("Incorrect");
                return true;
            }
            else
                return false;
        }

        return false;
    }

        private void StartRhythmGame()
        {
            Debug.Log("Starting the rhythm game");
            Context.simpleClockScript.FirstBeat();
            Context.StartCoroutine(Context.fretFeedbackScript.SetFret());
        }

        public override void OnExit()
        {

        }
    }

    //phase 2--notes will move across the screen and player has to hit them at the right time.
    private class Phase2 : FiniteStateMachine<RhythmGameController>.State
    {
        //phase 2 has strikes. The player gets to mess up 4 times and on the 5th, the game restarts
        private int strikes = 0;
        public GameObject lifeSprite1;
        public GameObject lifeSprite2;
        public GameObject lifeSprite3;
        public GameObject lifeSprite4;
        public GameObject lifeSprite5;
        GameObject[] lifeSprites;


        public override void OnEnter()
        {
            // lifeSprites = new GameObject[] {lifeSprite1, lifeSprite2, lifeSprite3, lifeSprite4, lifeSprite5};
            // foreach (GameObject lifeSprite in lifeSprites) {
            //     lifeSprite.SetActive(true);
            // }

            Debug.Log("Entering phase2 state");
            Context.StartCoroutine(Context.StartNoteMovement());
        }

        public override void Update()
        {
            Context.RhythmGameEventHandler();
            Context.CheckWindow();
            //StrikeCheck();
        }

        private void StrikeCheck()
        {
            if (strikes > 0)
            {
                lifeSprite5.SetActive(false);
            }
            if (strikes > 1)
            {
                lifeSprite4.SetActive(false);
            }
            if (strikes > 2)
            {
                lifeSprite3.SetActive(false);
            }
            if (strikes > 3)
            {
                lifeSprite2.SetActive(false);
            }
            if (strikes > 4)
            {
                lifeSprite1.SetActive(false);
            }
        }

        public override void OnExit()
        {

        }
    }

    private class ClosingAnimation : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            Debug.Log("Start of Phase2 script, launching the note movements)");
        }
        public override void Update()
        {

        }

        public override void OnExit()
        {

        }
    }
}


