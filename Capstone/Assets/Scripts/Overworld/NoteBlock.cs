using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBlock : EnvironmentNote
{
    //The note codes for the different positions.
    public string code1;
    public string code2;
    string activeCode;

    //The sprites for the different positions. Should correspond to the codes.
    public Sprite note1;
    public Sprite note2;

    //How much the note block moves between positions
    public float xTransform;
    public float yTransform;

    int pos = 1; //What position the note is in. In theory, could go above 2, but right now it's just 1 or 2.

    SpriteRenderer sr;

    SpriteRenderer noteSprite; //The spriterenderer of the child sprite, which indicates the note.

    Vector3 position1; //The vector coordinates for the note block positions
    Vector3 position2;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        activeCode = code1;
        noteSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        position1 = transform.position; //Set positions 1 and 2
        position2 = new Vector3(position1.x + xTransform, position1.y + yTransform, position1.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(pos == 1 && Vector3.Distance(transform.position, position1) > 0.001)
        {
            transform.position = Vector3.Lerp(transform.position, position1, 0.1f);
        }
        if (pos == 2 && Vector3.Distance(transform.position, position2) > 0.001)
        {
            transform.position = Vector3.Lerp(transform.position, position2, 0.1f);
        }
    }

    public override void keyed(bool correct) //Called when the block's note is played.
    {
        if (correct)
        {
            if (pos == 1)
            {
                //transform.position = new Vector2(transform.position.x + xTransform, transform.position.y + yTransform);
                noteSprite.sprite = note2;
                activeCode = code2;
                pos = 2;
            }
            else if (pos == 2)
            {
                //transform.position = new Vector2(transform.position.x + (xTransform * -1f), transform.position.y + (yTransform * -1f));
                noteSprite.sprite = note1;
                activeCode = code1;
                pos = 1;
            }
        }
    }

    public override void inRange(bool on) //Locks will light up if in range
    {
        if (on)
        {
            sr.color = new Color(.5f, .5f, 1f);
        }
        else
        {
            sr.color = new Color(1f, 1f, 1f);
        }
    }

    public override string getCode() //Getter for the note code
    {
        return activeCode;
    }
}
