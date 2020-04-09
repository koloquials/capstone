using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MouseMove : MonoBehaviour 
{
    public float speed = 1f;
    private Vector3 mousePosition;
    private Vector3 targetPosition;
    private bool isMoving;
    public GameObject clickAnimation;

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonUp(0)) {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
            targetPosition.y = transform.position.y;
            targetPosition.x = mousePosition.x;

            if (!isMoving) {
                isMoving = true;
            }
        }
        
        if (isMoving) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed*Time.deltaTime);
        }
        
        if (targetPosition == transform.position) {
            isMoving = false;
        }
    }

}