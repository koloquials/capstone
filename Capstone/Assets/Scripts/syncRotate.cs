using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syncRotate : MonoBehaviour

{
    public conductorScript script;

    //tHTANK S YUO IAN
    public float theta;
    public float r = 1;
    public float speed = .1f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = ((2 * Mathf.PI) * ((script.songPosinBeats%3) / 3));
        //float angle = 0;
        this.transform.localPosition = PointOnCircle(angle, r);
        Debug.Log(angle);
    }

    Vector3 PointOnCircle(float angle, float radius)
    {
        return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
    }
}
