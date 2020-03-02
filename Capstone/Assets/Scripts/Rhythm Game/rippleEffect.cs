using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//usage: put this on a 2D sprite
//intent: feedback purposes--this script goes on the fret (during the rhythm game) and creates
//used for a pulse effect when hitting a note. It'll pulse pink when the correct note was hit, otherwise it
//will pulse blue 
//it only generates a pulse if there was a combination pressed whilst the note was within range with the fret.

public class rippleEffect : MonoBehaviour {

    private SpriteRenderer mySpriteRenderer; 
    private Color spriteColour;
    public bool correct; 

    private int coroutineCounter = 2;

    //instantiate multiple ripples per note 
    private float multiRippleTimer = 3f;

    void Start() {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //spriteColour = SetColour(correct);
        StartCoroutine(Ripple(1f));
    }

    IEnumerator Ripple(float time) {
        Debug.Log("Trying to make the object pulse");
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3 (3.0f, 3.0f, 3.0f);
        
        Color spriteColour = mySpriteRenderer.color;
        
        float currTime = 0f;

        do {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currTime / time);
            spriteColour.a = 1 - (currTime / time);
            mySpriteRenderer.color = spriteColour;
            yield return null;
            currTime += Time.deltaTime;
        } while (currTime <= time);
         
        Destroy(gameObject);
    }
}
