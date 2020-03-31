using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLock : EnvironmentNote
{

    //The three gameobjects that make up the lock. Should be blocking the player's path, and have an image of their note on them somewhere
    public GameObject lock1;
    public GameObject lock2;
    public GameObject lock3;

    //The positions of the locks. Used to reset their positions.
    public Vector2 pos1;
    public Vector2 pos2;
    public Vector2 pos3;
    

    //The notes that open the respective locks. Stored as a string, for example W and Up Arrow would be "UU", A and Right Arrow would be "LR"
    public string code1;
    public string code2;
    public string code3;

    //If all three locks are open. Keeps the gate open.
    bool open = false;

    //The current lock and code that has to be entered to continue opening the gate.
    int active = 1;
    string activeCode;

    //How much a lock moves when opened, in the y direction
    float moveAmount = 5;

    // Start is called before the first frame update
    void Start()
    {
        pos1 = lock1.transform.localPosition;
        pos2 = lock2.transform.localPosition;
        pos3 = lock3.transform.localPosition;

        activeCode = code1;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public override void keyed(bool correct) //Called when notes are pressed
    {
        if (!open)
        {
            if (correct) //Correct note, open active lock and make next lock active
            {
                if (active == 1)
                {
                    lock1.transform.position = new Vector2(lock1.transform.position.x, lock1.transform.position.y + moveAmount);
                    active = 2;
                    activeCode = code2;
                }
                else if (active == 2)
                {
                    lock2.transform.position = new Vector2(lock2.transform.position.x, lock2.transform.position.y + moveAmount);
                    active = 3;
                    activeCode = code3;
                }
                else if (active == 3) //If all locks are open, keep the gate open
                {
                    lock3.transform.position = new Vector2(lock3.transform.position.x, lock3.transform.position.y + moveAmount);
                    open = true;
                }
            }
            else //Wrong note pressed, reset gate
            {
                active = 1;
                activeCode = code1;
                lock1.transform.localPosition = pos1;
                lock2.transform.localPosition = pos2;
                lock3.transform.localPosition = pos3;
            }
        }
    }

    public override string getCode() //Getter for the code
    {
        return activeCode;
    }

    public override void inRange(bool on) //Locks will light up if in range
    {
        if(on)
        {
            lock1.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, 1f);
            lock2.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, 1f);
            lock3.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, 1f);
        }
        else
        {
            lock1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            lock2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            lock3.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        }
    }
}
