using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpinner : MonoBehaviour //Spins rapidly until a note is played, which stops the spin for a time. NOTE, the sprite that shows the corresponding code should be nearby the spinner, but not parented to it, as otherwise it spins with it. Though that might be a fun puzzle, it's not ideal for every spinner.
{
    SpriteRenderer sr;

    float countdown = 0; //How long it stops spinning, in seconds

    public string code; //The rhythm combination that turns off the spin

    public float rotateSpeed = -3f; //How many degrees it rotates per tick

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(countdown > 0) //If stopped, count down the time stopped
        {
            countdown -= Time.deltaTime;
            if(countdown < 2f)
            {
                if(countdown % 0.2 >= 0.1)
                {
                    sr.color = Color.blue;
                }
                else
                {
                    sr.color = Color.white;
                }
            }
        }
        else //Otherwise spin
        {
            sr.color = Color.white;
            transform.Rotate(0, 0, rotateSpeed);
        }
    }

    public void keyed()
    {
        sr.color = Color.blue;
        countdown = 5f;
    }

    public string getCode()
    {
        return code;
    }
}
