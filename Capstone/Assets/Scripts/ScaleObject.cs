using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on an object that will have to scale up and down in size  
/// 
/// </summary>
public class ScaleObject : MonoBehaviour
{
    
    public bool finishedScaling = false;

    public IEnumerator Scale(float time, Vector3 scaleToSize)
    {
        Debug.Log("Scaling to size: " + scaleToSize);
        Vector3 originalScale = gameObject.transform.localScale;
        // Vector3 destinationScale = new Vector3(0.5f, 0.5f, 3f);
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

        finishedScaling = true;
    }
}
