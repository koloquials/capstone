using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class note : MonoBehaviour
{
    SpriteRenderer sr;

    Vector2 destination;
    Vector2 start;
    bool moving = false; //Currently actually means if the note is visible
    float motionTimer = 0;

    float BPM; //This many beats per 60 seconds
               //60 divided by BPM is how many seconds per beat, that x15 is how many seconds per beat
               //If it's not then you know that this is what's wrong.

    float scale = 0f;


    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(Mathf.Lerp(start.x, destination.x, motionTimer/((60/BPM)*15)), transform.position.y);
        motionTimer += Time.deltaTime;

        if(moving)
        {
            if(scale < 1)
            {
                scale = Mathf.Lerp(scale, 1, 0.05f);
                if(scale > 0.98f)
                {
                    scale = 1f;
                }
                transform.localScale = new Vector2(scale, scale);
            }
        }
    }

    public void setSprite(Sprite s)
    {
        sr.sprite = s;
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
        start = s;
        transform.position = start;
        motionTimer = 0;
    }

    public void setDestination(Vector2 d)
    {
        destination = d;
    }

    public Sprite getSprite()
    {
        return sr.sprite;
    }
}
