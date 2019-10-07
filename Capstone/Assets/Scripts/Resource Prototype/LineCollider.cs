using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCollider : MonoBehaviour
{
    public GameObject lineObj;
    drawLine lineScript;
    // Start is called before the first frame update
    void Start()
    {
        lineScript = lineObj.GetComponent<drawLine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Proto_Null")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            lineScript.notHit = false;
            Debug.Log("Collided with Object: " + collision.gameObject.name);
        }
    }
}
