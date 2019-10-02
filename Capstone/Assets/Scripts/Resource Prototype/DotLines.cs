using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotLines : MonoBehaviour
{
    //Put this on Terra
    Transform Terra;
    public Transform Mars;
    public Transform Venus;

    LineRenderer lines; //The linerenderer in the script

    public float lineConstant = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Terra = this.transform;
        lines = GetComponent<LineRenderer>(); //This script assumes it's attached to the same object as the linerenderer.
    }

    // Update is called once per frame
    void Update()
    {
        //This is the important stuff
        lines.SetPosition(0, Mars.position);    //This is setting the position of the points on the line. Right now it makes a triangle between Terra, Venus, and Mars
        lines.SetPosition(1, Terra.position);   //I'm not sure if you can add size to the array through the script.
        lines.SetPosition(2, Venus.position);   //You could have a position for each circle to thread through, plus two more for the origin and the mouse,
        lines.SetPosition(3, Mars.position);    //Then set each position after the first to be the mouse position, then set them to the circle positions as you pass through them.
                                                //So for the first circle you pass through, you'd set position 1 to that circle's position, and increment an int to keep track of which position to change.

        //Everything past here is weird dotproduct math that I did for Math for Game Designers last semester. Probably not relevant.
        Vector3 toVenus = Venus.position - Terra.position;
        Vector3 toMars = Mars.position - Terra.position;

        if (Vector3.Dot(toVenus.normalized, toMars.normalized) > 0.96 || Vector3.Dot(toVenus.normalized, toMars.normalized) < -0.96)
        {
            lines.startWidth = lineConstant * (Mathf.Abs(Vector3.Dot(toVenus.normalized, toMars.normalized))-.96f);
            lines.endWidth = lineConstant * (Mathf.Abs(Vector3.Dot(toVenus.normalized, toMars.normalized))-.96f);

            Debug.Log(Vector3.Dot(toVenus.normalized, toMars.normalized));
        }
        else
        {
            lines.startWidth = 0;
            lines.endWidth = 0;
        }
    }
}