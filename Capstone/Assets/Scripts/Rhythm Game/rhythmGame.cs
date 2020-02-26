using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// this script will control the entire rhythm game: it takes care of key input, checking if a combination was correct, 
/// 
/// </summary>

public class rhythmGame : MonoBehaviour
{
    //the rhythmGame script goes on an empty gameObject that also has the SimpleClock on it.
    //this script monitors events (keys pressed), generating all of the notes

    note[] thisSongsNotes = new note[93];
    string[] thisSongsKeyCombos = new string[93];

    GameObject notePrefab;

    string[] first8Notes = { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD", "LL", "RR" }; 

    SimpleClock simpleClockScript;

    void Awake()
    {
        string thisNotesCombo = "";

        //set the combinations for the first 10 notes
        for (int i = 0; i < 8; i++)
        {
            note newNote = Instansiate(notePrefab);

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

            newNote.setSprite(thisNotesCombo);
            
            thisSongsNotes[i] = newNote;
            thisSongsKeyCombos[i] = thisNotesCombo;
        }
    }

    void Update() {
        PressedKeyCheck();
    }

    //function that handles detecting which keys were pressed
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

    //function that checks
    void CombinationCheck() {

    }
}
