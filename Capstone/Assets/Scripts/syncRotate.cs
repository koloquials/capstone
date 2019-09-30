using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syncRotate : MonoBehaviour

{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, conductorScript.instance.songPosinBeats));
    
    }
}
