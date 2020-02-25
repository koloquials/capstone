using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public class note : MonoBehaviour
{
    SpriteRenderer sr;

    Vector3 destination;
    Vector3 start;
    bool moving = false; //Currently actually means if the note is visible
    float motionTimer = 0;

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

        transform.position = new Vector3(Mathf.Lerp(start.x, destination.x, motionTimer/((60/BPM)*15)), transform.position.y, transform.position.z);
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

    public void IncreaseBrightness() {
        spriteGlowScript.GlowBrightness = Mathf.Lerp(1.5f, 3f, (Vector2.Distance(transform.position, destination)));
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

    public void Pressed() {
        spriteGlowScript.GlowBrightness += 1f;
    }
}
