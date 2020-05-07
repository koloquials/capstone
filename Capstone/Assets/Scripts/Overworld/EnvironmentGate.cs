using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGate : EnvironmentObject //A gate that obstructs the player until activated.
{

    public ExampleVariableStorage varStor; //The example variable storage for the yarn system. Used to tell dialogue when the puzzle is completed.

    // Start is called before the first frame update
    void Start()
    {
        varStor.SetValue("$completedPuzzle", new Yarn.Value(0)); //Set the value to 0 at the start
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //When the object is activated
    public override void Activate() //When activated, disappear. May add functionality to have it move instead of just vanishing in the future.
    {
        //Note, if a scene contains more than one puzzle, could replace "$completedRhythm" with a constructed string that has the number of what puzzle was completed
        varStor.SetValue("$completedPuzzle", new Yarn.Value(1)); //Tell yarn that the puzzle was completed
        this.gameObject.GetComponent<Dissolve>().enabled = true;
        // gameObject.SetActive(false);
    }
}
