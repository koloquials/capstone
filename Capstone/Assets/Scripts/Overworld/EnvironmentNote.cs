using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentNote : MonoBehaviour
{
    string code;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void keyed(bool correct);

    public abstract string getCode();

    public abstract void inRange(bool on);


}
