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
    public SpriteRenderer spriteRenderer;
    private bool scaleUp;

    void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public IEnumerator Scale(float time, Vector3 scaleToSize)
    {
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = scaleToSize;

        float currTime = 0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currTime / time);
            //scale to max alpha or min alpha depending on which direction we are scaling in 
            // if (scaleUp) {
            //     spriteColour.a = (currTime / time);
            //     spriteRenderer.color = spriteColour;
            // }
            // else {
            //     spriteColour.a = 1 - (currTime / time);
            //     spriteRenderer.color = spriteColour;
            // }
            yield return null;
            currTime += Time.deltaTime;
        } while (currTime <= time);

        finishedScaling = true;
    }
}
