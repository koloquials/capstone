using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

//usage: put this on a interactable 2D Sprite
//intent: feedback purposes--this script is used to make characters that can be interacted with
//highlighted when moused over to indicate that they are interactable 

public class interactableHighlight : MonoBehaviour
{
    private Color originalColour;
    private Color highlightedColour;

    private SpriteRenderer mySpriteRenderer;

    private SpriteGlowEffect glowScript;
    public Material defaultMaterial;

    void Start() {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        glowScript = gameObject.GetComponent<SpriteGlowEffect>();
        originalColour = mySpriteRenderer.material.color;
        highlightedColour = Color.white;
    }

    void OnMouseEnter() {
        Debug.Log("Activating light up");
        LightUp(true, highlightedColour);
    }

    void OnMouseExit() {
        Debug.Log("Deactivating light up");
        LightUp(false, originalColour);
    }

    public void LightUp(bool scriptEnabled, Color newColour) {
        glowScript.enabled = scriptEnabled;
        mySpriteRenderer.material.color = newColour;
    }
}
