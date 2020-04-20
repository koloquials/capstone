using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

//usage: put this on a interactable 2D Sprite
//intent: feedback purposes--this script is used to make characters that can be interacted with
//highlighted when moused over to indicate that they are interactable 

public class InteractableHighlight : MonoBehaviour
{
    private Color originalColour;
    private Color highlightedColour;

    private SpriteRenderer spriteRenderer;

    private SpriteGlowEffect glowScript;

    void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        glowScript = gameObject.GetComponent<SpriteGlowEffect>();
        originalColour = spriteRenderer.material.color;
        highlightedColour = Color.white;

        LightUp(false, originalColour);
    }

    void OnDisable() {
        glowScript.enabled = false;
    }
    
    void OnMouseEnter() {
        Debug.Log("Activating light up");
        LightUp(true, highlightedColour);
    }

    void OnMouseExit() {
        Debug.Log("Deactivating light up");
        LightUp(false, originalColour);
        // spriteRenderer.material = defaultMat;
    }

    public void LightUp(bool enabled, Color newColour) {
        if (enabled) {
            glowScript.GlowBrightness = 2;
            glowScript.OutlineWidth = 1;
        }
        else {
            glowScript.GlowBrightness = 0;
            glowScript.OutlineWidth = 0;
        }

        spriteRenderer.material.color = newColour;
    }
}
