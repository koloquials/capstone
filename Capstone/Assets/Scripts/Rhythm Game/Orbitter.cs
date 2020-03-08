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
    }

    // Update is called once per frame
    void Update()
    {
        float angle = ((2 * Mathf.PI) * ((script.songPosinBeats % 3) / 3));
        this.transform.localPosition = PointOnCircle(angle, r);
    }

    Vector3 PointOnCircle(float angle, float radius) //Used to move the beat indicator in a circle.
    {
        return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), -0.3f);
    }

    public void ResetPosition() {
        transform.position = startPos;
    }
}