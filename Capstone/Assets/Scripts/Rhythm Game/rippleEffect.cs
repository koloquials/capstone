using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//usage: put this on a sprite
//intent: feedback purposes. this script goes on a circular sprite used for a pulse effect
//when hitting a note in the rhythm game 
public class rippleEffect : MonoBehaviour
{
    SpriteRenderer mySpriteRenderer; 

    void Start() {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Debug.Log("starting coroutine");
        //put it in Start() bc in Update() it'll keep calling itself a million times
        StartCoroutine(ScaleOverTime(1f));
    }

    
    IEnumerator ScaleOverTime(float time) {
        Debug.Log("Trying to make the object pulse");
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3 (3.0f, 3.0f, 3.0f);

        Color spriteColour = mySpriteRenderer.color;
        
        float currTime = 0f;

        do {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currTime / time);
            spriteColour.a = 1 - (currTime / time);
            //mySpriteRenderer.color.a don't work 
            mySpriteRenderer.color = spriteColour;
            yield return null;
            currTime += Time.deltaTime;
        } while (currTime <= time);
         
        Destroy(gameObject);
    }

}
