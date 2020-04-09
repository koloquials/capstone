using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAppear : EnvironmentObject //Something that appears when activated via an environment puzzle activator
{
    SpriteRenderer sr;

    //In the future, may add stuff to instantiate other objects, like environmentBalls.
    //For now it just makes a sprite appear. Could be used to reveal note combinations, like for a note lock.

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        //Instantiate thing
        sr.color = new Color(1, 1, 1, 1);
    }
}
