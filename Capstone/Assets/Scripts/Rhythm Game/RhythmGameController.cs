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
    string[] scriptedNotes = { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD" };


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

    public SimpleClock simpleClockScript;   //another script that is attached to the same object (not a child)
                                            //SimpleClock manages the song itself (beats, measures, playing it, etc...)

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

    void Start()
    {
        rhythmGameStateMachine = new FiniteStateMachine<RhythmGameController>(this);
        rhythmGameStateMachine.TransitionTo<LoadRhythmGame>();

        simpleClockScript = gameObject.GetComponent<SimpleClock>();
        //phase2Script = gameObject.GetComponent<Phase2>();


        GetNotes();

        //phase2Script.thisSong = this.thisSong;

        //noteLaunchCoroutine = StartNoteMovement();

        fret = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        rhythmGameStateMachine.Update();

        // if (currPhase == Phase.IntroAnimation)
        // {
        //     IntroAnimation();
        // }
        // else if (currPhase == Phase.Phase2)
        // {
        //     Debug.Log("intro animation completed, moving onto phase2");
        //     phase2Script.enabled = true;
        // }

        Test();

        PressedKeyCheck();
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

    //Coroutine for telling each note when it's time to start moving! 
    // IEnumerator StartNoteMovement()
    // {
    //     for (int i = 0; i < thisSong.GetLength(0); i++)
    //     {
    //         for (int j = 0; j < thisSong.GetLength(1); i++)
    //         {
    //             StartCoroutine(thisSong[i, j].gameObject.GetComponent<NewNote>().WaitAndMove(5f));

    //             yield return new WaitForSeconds(3f);
    //         }
    //     }
    // }

    //check input from the player
    void PressedKeyCheck()
    {
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
    }

    //have a function that looks at current beat and measure and returns the expected key combination
    //then have another function that checks if it was correct or not 
    private string GetExpectedCombination()
    {
        string expectedCombo = "";

        int currMeasure = simpleClockScript.Measures;
        int currBeat = simpleClockScript.Beats;

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

    public IEnumerator StartNoteMovement()
    {
        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); i++)
            {
                StartCoroutine(thisSong[i, j].gameObject.GetComponent<NewNote>().WaitAndMove(5f));

                yield return new WaitForSeconds(3f);
            }
        }
    }

    //the fret will also be changing its sprite as the rhythm game goes
    //string[] scriptedNotes = { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD" };
    public IEnumerator SetFret()
    {
        for (int i = 0; i < scriptedNotes.Length; i++)
        {
            //Debug.Log(scriptedNotes[i]);

            fret.gameObject.GetComponent<NewFretFeedback>().SetSprite(GetSprite(scriptedNotes[i]));

            yield return new WaitForSeconds(2f);
        }
    }

    private Sprite GetSprite(string combination)
    {
        switch (combination)
        {
            case "UU":
                return UU;
                break;
            case "UR":
                return UR;
                break;
            case "UL":
                return UL;
                break;
            case "UD":
                return UD;
                break;
            case "RU":
                return RU;
                break;
            case "RR":
                return RR;
                break;
            case "RL":
                return RL;
                break;
            case "RD":
                return RD;
                break;
            case "LU":
                return LU;
                break;
            case "LR":
                return LR;
                break;
            case "LL":
                return LL;
                break;
            case "LD":
                return LD;
                break;
            case "DU":
                return DU;
                break;
            case "DR":
                return DR;
                break;
            case "DL":
                return DL;
                break;
            case "DD":
                return DD;
                break;
        }

        return null;
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
            Context.StartCoroutine(Context.fret.gameObject.GetComponent<NewFretFeedback>().ScaleFret(3f));
        }

        public override void Update()
        {
            //TransitionTo<Phase2>();
            TransitionTo<Phase1>();
        }

        public override void OnExit()
        {

        }
    }

    //phase 1 of rhythm game--press an x# of notes 
    private class Phase1 : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            Debug.Log("Entering phase 1");
            Context.StartCoroutine(Context.SetFret());
        }   

        public override void Update()
        {
            TransitionTo<Phase2>();
        }

        public override void OnExit()
        {

        }
    }

    //phase 2--notes will move across the screen and player has to hit them at the right time.
    private class Phase2 : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            Debug.Log("Entering phase2 state");
            Context.StartCoroutine(Context.StartNoteMovement());
        }

        public override void Update()
        {

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


