﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public class NoteBlock : MonoBehaviour
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

    private MaterialManager matManager;

    private BoxCollider2D col;

    SpriteRenderer noteSprite; //The spriterenderer of the child sprite, which indicates the note.

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        activeCode = code1;
        noteSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        matManager = transform.GetChild(0).gameObject.GetComponent<MaterialManager>();
        col = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void keyed() //Called when the block's note is played.
    {
        //For now, just remove it
        col.enabled = false;
        AudioManager.Instance.PlayPuzzleSound("SoftGlassBreak");
        matManager.Dissolve();
        /*
        if (pos == 1)
        {
            transform.GetChild(0).gameObject.GetComponent<SpriteGlowEffect>().OutlineWidth = 0;
            transform.position = new Vector2(transform.position.x + xTransform, transform.position.y + yTransform);
            noteSprite.sprite = note2;
            activeCode = code2;
            AudioManager.Instance.PlayPuzzleSound("SoftGlassBreak");
            pos = 2;
        }
        else if (pos == 2)
        {
            transform.GetChild(0).gameObject.GetComponent<SpriteGlowEffect>().OutlineWidth = 1;
            transform.position = new Vector2(transform.position.x + (xTransform * -1f), transform.position.y + (yTransform * -1f    ));
            noteSprite.sprite = note1;
            activeCode = code1;
            AudioManager.Instance.PlayPuzzleSound("SoftGlassBreak");
            pos = 1;
        }*/
    }

    public void inRange(bool on) //Locks will light up if in range
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

    public string getCode() //Getter for the note code
    {
        return activeCode;
    }
}
