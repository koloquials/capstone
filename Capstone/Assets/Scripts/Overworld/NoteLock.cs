using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLock : MonoBehaviour
{

    //The three gameobjects that make up the lock. Should be blocking the player's path, and have an image of their note on them somewhere
    public GameObject lock1;
    public GameObject lock2;
    public GameObject lock3;

    Collider2D col1;
    Collider2D col2;
    Collider2D col3;

    MaterialManager mat1;
    MaterialManager mat2;
    MaterialManager mat3;

    SpriteRenderer sr1;
    SpriteRenderer sr2;

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

        col1 = lock1.GetComponent<Collider2D>();
        col2 = lock2.GetComponent<Collider2D>();
        col3 = lock3.GetComponent<Collider2D>();

        mat1 = lock1.GetComponent<MaterialManager>();
        mat2 = lock2.GetComponent<MaterialManager>();
        mat3 = lock3.GetComponent<MaterialManager>();

        sr1 = lock1.GetComponent<SpriteRenderer>(); //Only the first two need references to the spriterenderer, since once the third is activated the lock dissolves
        sr2 = lock2.GetComponent<SpriteRenderer>();

        activeCode = code1;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void opened() //Called when the correct note is pressed. Opens the active lock and makes the next one active.
    {
        if (!open)
        {
            if (active == 1)
            {
                sr1.color = new Color(1, 1, 1, 0.3f);
                //lock1.transform.position = new Vector2(lock1.transform.position.x, lock1.transform.position.y + moveAmount);
                active = 2;
                activeCode = code2;
                AudioManager.Instance.PlayPuzzleSound("SoftGlassBreak");
            }
            else if (active == 2)
            {
                sr2.color = new Color(1, 1, 1, 0.3f);
                //lock2.transform.position = new Vector2(lock2.transform.position.x, lock2.transform.position.y + moveAmount);
                active = 3;
                activeCode = code3;
                AudioManager.Instance.PlayPuzzleSound("SoftGlassBreak");
            }
            else if (active == 3) //If all locks are open, keep the gate open
            {
                mat1.Dissolve();
                mat2.Dissolve();
                mat3.Dissolve();
                col1.enabled = false;
                col2.enabled = false;
                col3.enabled = false;
                //lock3.transform.position = new Vector2(lock3.transform.position.x, lock3.transform.position.y + moveAmount);
                open = true;
                AudioManager.Instance.PlayPuzzleSound("SoftGlassBreak");
            }
        }
    }

    public void closed() //Called when a wrong note is pressed, resets the gate.
    {
        if (!open)
        {
            active = 1;
            activeCode = code1;
            lock1.transform.localPosition = pos1;
            lock2.transform.localPosition = pos2;
            lock3.transform.localPosition = pos3;
            sr1.color = Color.white;
            sr2.color = Color.white;

        }
    }

    public string getCode() //Getter for the code
    {
        return activeCode;
    }

    public void inRange(bool on) //Locks will light up if in range
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
