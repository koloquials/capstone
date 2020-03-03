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

    //enum to handle the phases of the rhythm game
    public enum Phase
    {
        IntroAnimation,
        Phase1,
        Phase2,
        ClosingAnimation,
        RestartAnimation,
    }

    //the song will be represented in a 2D array. The outer array representing the measure and the inner array 
    //representing the four beats. 
    GameObject[,] thisSong = new GameObject[77, 4];

    public GameObject notePrefab;

    //the first x# of notes for any song will be scripted
    string[] scriptedNotes = { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD" };

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

    IEnumerator noteLaunchCoroutine;

    GameObject fret;    //fret feedback object

    private Phase currPhase;    //enum to manage the states of the rhythm game

    void Start()
    {
        currPhase = Phase.IntroAnimation;

        simpleClockScript = gameObject.GetComponent<SimpleClock>();
        GetNotes();

        noteLaunchCoroutine = StartNoteMovement();

        fret = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (currPhase == Phase.IntroAnimation)
        {
            IntroAnimation();
        }
        else if (currPhase == Phase.Phase1)
        {
            Phase1();
        }

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
    IEnumerator StartNoteMovement()
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

    public void IntroAnimation()
    {
        Debug.Log("Scaling the fret");
        fret.gameObject.GetComponent<fretFeedback>();
    }

    //first phase of the rhythm game--the notes are not moving, just pressing the correct combination
    //as it appears on the fret
    public void Phase1()
    {

    }

    public void Phase2()
    {

    }

    public void ClosingAnimation() {

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

            StartCoroutine(noteLaunchCoroutine);
        }
    }
}
