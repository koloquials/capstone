using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//usage: put this on a interactable 2D Sprite
//intent: feedback purposes--this script is used to make characters that can be interacted with
//highlighted when moused over to indicate that they are interactable 

public class interactableHighlight : MonoBehaviour
{
    private Color originalColour;
    private Color highlightedColour;

    private SpriteRenderer mySpriteRenderer;

    void Start() {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalColour = mySpriteRenderer.material.color;
        highlightedColour = Color.white;
    }

    void OnMouseEnter() {
        Debug.Log("Activating light up");
        originalColour = mySpriteRenderer.material.color;
        mySpriteRenderer.material.color = Color.grey;
    }

    void OnMouseExit() {
        Debug.Log("Deactivating light up");
        mySpriteRenderer.material.color = originalColour;
    }
}
