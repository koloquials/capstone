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
    public Material dissolveMat;
    public InteractableHighlight highlightScript;
    public Dissolve dissolveScript;

    public Explodable explodeScript;

    public Texture mainTex;

    // Start is called before the first frame update
    void Start() {
        highlightScript = gameObject.GetComponent<InteractableHighlight>();
        dissolveScript = gameObject.GetComponent<Dissolve>();
        explodeScript = gameObject.GetComponent<Explodable>();


        foreach (Transform shard in transform) {
            MeshRenderer shardRenderer = shard.gameObject.GetComponent<MeshRenderer>();

            // Destroy(shardRenderer);
            shard.gameObject.AddComponent<Dissolve>();

            Dissolve shardDissolve = shard.gameObject.GetComponent<Dissolve>();
            shardDissolve.dissolveMat = this.dissolveMat;
            shardDissolve.mainTex = this.mainTex;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            Dissolve();
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            explodeScript.explode();
        }
    }

    void Dissolve() {
        if (highlightScript != null) 
            highlightScript.enabled = false;
            
        dissolveScript.enabled = true;
    }
}
