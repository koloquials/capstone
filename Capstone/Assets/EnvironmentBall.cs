using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnvironmentBall : MonoBehaviour
{
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = this.gameObject.transform.localPosition;   
    }

    // Update is called once per frame
    void Update()
    {
        //the ball should stay within the x bounds of where it starts and the spinner object
        if (transform.localPosition.x > 5.5f)
            transform.localPosition = startPos;

        if (transform.localPosition.x <= (startPos.x - 0.5f))
            transform.localPosition = startPos;



        //ball should stay within the y bounds of where it starts and the ground 
        if (transform.localPosition.y > (startPos.y + 2f))
            transform.localPosition = startPos;
        if (transform.localPosition.y <= -4.1f)
            transform.localPosition = startPos;

    }
}
