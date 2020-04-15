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
    public float angularSpeed;
	float posX;
    float posY;
    float angle = 0f;


    void Start() {
        startPos = this.transform.position;
        Debug.Log(startPos);
        script = gameObject.GetComponent<conductorScript>();
        // angularSpeed = SimpleClock.BeatLength();
        Debug.Log("angular speed will be: " + angularSpeed);
        // rotationCenter = transform.GetChild(0).gameObject.GetComponent<Transform>();
    }

    void Update() { 
        if (rotating) {
            Debug.Log("Rotating the orbitter");
            posX = rotationCenter.position.x + Mathf.Cos(angle) * rotationRadius;
            posY = rotationCenter.position.y + Mathf.Sin(angle) * rotationRadius;
            transform.position = new Vector2 (posX, posY);
            angle = angle + Time.deltaTime * angularSpeed;

            if (angle >= 360f)
                angle = 0f;
        }
	}

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

    public IEnumerator ScaleOrbitter(float time, Vector3 scaleToSize)
    {
        //Debug.Log("Coroutine: trying to scale the fret");
        Vector3 originalScale = gameObject.transform.localScale;
        // Vector3 destinationScale = new Vector3(0.5f, 0.5f, 3f);
        Vector3 destinationScale = scaleToSize;

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