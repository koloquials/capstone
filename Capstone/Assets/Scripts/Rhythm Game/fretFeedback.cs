using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class fretFeedback : MonoBehaviour
{

    SpriteRenderer sr;

    public GameObject rippleSprite;
    public ParticleSystem rippleParticles;


    //A script for the fret, the circle that the notes line up with. Mostly for feedback.

    float scaleMod = 0f; //Modifies the scale of the fret.
    float sConstant = 1f; //The default scale

    Color fretCol = Color.white; //Color of the fret

    bool startScale = false; //When starting, scale the thing up.
    bool endScale = false; //When ending, scale down.
    bool started = false; //When the rhythm game is actually running

    AudioSource tambourineSoundSrc;
    
    // Start is called before the first frame update
    void Start()
    {
        tambourineSoundSrc = gameObject.GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();

        transform.localScale = Vector2.zero; //For the start animation
    }

    // Update is called once per frame
    void Update()
    {
        if (startScale)
        {
            scaleMod = Mathf.Lerp(scaleMod, sConstant, 0.04f);
            if (scaleMod >= 1f)
            {
                scaleMod = 1;
                startScale = false;
                started = true;
            }
            transform.localScale = new Vector2(scaleMod*sConstant, scaleMod*sConstant);
        }
        else if (endScale)
        {
            scaleMod = Mathf.Lerp(scaleMod, 0, 0.06f);
            if (scaleMod < 0.1f)
            {
                scaleMod = 0;
            }
            transform.localScale = new Vector2(scaleMod * sConstant, scaleMod * sConstant);
        }
        else if (started)
        {
            transform.localScale = new Vector2(scaleMod * sConstant, scaleMod * sConstant); //Set the scale of the fret
            if (scaleMod > 1.01f || scaleMod < 0.99f) //If the scaleMod isn't 1, return to 1
            {
                scaleMod = Mathf.Lerp(scaleMod, 1, .2f);
            }
            else //If the size is close to normal, reset the color
            {
                sr.color = Color.white;
            }
        }
    }

    public void fretHit(bool h) //Whenever the fret gets hit. Pass a bool for if the hit was accurate
    {
        if (!endScale)
        {
            if (h) //If it hits, set the color and increase the scale
            {
                sr.color = new Color(0.26667f, 0.49020f, 0.85490f, 0.5f); //May change this and the miss color once our palette is finalized.
                scaleMod = 1.4f;
            }
            else //On a miss, set the color and decrease the scale
            {
                sr.color = new Color(0.96078f, 0.43922f, 0.53333f, 0.8f);
                scaleMod = 0.8f;
            }
        }
    }

    public void noteRippleParticles() {
        rippleParticles.Play();
    }

    public void PlayTambourineSound() {
        tambourineSoundSrc.Play();
    }

    public void setSprite(Sprite newSprite)
    {
        if(newSprite == null)
            Debug.Log("This is null");
        else
            sr.sprite = newSprite;
    }

    public void startingScaling()
    {
        startScale = true;
        started = false;
        sr.enabled = true;
    }

    public void endingScaling()
    {
        endScale = true;
    }

    public void endReset()
    {
        scaleMod = 0;
        startScale = false;
        sr.color = Color.white;
        sr.enabled = false;
    }
}
