using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// for controlling the blue circle orbiting the fret
/// </summary>

public class Orbitter : MonoBehaviour
{
    private Vector3 startPos;

    public bool rotating = false;

    private float angularSpeed;

    private ScaleObject objectScalerScript;

    public Transform pivot;
    public Transform child;
    public float radius;
    public float rotationSpeed;

    private float rotationTime = 0f;


    void Start() {
        startPos = this.transform.position;
        Debug.Log("angular speed will be: " + angularSpeed);
        objectScalerScript = gameObject.GetComponent<ScaleObject>();

        radius = (child.position - pivot.position).magnitude;

        Debug.Log("Radius is: "  + radius);
    }

    void Update() { 
        angularSpeed = SimpleClock.Instance.(SimpleClock.BeatLength() * 2);

        if (rotating) {
            child.transform.localPosition = PointOnCircle(rotationTime * ((2 * Mathf.PI) / angularSpeed)) * radius;
            
            rotationTime += Time.deltaTime;
        }
	}

    private Vector3 PointOnCircle(float theta) {
        return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
    }

    public void StartRotation() {
        rotating = true;
    }

    public void StopRotation() {
        rotating = false;
    }

    public void ResetPosition() {
        transform.position = startPos;
        rotationTime = 0f;
    }

    public void ScaleOrbitter(float time, Vector3 scaleToSize) {
        StartCoroutine(objectScalerScript.Scale(time, scaleToSize));
    }
}