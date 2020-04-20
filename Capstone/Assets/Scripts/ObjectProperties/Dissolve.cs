using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script to control the properties of the dissolve custom material and shader
/// this typically should not be the default material of an object. This script should be deactivated at the beginning and activated as needed.
/// the gameObject that this object is on will be deactivated once it has finished executing! 
/// </summary>

public class Dissolve : MonoBehaviour
{
    public Material dissolveMat;

    public Texture mainTex;

    public SpriteRenderer spriteRenderer;

    void OnEnable() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        dissolveMat.SetTexture("_MainTex", mainTex);
        spriteRenderer.material = dissolveMat;

        StartCoroutine(DissolveObject(5f));
    }

    //gradually make the current object dissolve 
    public IEnumerator DissolveObject(float time) {
        float originalFadeValue = dissolveMat.GetFloat("_Fade");

        float currTime = 0f;

        do
        {
            spriteRenderer.material.SetFloat("_Fade", Mathf.Lerp(originalFadeValue, 0f, currTime/time));
            //spriteColour.a = 1 - (currTime / time);
            //mySpriteRenderer.color = spriteColour;
            yield return null;
            currTime += Time.deltaTime;
        } while (currTime <= time);

        this.gameObject.SetActive(false);
    }
}
