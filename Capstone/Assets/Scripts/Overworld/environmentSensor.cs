using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class environmentSensor : MonoBehaviour //A gameobject that activates an environment object when a triggering object touches it.
{
    public EnvironmentObject target; //The environment object that gets activated when the sensor is triggered.

    private SpriteRenderer sr; //The spriterenderer. Changes the sensor when it is triggered.

    public Sprite completed;

    private bool locked = true;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision) //Activate the target when the sensor is triggered. Right now the triggering tag is environmentBall. May be subject to change.
    {
        if(collision.gameObject.tag == "environmentBall")
        {
            AudioManager.Instance.PlayPuzzleSound("MuffledClunk");
            target.Activate();
            // sr.color = Color.green;
            sr.sprite = completed;
            locked = false;
        }
    }

    public bool GetStatus() {
        return locked;
    }
}
