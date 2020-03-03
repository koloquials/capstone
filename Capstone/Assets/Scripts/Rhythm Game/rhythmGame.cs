using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script will control the entire rhythm game: it takes care of key input, checking if a combination was correct, 
/// 
/// </summary>
public class RhythmGame : MonoBehaviour
{
    //the rhythmGame script goes on an empty gameObject that also has the SimpleClock on it.
    //SimpleClock is a SINGLETON.
    //this script monitors events (keys pressed), generating all of the notes

    //total number of notes to press in this song
    GameObject[] thisSongsNotes = new GameObject[93];
    string[] thisSongsKeyCombos = new string[93];

    //the song will be represented in a 2D array. The outer array representing the measure and the inner array 
    //representing the four beats. 
    GameObject[,] thisSong = new GameObject[77, 4];

    public GameObject notePrefab;

    string[] first8Notes = { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD" };

    public SimpleClock simpleClockScript;

    private int beatVal;
    private int measureVal;


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

    string expectedCombo;
    bool combinationCheck;

    void Start()
    {
        simpleClockScript = gameObject.GetComponent<SimpleClock>();
        GetNotes();

        StartCoroutine(StartNoteMovement());
    }

    // Update is called once per frame
    void Update()
    {
        PressedKeyCheck();
        expectedCombo = ExpectedCombination();
        //combinationCheck = CombinationCheck(keys, expectedCombo);
    }

    private void GetNotes()
    {
        string thisNotesCombo = "";

        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); j++)
            {
                if (j == 1 || j == 3)
                {
                    thisSong[i, j] = null;
                }
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
                    //set the beat
                    //each note will either be the first or third beat in a given measure, so there are only 
                    //two cases that need to be taken care of. 
                    //all even indices in the array are the first beat, all odd are the third beat.

                    newNoteScript.SetBeat(j);

                    //set which beat in the measure it was
                    //at the moment, we are only considering the first and third beat in each measure


                    //set the measure
                    measureVal = i;
                    newNoteScript.SetMeasure(measureVal);

                    //set the combination sprite
                    newNoteScript.SetSprite(thisNotesCombo, transform.position);

                    //set the starting position

                    //add the note to the note list, the combination to the correct combination list
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
    private string ExpectedCombination()
    {
        string expectedCombo = "";

        int currMeasure = simpleClockScript.Measures;
        int currBeat = simpleClockScript.Beats;

        Debug.Log("Curr measure: " + currMeasure + "curr beat: " + currBeat);

        //this is causing some kind of out of bounds error? 
        //GameObject posInSong = thisSong[currMeasure, currBeat];
        
        // if (posInSong != null) {
        //     expectedCombo = posInSong.gameObject.GetComponent<NewNote>().GetCombination();
        // }

        return expectedCombo;
    }

    private bool CombinationCheck(string pressedKeys, string expectedCombo) {
        if (pressedKeys.Equals(expectedCombo)) 
            return true;
        else 
            return false;
    }

    //first phase of the rhythm game--the notes are not moving, just pressing the correct combination
    //as it appears on the fret
    public void Phase1()
    {

    }

    //second phase of the rhythm game--notes start moving in from off-screen
    // public void Phase2()
    // {
    //     StartCoroutine(StartNoteMovement);
    // }



    //miscellaneous debugging functions! 

    //Debug function to see what is populating the 2D array
    void Print2DArray() {
        for (int i = 0; i < thisSong.GetLength(0); i++) {
            for (int j = 0; j < thisSong.GetLength(1); j++) {
                Debug.Log("measure: " + i + " beat: " + thisSong[i, j]);
            }
        }
    }
}
