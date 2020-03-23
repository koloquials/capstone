using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// for moving the blue circle orbiting the fret
/// </summary>

public class Orbitter : MonoBehaviour
{
    public conductorScript script;
    public float r = 1;

    private Vector3 startPos;

    void Start() {
        startPos = this.transform.position;
        Debug.Log(startPos);
        script = gameObject.GetComponent<conductorScript>();
    }

    // Update is called once per frame
    void Update()
    {
    //     Debug.Log("trying to move the orbitter");
        float angle = ((2 * Mathf.PI) * ((script.songPosinBeats % 3) / 3));
        this.transform.position = PointOnCircle(angle, r);
    }

    Vector3 PointOnCircle(float angle, float radius) //Used to move the beat indicator in a circle.
    {
        return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), -0.3f);
    }

    public void ResetPosition() {
        transform.position = startPos;
    }

    public IEnumerator ScaleOrbitter(float time)
    {
        //Debug.Log("Coroutine: trying to scale the fret");
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3(0.5f, 0.5f, 3f);

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