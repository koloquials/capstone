using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public class note : MonoBehaviour
{
    SpriteRenderer sr;

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

    Vector3 destination;
    Vector3 start;
    bool moving = false; //Currently actually means if the note is visible
    float motionTimer = 0;

    string myCombination;


    //note should have a quality on it time should be played 
    //when there's a hit,
    //each note should know when it's supposed to be hit and then check it against the window.
    //each note will have a data type that says what buttons they correspond to and what time they're meant to be played at
    //%4 get remainder, gets the place in the measures

    float BPM; //This many beats per 60 seconds
               //60 divided by BPM is how many seconds per beat, that x15 is how many seconds per beat
               //If it's not then you know that this is what's wrong.

    float scale = 0f;

    SpriteGlowEffect spriteGlowScript;


    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        spriteGlowScript = gameObject.GetComponent<SpriteGlowEffect>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseBrightness();

        transform.position = new Vector3(Mathf.Lerp(start.x, destination.x, motionTimer / ((60 / BPM) * 15)), transform.position.y, transform.position.z);
        motionTimer += Time.deltaTime;

        if (moving)
        {
            if (scale < 1)
            {
                scale = Mathf.Lerp(scale, 1, 0.05f);
                if (scale > 0.98f)
                {
                    scale = 1f;
                }
                transform.localScale = new Vector2(scale, scale);
            }
        }
    }

    public void IncreaseBrightness()
    {
        spriteGlowScript.GlowBrightness = Mathf.Lerp(1.5f, 3f, (Vector2.Distance(transform.position, destination)));
    }

    public void setSprite(string thisNotesCombo)
    {
        switch (thisNotesCombo)
        {//I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
            case "UU":
                sr.sprite = UU;
                setStart(new Vector2(14f, 0.32f));
                break;
            case "UR":
                sr.sprite = UR;
                setStart(new Vector2(14f, 0.27f));
                break;
            case "UL":
                sr.sprite = UL;
                setStart(new Vector2(14f, 0.22f));
                break;
            case "UD":
                sr.sprite = UD;
                setStart(new Vector2(14f, 0.17f));
                break;
            case "RU":
                sr.sprite = RU;
                setStart(new Vector2(14f, 0.12f));
                break;
            case "RR":
                sr.sprite = RR;
                setStart(new Vector2(14f, 0.07f));
                break;
            case "RL":
                sr.sprite = RL;
                setStart(new Vector2(14f, 0.02f));
                break;
            case "RD":
                sr.sprite = RD;
                setStart(new Vector2(14f, -0.03f));
                break;
            case "LU":
                sr.sprite = LU;
                setStart(new Vector2(14f, -0.08f));
                break;
            case "LR":
                sr.sprite = LR;
                setStart(new Vector2(14f, -0.13f));
                break;
            case "LL":
                sr.sprite = LL;
                setStart(new Vector2(14f, -0.18f));
                break;
            case "LD":
                sr.sprite = LD;
                setStart(new Vector2(14f, -0.23f));
                break;
            case "DU":
                sr.sprite = DU;
                setStart(new Vector2(14f, -0.28f));
                break;
            case "DR":
                sr.sprite = DR;
                setStart(new Vector2(14f, -0.33f));
                break;
            case "DL":
                sr.sprite = DL;
                setStart(new Vector2(14f, -0.38f));
                break;
            case "DD":
                sr.sprite = DD;
                setStart(new Vector2(14f, -0.43f));
                break;
        }
    }
}

public void stopMotion() //THE NAMES ARE MISLEADING: The notes work better if they're always moving but invisible when you're not supposed to see them. Currently changes sprite alpha.
{
    moving = false;
    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
}

public void startMotion()
{
    moving = true;
    transform.localScale = Vector2.zero;
    scale = 0;
    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
}

public void setBPM(float bpm)
{
    BPM = bpm;
}

public void setStart(Vector2 s)
{
    start = new Vector3(s.x, s.y, transform.position.z);
    transform.position = start;
    motionTimer = 0;
}

public void setDestination(Vector2 d)
{
    destination = new Vector3(d.x, d.y, transform.position.z);
}

public Sprite getSprite()
{
    return sr.sprite;
}

public void Pressed()
{
    spriteGlowScript.GlowBrightness += 1f;
}
}
