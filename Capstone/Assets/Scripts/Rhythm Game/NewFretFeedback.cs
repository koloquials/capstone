using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFretFeedback : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;
    
    void Start() {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public IEnumerator ScaleFret(float time) {
        //Debug.Log("Coroutine: trying to scale the fret");
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3 (3.0f, 3.0f, 3.0f);
        
        //Color spriteColour = mySpriteRenderer.color;
        
        float currTime = 0f;

        do {
            //Debug.Log("Scaling the object up");
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currTime / time);
            //spriteColour.a = 1 - (currTime / time);
            //mySpriteRenderer.color = spriteColour;
            yield return null;
            currTime += Time.deltaTime;
        } while (currTime <= time);

        Debug.Log("This coroutine has finished executing. Thx for ur business :) ");
    }

    public void SetSprite(Sprite thisSprite) {
        if (thisSprite != null) {
            mySpriteRenderer.sprite = thisSprite;
        }
    }
}
