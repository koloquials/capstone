using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentObject : MonoBehaviour //The abstract for an object in the environment that responds to some sort of stimuli.
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //When the object is activated, some effect happens. Scripts that extend this one specify what activation does.
    public abstract void Activate();
}
