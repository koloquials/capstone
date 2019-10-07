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

    public Camera cam;

    public ParticleSystem party;

    private int score;

    public bool inZone = false;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Vector3.zero;
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = ((2 * Mathf.PI) * ((script.songPosinBeats%3) / 3));
        //float angle = 0;
        this.transform.localPosition = PointOnCircle(angle, r);
        //Debug.Log(angle);

        //play g a e m
        if (inZone == true)
        {
            if (Input.GetKey(KeyCode.Space))
                {
                score++;
                Debug.Log(score);

                inZone = false;
                    //reference script and engagE THE SHAKE
                    //camScript cs = cam.GetComponent<camScript>();

                    //cs.Shake();
                }
        }
    }

    Vector3 PointOnCircle(float angle, float radius)
    {
        return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //twue..............
        if (other.gameObject.tag == "beat")
        {
            inZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //nop
        if (other.gameObject.tag == "beat")
        {
            inZone = false;
        }
    }
}
