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

    public bool rotating = false;

	public Transform rotationCenter;

	public float rotationRadius;
    private float angularSpeed;
	float posX;
    float posY;
    float angle = 0f;

    private ScaleObject objectScalerScript;

    public Transform pivot;
    public Transform child;
    public float radius;
    public float rotationSpeed;

    private float rotationTime = 0f;


    void Start() {
        startPos = this.transform.position;
        Debug.Log(startPos);
        script = gameObject.GetComponent<conductorScript>();
        // angularSpeed = SimpleClock.BeatLength();
        Debug.Log("angular speed will be: " + angularSpeed);
        objectScalerScript = gameObject.GetComponent<ScaleObject>();
        // rotationCenter = transform.GetChild(0).gameObject.GetComponent<Transform>();

        radius = (child.position - pivot.position).magnitude;

        Debug.Log("Radius is: "  + radius);
    }

    void Update() { 
        angularSpeed = SimpleClock.BeatLength() * 2;

        if (rotating) {
            
            child.transform.localPosition = PointOnCircle(rotationTime * ((2 * Mathf.PI) / angularSpeed)) * radius;
            
            rotationTime += Time.deltaTime;
        }

        Debug.Log("orbitter currently at: " + child.position);

        // if (rotating) {
        //     Debug.Log("Rotating the orbitter");
        //     posX = rotationCenter.position.x + Mathf.Cos(angle) * rotationRadius;
        //     posY = rotationCenter.position.y + Mathf.Sin(angle) * rotationRadius;
        //     transform.position = new Vector2 (posX, posY);
        //     angle = angle + Time.deltaTime * angularSpeed;

        //     if (angle >= 360f)
        //         angle = 0f;
        // }
	}

    private Vector3 PointOnCircle(float theta) {
        return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
    }



    //    Vector3 PointOnCircle(float angle, float radius) //Used to move the beat indicator in a circle.
    // {
    //     return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), -0.3f);
    // }

    public void StartRotation() {
        rotating = true;
    }

    public void StopRotation() {
        rotating = false;
    }

    public void ResetPosition() {
        transform.position = startPos;
        angle = 0f;
    }

    public void ScaleOrbitter(float time, Vector3 scaleToSize) {
        StartCoroutine(objectScalerScript.Scale(time, scaleToSize));
    }
}