using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBehaviour : MonoBehaviour
{
    // Transform of the GameObject you want to shake
    private Transform transform;
    
    // Desired duration of the shake effect
    private float shakeDuration = 0f;
    
    // A measure of magnitude for the shake. Tweak based on your preference
    private float shakeMagnitude = 0f;
    
    // A measure of how quickly the shake effect should evaporate
    private float dampingSpeed = 1.0f;
    
    // The initial position of the GameObject
    Vector3 startPos;

    void Awake() {
        transform = this.gameObject.GetComponent<Transform>();
        startPos = transform.localPosition;
    }

    public void TriggerShake(float shakeDuration, float shakeMagnitude) {
        this.shakeDuration = shakeDuration;
        this.shakeMagnitude = shakeMagnitude;

        startPos = transform.localPosition;

        StartCoroutine(Shake());
    }

    public IEnumerator Shake() {
        do {
            transform.localPosition = startPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
            yield return null;

        } while (shakeDuration > 0f);
        
        transform.localPosition = startPos;
    }
}
