using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A controller for objects that will have multiple materials put onto it throughout the game
/// as of right now, this is only functional on objects that will need to switch between the dissolve shader and glow shader 
/// </summary>

public class MaterialManager : MonoBehaviour
{
    public Material[] materials;
    // public Material defaultMat;
    // public Material alternateMat;
    public InteractableHighlight highlightScript;
    public Dissolve dissolveScript;

    // Start is called before the first frame update
    void Start() {
        highlightScript = gameObject.GetComponent<InteractableHighlight>();
        dissolveScript = gameObject.GetComponent<Dissolve>();
        // materials = new Material[] {defaultMat, alternateMat};
    // {
    //     spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    //     spriteRenderer.material = defaultMat;
    //     materials[0] = defaultMat;
    //     materials[1] = dissolveMat;
    //     materials[2] = glowMat;

        // dissolveScript = gameObject.GetComponent<Dissolve>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            Dissolve();
        }
    }

    void Dissolve() {
        if (highlightScript != null) 
            highlightScript.enabled = false;
            
        dissolveScript.enabled = true;
    }
}
