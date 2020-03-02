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
    //this script monitors events (keys pressed), generating all of the notes

    //total number of notes to press in this song
    GameObject[] thisSongsNotes = new GameObject[93];
    string[] thisSongsKeyCombos = new string[93];

    public GameObject notePrefab;

    string[] first8Notes = { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD" };

    SimpleClock simpleClockScript;

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

    void Start()
    {
        string thisNotesCombo = "";

        //sets the combination for all notes in the song
        for (int i = 0; i < thisSongsNotes.Length; i++)
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
            if (i % 2 == 0)
            {
                newNoteScript.SetBeat(beatVal);
                beatVal = 3;
            }
            else if (i % 2 == 1)
            {
                newNoteScript.SetBeat(beatVal);
                beatVal = 1;
            }

            //set the measure
            measureVal = i % 4;
            newNoteScript.SetMeasure(measureVal);

            Debug.Log(thisNotesCombo);
            //set the combination sprite
            newNoteScript.SetSprite(thisNotesCombo);

            //set the starting position

            //add the note to the note list, the combination to the correct combination list
            thisSongsNotes[i] = newNote;
            thisSongsKeyCombos[i] = thisNotesCombo;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
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
}
