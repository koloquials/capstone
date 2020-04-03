using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public IEnumerator ScaleBackground(float time, Vector3 scaleToSize)
    {
        //Debug.Log("Coroutine: trying to scale the fret");
        Vector3 originalScale = gameObject.transform.localScale;
        // Vector3 destinationScale = new Vector3(24f, 12f, 1f);
        Vector3 destinationScale = scaleToSize;

        //Color spriteColour = mySpriteRenderer.color;

        float currTime = 0f;

        do
        {
            //Debug.Log("Scaling the object up");
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currTime / time);
            //spriteColour.a = 1 - (currTime / time);
            //mySpriteRenderer.color = spriteColour;
            yield return null;
            currTime += Time.deltaTime;
        } while (currTime <= time);
    }
}
