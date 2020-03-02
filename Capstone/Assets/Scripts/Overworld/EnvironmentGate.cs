using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGate : EnvironmentObject //A gate that obstructs the player until activated.
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //When the object is activated
    public override void Activate() //When activated, disappear. May add functionality to have it move instead of just vanishing in the future.
    {
        gameObject.SetActive(false);
    }
}
